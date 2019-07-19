using System.IO;

namespace Volte.Data.Models.Results
{
    public struct FileAttachment
    {
        private FileAttachment(Stream stream, string filename)
        {
            Stream = stream;
            Filename = filename;
        }

        public Stream Stream { get; }
        public string Filename { get; }

        public static FileAttachment FromStream(Stream stream, string filename)
        {
            return new FileAttachment(stream, filename);
        }
    }
}