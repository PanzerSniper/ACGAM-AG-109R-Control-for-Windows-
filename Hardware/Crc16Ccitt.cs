namespace Ac109RDriverWin.Hardware
{
    /// <summary>
    /// Computes the CRC-CCITT checksum variant used by the AG109R lighting protocol.
    /// </summary>
    internal static class Crc16Ccitt
    {
        private const ushort Polynomial = 0x1021;
        private static readonly ushort[] Table = CreateTable();

        /// <summary>
        /// Computes a CRC-CCITT value with the 0xFFFF start value used by the original Linux driver.
        /// </summary>
        public static ushort Compute(byte[] data)
        {
            ushort crc = 0xffff;

            for (int i = 0; i < data.Length; i++)
            {
                crc = (ushort)((crc << 8) ^ Table[((crc >> 8) ^ data[i]) & 0xff]);
            }

            return crc;
        }

        /// <summary>
        /// Builds the lookup table for the CCITT polynomial.
        /// </summary>
        private static ushort[] CreateTable()
        {
            ushort[] table = new ushort[256];

            for (int i = 0; i < table.Length; i++)
            {
                ushort crc = 0;
                ushort c = (ushort)(i << 8);

                for (int j = 0; j < 8; j++)
                {
                    if (((crc ^ c) & 0x8000) != 0)
                    {
                        crc = (ushort)((crc << 1) ^ Polynomial);
                    }
                    else
                    {
                        crc = (ushort)(crc << 1);
                    }

                    c = (ushort)(c << 1);
                }

                table[i] = crc;
            }

            return table;
        }
    }
}
