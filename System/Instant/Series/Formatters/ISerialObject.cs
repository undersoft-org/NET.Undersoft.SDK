namespace System
{
    public interface ISerialObject
    {
        object Locate(object path = null);

        object Merge(object source, string name = null);
    }
}
