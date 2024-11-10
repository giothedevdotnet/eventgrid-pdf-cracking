using System;

namespace ChatBotFunc.SplitPdfFile;

internal class BlobEventData
{
    public string SpecVersion { get; set; }
    public string Type { get; set; }
    public string Source { get; set; }
    public string Id { get; set; }
    public DateTime Time { get; set; }
    public string Subject { get; set; }
    public Data Data { get; set; }
}

internal class Data
{
    public string Api { get; set; }
    public string ClientRequestId { get; set; }
    public string RequestId { get; set; }
    public string ETag { get; set; }
    public string ContentType { get; set; }
    public long ContentLength { get; set; }
    public string BlobType { get; set; }
    public string Url { get; set; }
    public string Sequencer { get; set; }
    public StorageDiagnostics StorageDiagnostics { get; set; }
}

internal class StorageDiagnostics
{
    public string BatchId { get; set; }
}