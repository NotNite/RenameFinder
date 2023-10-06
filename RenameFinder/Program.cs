using System.Security.Cryptography;
using Lumina;
using Lumina.Data;
using RenameFinder;

if (args.Length < 3) {
    Console.WriteLine(
        "Usage: RenameFinder.exe \"<path to game/sqpack folder>\" \"<path to input file>\" \"<path to output file>\"");
    return;
}

var gamePath = args[0];
var inputPath = args[1];
var outputPath = args[2];

Console.WriteLine("Initializing Lumina...");
var lumina = new GameData(gamePath);

Console.WriteLine("Initializing ResLogger...");
var hashes = ResLogger.GetHashes();

string RecoverPath(uint folder, uint file) {
    var folderName = $"~{folder:X8}";
    var fileName = $"~{file:X8}";

    foreach (var hash in hashes) {
        if (hash.FolderHash == folder) folderName = hash.FolderName;
        if (hash.FileHash == file) fileName = hash.FileName;
    }

    return $"{folderName}/{fileName}";
}

string Sha256(byte[] bytes) {
    var hash = SHA256.HashData(bytes);
    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
}

var lines = File.ReadAllText(inputPath).Trim().Split('\n');
var foundCount = 0;
var total = lines.Length;

foreach (var line in lines) {
    var chunks = line.Split(' ');
    var path = chunks[0];
    var onDiskPath = chunks[1];

    Console.WriteLine($"Searching for {path}...");

    var parsedPath = GameData.ParseFilePath(path)!;
    var onDiskFile = File.ReadAllBytes(onDiskPath);

    var categoryId = Repository.CategoryNameToIdMap[parsedPath.Category];
    var categories = lumina.Repositories[parsedPath.Repository].Categories[categoryId];

    var found = false;
    foreach (var category in categories) {
        if (found) break;

        foreach (var (hash, entry) in category.IndexHashTableEntries!) {
            if (found) break;

            var folderHash = (uint) (hash >> 32);
            var fileHash = (uint) hash;

            if (parsedPath.FolderHash == folderHash) {
                var file = category.GetFile<FileResource>(hash)!;
                var data = file.Data;

                if (data.Length == onDiskFile.Length) {
                    var oldHash = Sha256(onDiskFile);
                    var newHash = Sha256(data);

                    if (oldHash == newHash) {
                        Console.WriteLine($"{path} -> {RecoverPath(folderHash, fileHash)}");
                        File.AppendAllText(outputPath, $"{path} {RecoverPath(folderHash, fileHash)}\n");
                        found = true;
                    }
                }
            }
        }
    }

    if (found) {
        foundCount++;
    } else {
        Console.WriteLine($"Could not find {path}.");
    }
}

Console.WriteLine($"Found {foundCount}/{total} files.");
