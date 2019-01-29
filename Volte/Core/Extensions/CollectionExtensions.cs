using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Volte.Core.Extensions {
    public static class CollectionExtensions {
        public static Stream ToStream(this IEnumerable<byte> bytes) {
            var memStream = new MemoryStream(bytes as byte[] ?? bytes.ToArray(), false);
            memStream.Seek(0, SeekOrigin.Begin);
            return memStream;
        }
    }
}