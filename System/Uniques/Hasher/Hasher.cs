namespace System.Uniques
{
    public static class Hasher32
    {
        public static unsafe Byte[] ComputeBytes(byte* ptr, int length, ulong seed = 0)
        {
            byte[] b = new byte[4];
            fixed (byte* pb = b)
            {
                *((uint*)pb) = xxHash32.UnsafeComputeHash(ptr, length, (uint)seed);
            }
            return b;
        }

        public static unsafe Byte[] ComputeBytes(byte[] bytes, ulong seed = 0)
        {
            byte[] b = new byte[4];
            fixed (
                byte* pb = b,
                    pa = bytes
            )
            {
                *((uint*)pb) = xxHash32.UnsafeComputeHash(pa, bytes.Length, (uint)seed);
            }
            return b;
        }

        public static unsafe uint ComputeKey(byte* ptr, int length, ulong seed = 0)
        {
            return xxHash32.UnsafeComputeHash(ptr, length, (uint)seed);
        }

        public static unsafe uint ComputeKey(byte[] bytes, ulong seed = 0)
        {
            fixed (byte* pa = bytes)
            {
                return xxHash32.UnsafeComputeHash(pa, bytes.Length, (uint)seed);
            }
        }
    }

    public static class Hasher64
    {
        public static unsafe Byte[] ComputeBytes(byte* bytes, int length, ulong seed = 0)
        {
            byte[] b = new byte[8];
            fixed (byte* pb = b)
            {
                *((ulong*)pb) = (xxHash64.UnsafeComputeHash(bytes, length, seed) << 12) >> 12;
            }
            return b;
        }

        public static unsafe Byte[] ComputeBytes(byte[] bytes, ulong seed = 0)
        {
            byte[] b = new byte[8];
            fixed (
                byte* pb = b,
                    pa = bytes
            )
            {
                *((ulong*)pb) = (xxHash64.UnsafeComputeHash(pa, bytes.Length, seed) << 12) >> 12;
            }
            return b;
        }

        public static unsafe ulong ComputeKey(byte* ptr, int length, ulong seed = 0)
        {
            return (xxHash64.UnsafeComputeHash(ptr, length, seed) << 12) >> 12;
        }

        public static unsafe ulong ComputeKey(byte[] bytes, ulong seed = 0)
        {
            fixed (byte* pa = bytes)
            {
                return (xxHash64.UnsafeComputeHash(pa, bytes.Length, seed) << 12) >> 12;
            }
        }
    }
}
