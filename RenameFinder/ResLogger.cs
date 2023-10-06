using System.IO.Compression;

namespace RenameFinder;

public static class ResLogger {
    public static List<String> GetPaths(bool currentOnly = true) {
        var client = new HttpClient();
        var fileName = currentOnly ? "CurrentPathList" : "PathList";
        var url = $"https://rl2.perchbird.dev/download/export/{fileName}.gz";

        var stream = new GZipStream(client.GetStreamAsync(url).Result, CompressionMode.Decompress);
        var pathList = new StreamReader(stream).ReadToEnd();
        var lines = pathList.Trim().Split("\n");

        return lines.ToList();
    }

    public static List<Hash> GetHashes(bool currentOnly = true) {
        var client = new HttpClient();
        var fileName = currentOnly ? "CurrentPathListWithHashes" : "PathListWithHashes";
        var url = $"https://rl2.perchbird.dev/download/export/{fileName}.gz";

        using var stream = new GZipStream(client.GetStreamAsync(url).Result, CompressionMode.Decompress);
        using var reader = new StreamReader(stream);
        var pathList = reader.ReadToEnd();
        var lines = pathList.Trim().Split("\n");

        var result = new List<Hash>();

        foreach (var line in lines.Skip(1)) {
            var hash = Hash.Parse(line);
            result.Add(hash);
        }

        
        
        return result;
    }

    public class Hash {
        public uint IndexID;
        public uint FolderHash;
        public uint FileHash;
        public ulong FullHash;

        public string FullPath;
        public string FolderName;
        public string FileName;

        private Hash(uint indexId, uint folderHash, uint fileHash, ulong fullHash, string fullPath) {
            IndexID = indexId;
            FolderHash = folderHash;
            FileHash = fileHash;
            FullHash = fullHash;

            FullPath = fullPath;

            var split = fullPath.Split("/");
            FolderName = string.Join("/", split.Take(split.Length - 1));
            FileName = split.Last();
        }

        public static Hash Parse(string line) {
            var parts = line.Split(",");
            var indexId = uint.Parse(parts[0]);
            var folderHash = uint.Parse(parts[1]);
            var fileHash = uint.Parse(parts[2]);
            var fullHash = ulong.Parse(parts[3]);
            var fullPath = parts[4];

            return new Hash(indexId, folderHash, fileHash, fullHash, fullPath);
        }
    }
}
