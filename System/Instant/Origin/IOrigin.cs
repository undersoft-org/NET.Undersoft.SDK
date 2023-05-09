namespace System.Instant
{
    public interface IOrigin
    {
        int OriginKey { get; set; }
        string OriginName { get; set; }
        DateTime Created { get; set; }
        string Creator { get; set; }
        DateTime Modified { get; set; }
        string Modifier { get; set; }
    }
}
