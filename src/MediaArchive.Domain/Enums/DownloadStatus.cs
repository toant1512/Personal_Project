namespace MediaArchive.Domain.Enums;

public enum DownloadStatus
{
    Pending = 0,
    Queued = 1,
    Downloading = 2,
    Completed = 3,
    Failed = 4
}