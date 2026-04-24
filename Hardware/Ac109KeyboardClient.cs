using Ac109RDriverWin.Profiles;
using System;
using System.Collections.Generic;
using System.IO;

namespace Ac109RDriverWin.Hardware
{
    /// <summary>
    /// High-level client for the AC109R lighting protocol.
    /// </summary>
    internal sealed class Ac109KeyboardClient : IDisposable
    {
        /// <summary>
        /// USB vendor identifier used by the AC109R keyboard.
        /// </summary>
        public const ushort VendorId = 0x1EA7;

        /// <summary>
        /// USB product identifier used by the AC109R keyboard.
        /// </summary>
        public const ushort ProductId = 0x0907;

        private const int StreamBufferLength = 56;
        private const int DefaultTimeoutMs = 2000;
        private readonly HidDeviceConnection device;

        private Ac109KeyboardClient(HidDeviceConnection device)
        {
            this.device = device;
        }

        /// <summary>
        /// Finds all accessible HID interfaces that match the AC109R vendor and product IDs.
        /// </summary>
        public static IList<HidDeviceInfo> FindDevices()
        {
            return HidDeviceConnection.FindDevices(VendorId, ProductId);
        }

        /// <summary>
        /// Opens the preferred lighting interface of the keyboard.
        /// </summary>
        public static Ac109KeyboardClient Open()
        {
            IList<HidDeviceInfo> devices = FindDevices();
            if (devices.Count == 0)
            {
                throw new IOException("AC-109R keyboard not found or lighting interface is inaccessible.");
            }

            return new Ac109KeyboardClient(HidDeviceConnection.Open(devices[0]));
        }

        /// <summary>
        /// Sends the firmware ping command and waits for the keyboard response.
        /// </summary>
        public void Ping()
        {
            SendCommand(new byte[] { 0x0c }, 1);
        }

        /// <summary>
        /// Selects one of the three onboard custom lighting profiles.
        /// </summary>
        public void SetProfile(int profile)
        {
            ValidateProfile(profile);
            SendCommand(new byte[] { 0x0b, (byte)(0x01 + profile) }, 2);
        }

        /// <summary>
        /// Turns off all LEDs for the selected profile.
        /// </summary>
        public void ClearProfile(int profile)
        {
            ValidateProfile(profile);
            SendCommand(new byte[] { 0x21, (byte)(0x01 + profile), 0x06 }, 3);
        }

        /// <summary>
        /// Fills a profile with one RGB color and makes it active.
        /// </summary>
        public void FillProfile(int profile, byte red, byte green, byte blue)
        {
            SendProfile(profile, ProfileParser.CreateFilled(red, green, blue));
        }

        /// <summary>
        /// Writes a full static profile using the same transfer sequence as the Linux driver.
        /// </summary>
        public void SendProfile(int profile, KeyColor[] keys)
        {
            SendProfile(profile, keys, true, true);
        }

        /// <summary>
        /// Writes a profile payload, optionally clearing and activating it around the transfer.
        /// </summary>
        private void SendProfile(int profile, KeyColor[] keys, bool clearBeforeWrite, bool activateAfterWrite)
        {
            ValidateProfile(profile);
            if (keys == null || keys.Length != ProfileParser.KeyboardKeyCount)
            {
                throw new ArgumentException("The profile must contain 131 key slots.", "keys");
            }

            if (clearBeforeWrite)
            {
                ClearProfile(profile);
            }

            StreamState state = new StreamState(profile);
            StreamSend(state, new byte[]
            {
                0x00, 0x02, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x14, 0x04, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                0x2e, 0x04, 0x00, 0x00, 0x0d, 0x00, 0x00, 0x00,
                0xce, 0x05, 0x00, 0x00, 0x0c, 0x00, 0x00, 0x00,
                0x06, 0x07, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00
            });

            byte[] keyBase = new byte[] { 0xff, 0xff, 0xff, 0xff };
            for (int i = 0; i < 116; i++)
            {
                StreamSend(state, keyBase);
            }

            StreamSend(state, new byte[] { 0x00, 0x00, 0x10, 0x02 });

            for (int i = 0; i < keys.Length; i++)
            {
                KeyColor key = keys[i];
                StreamSend(state, new byte[] { key.Red, key.Green, key.Blue, key.Alpha });
            }

            byte[] zero = new byte[] { 0x00 };
            for (int i = 0; i < 1894; i++)
            {
                StreamSend(state, zero);
            }

            StreamEnd(state);

            if (activateAfterWrite)
            {
                SetProfile(profile);
            }
        }

        /// <summary>
        /// Closes the HID handle associated with this client.
        /// </summary>
        public void Dispose()
        {
            if (device != null)
            {
                device.Dispose();
            }
        }

        /// <summary>
        /// Builds, writes, and validates one command packet.
        /// </summary>
        private void SendCommand(byte[] command, int size)
        {
            byte[] packet = PacketBuilder.Build(command, size);
            device.WritePacketAndReadResponse(packet, DefaultTimeoutMs);
        }

        /// <summary>
        /// Appends arbitrary data to the 56-byte stream buffer and flushes full chunks.
        /// </summary>
        private void StreamSend(StreamState state, byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (state.BufferFilled == StreamBufferLength)
                {
                    StreamFlushFull(state);
                }

                state.Buffer[state.BufferFilled] = data[i];
                state.BufferFilled++;
            }
        }

        /// <summary>
        /// Flushes one complete 56-byte stream chunk.
        /// </summary>
        private void StreamFlushFull(StreamState state)
        {
            SendStreamPacket(state, StreamBufferLength);
            state.Sent += StreamBufferLength;
            state.BufferFilled = 0;
            Array.Clear(state.Buffer, 0, state.Buffer.Length);
        }

        /// <summary>
        /// Sends the final partial stream packet.
        /// </summary>
        private void StreamEnd(StreamState state)
        {
            SendStreamPacket(state, state.BufferFilled);
        }

        /// <summary>
        /// Sends one stream command with the current payload offset.
        /// </summary>
        private void SendStreamPacket(StreamState state, int length)
        {
            byte[] command = new byte[6 + StreamBufferLength];
            command[0] = 0x27;
            command[1] = (byte)(0x01 + state.Profile);
            command[2] = (byte)(state.Sent & 0xff);
            command[3] = (byte)((state.Sent >> 8) & 0xff);
            command[4] = (byte)((state.Sent >> 16) & 0xff);
            command[5] = (byte)length;
            Buffer.BlockCopy(state.Buffer, 0, command, 6, length);
            SendCommand(command, command.Length);
        }

        /// <summary>
        /// Ensures that the selected onboard profile number is supported by the keyboard.
        /// </summary>
        private static void ValidateProfile(int profile)
        {
            if (profile < 1 || profile > 3)
            {
                throw new ArgumentOutOfRangeException("profile", "The profile number must be between 1 and 3.");
            }
        }

        /// <summary>
        /// Mutable state used while splitting a profile into protocol stream packets.
        /// </summary>
        private sealed class StreamState
        {
            public StreamState(int profile)
            {
                Profile = profile;
                Buffer = new byte[StreamBufferLength];
            }

            public int Sent;
            public int BufferFilled;
            public byte[] Buffer;
            public int Profile;
        }
    }
}
