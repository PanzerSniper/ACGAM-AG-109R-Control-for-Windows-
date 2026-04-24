using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Ac109RDriverWin.Hardware
{
    /// <summary>
    /// Describes one HID interface that matches the keyboard vendor and product identifiers.
    /// </summary>
    internal sealed class HidDeviceInfo
    {
        public HidDeviceInfo(string devicePath, ushort vendorId, ushort productId, ushort inputReportLength, ushort outputReportLength, ushort featureReportLength)
        {
            DevicePath = devicePath;
            VendorId = vendorId;
            ProductId = productId;
            InputReportLength = inputReportLength;
            OutputReportLength = outputReportLength;
            FeatureReportLength = featureReportLength;
        }

        public string DevicePath { get; private set; }

        public ushort VendorId { get; private set; }

        public ushort ProductId { get; private set; }

        public ushort InputReportLength { get; private set; }

        public ushort OutputReportLength { get; private set; }

        public ushort FeatureReportLength { get; private set; }

        public bool IsPreferredInterface
        {
            get { return DevicePath.IndexOf("mi_01", StringComparison.OrdinalIgnoreCase) >= 0; }
        }

        /// <summary>
        /// Returns a concise diagnostic string for logs and detection messages.
        /// </summary>
        public override string ToString()
        {
            return string.Format("VID_{0:X4}&PID_{1:X4}, in={2}, out={3}{4}", VendorId, ProductId, InputReportLength, OutputReportLength, IsPreferredInterface ? ", MI_01" : string.Empty);
        }
    }

    /// <summary>
    /// Low-level Windows HID transport used to write command packets and read firmware responses.
    /// </summary>
    internal sealed class HidDeviceConnection : IDisposable
    {
        private const int ErrorIoPending = 997;
        private const int ErrorNoMoreItems = 259;
        private const int ErrorInsufficientBuffer = 122;
        private const uint DigcfPresent = 0x00000002;
        private const uint DigcfDeviceInterface = 0x00000010;
        private const uint GenericRead = 0x80000000;
        private const uint GenericWrite = 0x40000000;
        private const uint FileShareRead = 0x00000001;
        private const uint FileShareWrite = 0x00000002;
        private const uint OpenExisting = 3;
        private const uint FileAttributeNormal = 0x00000080;
        private const uint FileFlagOverlapped = 0x40000000;
        private const uint WaitObject0 = 0;
        private const uint WaitTimeout = 258;
        private const int HidpStatusSuccess = 0x00110000;

        private readonly SafeFileHandle handle;
        private readonly HidDeviceInfo info;

        private HidDeviceConnection(SafeFileHandle handle, HidDeviceInfo info)
        {
            this.handle = handle;
            this.info = info;
        }

        /// <summary>
        /// Enumerates HID interfaces and filters them by vendor and product identifiers.
        /// </summary>
        public static IList<HidDeviceInfo> FindDevices(ushort vendorId, ushort productId)
        {
            List<HidDeviceInfo> result = new List<HidDeviceInfo>();
            Guid hidGuid;
            HidD_GetHidGuid(out hidGuid);

            IntPtr deviceInfoSet = SetupDiGetClassDevs(ref hidGuid, IntPtr.Zero, IntPtr.Zero, DigcfPresent | DigcfDeviceInterface);
            if (deviceInfoSet == new IntPtr(-1))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to enumerate HID devices.");
            }

            try
            {
                uint memberIndex = 0;
                while (true)
                {
                    SpDeviceInterfaceData interfaceData = new SpDeviceInterfaceData();
                    interfaceData.CbSize = Marshal.SizeOf(typeof(SpDeviceInterfaceData));

                    bool enumOk = SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref hidGuid, memberIndex, ref interfaceData);
                    if (!enumOk)
                    {
                        int error = Marshal.GetLastWin32Error();
                        if (error == ErrorNoMoreItems)
                        {
                            break;
                        }

                        throw new Win32Exception(error, "HID enumeration failed.");
                    }

                    string path = GetDevicePath(deviceInfoSet, ref interfaceData);
                    HidDeviceInfo device = TryCreateInfo(path, vendorId, productId);
                    if (device != null)
                    {
                        result.Add(device);
                    }

                    memberIndex++;
                }
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }

            result.Sort(CompareDevices);
            return result;
        }

        /// <summary>
        /// Opens a HID interface for overlapped read/write operations.
        /// </summary>
        public static HidDeviceConnection Open(HidDeviceInfo deviceInfo)
        {
            if (deviceInfo == null)
            {
                throw new ArgumentNullException("deviceInfo");
            }

            SafeFileHandle fileHandle = OpenHandle(deviceInfo.DevicePath, GenericRead | GenericWrite, true);
            if (fileHandle == null || fileHandle.IsInvalid)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to open the keyboard for read/write access.");
            }

            return new HidDeviceConnection(fileHandle, deviceInfo);
        }

        /// <summary>
        /// Sends one 64-byte protocol packet and waits for the keyboard acknowledgment.
        /// </summary>
        public void WritePacketAndReadResponse(byte[] packet, int timeoutMs)
        {
            if (packet == null)
            {
                throw new ArgumentNullException("packet");
            }

            if (packet.Length != PacketBuilder.PacketLength)
            {
                throw new ArgumentException("The packet must be 64 bytes long.", "packet");
            }

            byte[] outputReport = CreateOutputReport(packet);
            int written = ExecuteIo(outputReport, true, timeoutMs);
            if (written != outputReport.Length)
            {
                throw new IOException("Incomplete HID write.");
            }

            byte[] inputReport = new byte[Math.Max(info.InputReportLength, (ushort)(PacketBuilder.PacketLength + 1))];
            int read = ExecuteIo(inputReport, false, timeoutMs);
            if (read <= 0)
            {
                throw new IOException("Empty HID response.");
            }
        }

        /// <summary>
        /// Closes the native HID handle.
        /// </summary>
        public void Dispose()
        {
            if (handle != null)
            {
                handle.Dispose();
            }
        }

        /// <summary>
        /// Creates a Windows HID output report, including a leading report ID byte when required.
        /// </summary>
        private byte[] CreateOutputReport(byte[] packet)
        {
            int reportLength = info.OutputReportLength;
            if (reportLength <= 0)
            {
                reportLength = PacketBuilder.PacketLength + 1;
            }

            if (reportLength < packet.Length)
            {
                throw new IOException("The HID output report is too short for this protocol.");
            }

            byte[] report = new byte[reportLength];
            if (reportLength == packet.Length)
            {
                Buffer.BlockCopy(packet, 0, report, 0, packet.Length);
            }
            else
            {
                Buffer.BlockCopy(packet, 0, report, 1, packet.Length);
            }

            return report;
        }

        /// <summary>
        /// Executes a single overlapped read or write and enforces a timeout.
        /// </summary>
        private int ExecuteIo(byte[] buffer, bool write, int timeoutMs)
        {
            if (handle == null || handle.IsInvalid || handle.IsClosed)
            {
                throw new ObjectDisposedException("HidDeviceConnection");
            }

            GCHandle pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            IntPtr eventHandle = CreateEvent(IntPtr.Zero, true, false, null);
            if (eventHandle == IntPtr.Zero)
            {
                pinnedBuffer.Free();
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to create the I/O event.");
            }

            NativeOverlapped overlapped = new NativeOverlapped();
            overlapped.EventHandle = eventHandle;

            try
            {
                int immediateBytes;
                bool started = write
                    ? WriteFile(handle, pinnedBuffer.AddrOfPinnedObject(), buffer.Length, out immediateBytes, ref overlapped)
                    : ReadFile(handle, pinnedBuffer.AddrOfPinnedObject(), buffer.Length, out immediateBytes, ref overlapped);

                int error = Marshal.GetLastWin32Error();
                if (!started && error != ErrorIoPending)
                {
                    throw new Win32Exception(error, write ? "HID write failed." : "HID read failed.");
                }

                uint waitResult = WaitForSingleObject(eventHandle, (uint)timeoutMs);
                if (waitResult == WaitTimeout)
                {
                    CancelIoEx(handle, ref overlapped);
                    throw new TimeoutException(write ? "HID write timed out." : "HID read timed out.");
                }

                if (waitResult != WaitObject0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "HID I/O wait failed.");
                }

                int transferred;
                if (!GetOverlappedResult(handle, ref overlapped, out transferred, false))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), write ? "Invalid HID write result." : "Invalid HID read result.");
                }

                return transferred;
            }
            finally
            {
                CloseHandle(eventHandle);
                pinnedBuffer.Free();
            }
        }

        /// <summary>
        /// Reads the symbolic device path from SetupAPI.
        /// </summary>
        private static string GetDevicePath(IntPtr deviceInfoSet, ref SpDeviceInterfaceData interfaceData)
        {
            int requiredSize;
            bool detailOk = SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref interfaceData, IntPtr.Zero, 0, out requiredSize, IntPtr.Zero);
            int error = Marshal.GetLastWin32Error();
            if (detailOk || error != ErrorInsufficientBuffer)
            {
                throw new Win32Exception(error, "Unable to read the HID path size.");
            }

            IntPtr detailBuffer = Marshal.AllocHGlobal(requiredSize);
            try
            {
                Marshal.WriteInt32(detailBuffer, IntPtr.Size == 8 ? 8 : 4 + Marshal.SystemDefaultCharSize);
                if (!SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref interfaceData, detailBuffer, requiredSize, out requiredSize, IntPtr.Zero))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to read the HID path.");
                }

                return Marshal.PtrToStringAuto(IntPtr.Add(detailBuffer, 4));
            }
            finally
            {
                Marshal.FreeHGlobal(detailBuffer);
            }
        }

        /// <summary>
        /// Opens a candidate path long enough to query HID attributes and report lengths.
        /// </summary>
        private static HidDeviceInfo TryCreateInfo(string devicePath, ushort vendorId, ushort productId)
        {
            if (string.IsNullOrEmpty(devicePath))
            {
                return null;
            }

            string lowerPath = devicePath.ToLowerInvariant();
            string vidPart = "vid_" + vendorId.ToString("x4");
            string pidPart = "pid_" + productId.ToString("x4");

            if (lowerPath.IndexOf(vidPart, StringComparison.Ordinal) < 0 || lowerPath.IndexOf(pidPart, StringComparison.Ordinal) < 0)
            {
                return null;
            }

            SafeFileHandle fileHandle = OpenHandle(devicePath, GenericRead | GenericWrite, false);
            if (fileHandle == null || fileHandle.IsInvalid)
            {
                if (fileHandle != null)
                {
                    fileHandle.Dispose();
                }

                return null;
            }

            using (fileHandle)
            {
                HiddAttributes attributes = new HiddAttributes();
                attributes.Size = Marshal.SizeOf(typeof(HiddAttributes));
                if (!HidD_GetAttributes(fileHandle, ref attributes))
                {
                    return null;
                }

                if (attributes.VendorID != vendorId || attributes.ProductID != productId)
                {
                    return null;
                }

                ushort inputLength = 0;
                ushort outputLength = 0;
                ushort featureLength = 0;
                IntPtr preparsedData;

                if (HidD_GetPreparsedData(fileHandle, out preparsedData))
                {
                    try
                    {
                        HidpCaps caps = new HidpCaps();
                        caps.Reserved = new ushort[17];
                        int capsStatus = HidP_GetCaps(preparsedData, ref caps);
                        if (capsStatus == HidpStatusSuccess)
                        {
                            inputLength = caps.InputReportByteLength;
                            outputLength = caps.OutputReportByteLength;
                            featureLength = caps.FeatureReportByteLength;
                        }
                    }
                    finally
                    {
                        HidD_FreePreparsedData(preparsedData);
                    }
                }

                if (outputLength > 0 && outputLength < PacketBuilder.PacketLength)
                {
                    return null;
                }

                return new HidDeviceInfo(devicePath, attributes.VendorID, attributes.ProductID, inputLength, outputLength, featureLength);
            }
        }

        /// <summary>
        /// Sorts candidate interfaces so the known lighting interface is tried first.
        /// </summary>
        private static int CompareDevices(HidDeviceInfo left, HidDeviceInfo right)
        {
            int leftScore = left.IsPreferredInterface ? 0 : 1;
            int rightScore = right.IsPreferredInterface ? 0 : 1;

            if (leftScore != rightScore)
            {
                return leftScore.CompareTo(rightScore);
            }

            int leftOutputDistance = Math.Abs(left.OutputReportLength - (PacketBuilder.PacketLength + 1));
            int rightOutputDistance = Math.Abs(right.OutputReportLength - (PacketBuilder.PacketLength + 1));
            if (leftOutputDistance != rightOutputDistance)
            {
                return leftOutputDistance.CompareTo(rightOutputDistance);
            }

            return string.Compare(left.DevicePath, right.DevicePath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Opens a Windows file handle for a HID interface path.
        /// </summary>
        private static SafeFileHandle OpenHandle(string path, uint desiredAccess, bool overlapped)
        {
            uint flags = FileAttributeNormal;
            if (overlapped)
            {
                flags |= FileFlagOverlapped;
            }

            return CreateFile(path, desiredAccess, FileShareRead | FileShareWrite, IntPtr.Zero, OpenExisting, flags, IntPtr.Zero);
        }

        [DllImport("hid.dll")]
        private static extern void HidD_GetHidGuid(out Guid hidGuid);

        [DllImport("hid.dll", SetLastError = true)]
        private static extern bool HidD_GetAttributes(SafeFileHandle hidDeviceObject, ref HiddAttributes attributes);

        [DllImport("hid.dll", SetLastError = true)]
        private static extern bool HidD_GetPreparsedData(SafeFileHandle hidDeviceObject, out IntPtr preparsedData);

        [DllImport("hid.dll", SetLastError = true)]
        private static extern bool HidD_FreePreparsedData(IntPtr preparsedData);

        [DllImport("hid.dll")]
        private static extern int HidP_GetCaps(IntPtr preparsedData, ref HidpCaps capabilities);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, IntPtr enumerator, IntPtr hwndParent, uint flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr deviceInfoData, ref Guid interfaceClassGuid, uint memberIndex, ref SpDeviceInterfaceData deviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref SpDeviceInterfaceData deviceInterfaceData, IntPtr deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, out int requiredSize, IntPtr deviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern SafeFileHandle CreateFile(string fileName, uint desiredAccess, uint shareMode, IntPtr securityAttributes, uint creationDisposition, uint flagsAndAttributes, IntPtr templateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateEvent(IntPtr eventAttributes, bool manualReset, bool initialState, string name);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint WaitForSingleObject(IntPtr handle, uint milliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CancelIoEx(SafeFileHandle fileHandle, ref NativeOverlapped overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetOverlappedResult(SafeFileHandle fileHandle, ref NativeOverlapped overlapped, out int bytesTransferred, bool wait);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadFile(SafeFileHandle fileHandle, IntPtr buffer, int bytesToRead, out int bytesRead, ref NativeOverlapped overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteFile(SafeFileHandle fileHandle, IntPtr buffer, int bytesToWrite, out int bytesWritten, ref NativeOverlapped overlapped);

        [StructLayout(LayoutKind.Sequential)]
        private struct SpDeviceInterfaceData
        {
            public int CbSize;
            public Guid InterfaceClassGuid;
            public int Flags;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HiddAttributes
        {
            public int Size;
            public ushort VendorID;
            public ushort ProductID;
            public ushort VersionNumber;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HidpCaps
        {
            public ushort Usage;
            public ushort UsagePage;
            public ushort InputReportByteLength;
            public ushort OutputReportByteLength;
            public ushort FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public ushort[] Reserved;
            public ushort NumberLinkCollectionNodes;
            public ushort NumberInputButtonCaps;
            public ushort NumberInputValueCaps;
            public ushort NumberInputDataIndices;
            public ushort NumberOutputButtonCaps;
            public ushort NumberOutputValueCaps;
            public ushort NumberOutputDataIndices;
            public ushort NumberFeatureButtonCaps;
            public ushort NumberFeatureValueCaps;
            public ushort NumberFeatureDataIndices;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeOverlapped
        {
            public IntPtr Internal;
            public IntPtr InternalHigh;
            public uint Offset;
            public uint OffsetHigh;
            public IntPtr EventHandle;
        }
    }
}
