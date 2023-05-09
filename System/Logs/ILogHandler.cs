namespace System.Logs
{
    public interface ILogHandler
    {
        bool Clean(DateTime olderThen);

        void Write(Starlog log);
    }
}
