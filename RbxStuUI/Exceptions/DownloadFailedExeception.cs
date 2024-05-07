using System.Runtime.Serialization;

namespace RbxStuUI.Exceptions;
[Serializable]
internal class DownloadFailedExeception : Exception {
    public DownloadFailedExeception() {
    }

    public DownloadFailedExeception(string? message) : base(message) {
    }

    public DownloadFailedExeception(string? message, Exception? innerException) : base(message, innerException) {
    }

    protected DownloadFailedExeception(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
}