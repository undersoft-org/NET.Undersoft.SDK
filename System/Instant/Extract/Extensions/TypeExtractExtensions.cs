namespace System.Extract
{
    public static class TypeExtractExtenstion
    {
        public unsafe static object NewStructure(this Type structure, byte* binary, long offset = 0)
        {
            return Extractor.PointerToStructure(binary, structure, offset);
        }

        public static object NewStructure(this Type structure, byte[] binary, long offset = 0)
        {
            return Extractor.BytesToStructure(binary, structure, offset);
        }
    }
}
