namespace System.Instant.Stock
{
    using System;

    public interface IStockContext : ISerialBuffer, IDisposable
    {
        #region Properties

        long BufferSize { get; set; }

        int ClientCount { get; set; }

        int Elements { get; set; }

        string File { get; set; }

        long FreeSize { get; set; }

        long ItemCapacity { get; set; }

        int ItemCount { get; set; }

        int ItemSize { get; set; }

        int NodeCount { get; set; }

        string Path { get; set; }

        ushort SectorId { get; set; }

        int ServerCount { get; set; }

        ushort StockId { get; set; }

        long UsedSize { get; set; }

        #endregion
    }
}