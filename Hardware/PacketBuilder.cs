using System;

namespace Ac109RDriverWin.Hardware
{
    /// <summary>
    /// Builds the 64-byte command packet expected by the keyboard firmware.
    /// </summary>
    internal static class PacketBuilder
    {
        /// <summary>
        /// Fixed AC109R command packet length.
        /// </summary>
        public const int PacketLength = 64;

        /// <summary>
        /// Maximum command payload that fits around the protocol CRC field.
        /// </summary>
        public const int MaxCommandLength = 62;

        /// <summary>
        /// Places command bytes into the AC109R packet layout and writes the CRC at byte offsets 6 and 7.
        /// </summary>
        public static byte[] Build(byte[] command, int size)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (size <= 0 || size > MaxCommandLength || size > command.Length)
            {
                throw new ArgumentOutOfRangeException("size");
            }

            byte[] packet = new byte[PacketLength];

            if (size <= 6)
            {
                Buffer.BlockCopy(command, 0, packet, 0, size);
            }
            else
            {
                Buffer.BlockCopy(command, 0, packet, 0, 6);
                Buffer.BlockCopy(command, 6, packet, 8, size - 6);
            }

            ushort crc = Crc16Ccitt.Compute(packet);
            packet[6] = (byte)(crc & 0xff);
            packet[7] = (byte)((crc >> 8) & 0xff);

            return packet;
        }
    }
}
