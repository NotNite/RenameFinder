using System.Reflection;
using System.Runtime.InteropServices;
using ImGuiNET;
using Lumina;
using VfxEditor;
using VfxEditor.AvfxFormat;

if (args.Length < 2) {
    Console.WriteLine(
        "Usage: RenameFinder.exe \"<path to game/sqpack folder>\" \"<game path of avfx file>\"");
    return;
}

var gamePath = args[0];
var avfxPath = args[1];

Console.WriteLine("Initializing Lumina...");
var lumina = new GameData(gamePath);

Console.WriteLine("Doing arcane hacks...");

// need to load cimgui before we do arcane hacks
var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)!;
var cimguiPath = Path.Combine(appdata, "XIVLauncher", "addon", "Hooks", "dev", "runtimes", "win-x64", "native",
                              "cimgui.dll");

NativeInterop.LoadLibrary(cimguiPath);
ImGui.CreateContext();

// fucking `private set` dude cmon
var plugin = typeof(Plugin);

var config = new Configuration();
plugin
    .GetProperty("Configuration", BindingFlags.Static | BindingFlags.Public)!
    .SetValue(null, config);

var manager = new AvfxManager();
plugin
    .GetProperty("AvfxManager", BindingFlags.Static | BindingFlags.Public)!
    .SetValue(null, manager);

plugin
    .GetProperty("RootLocation", BindingFlags.Static | BindingFlags.Public)!
    .SetValue(null, Directory.GetCurrentDirectory());

Console.WriteLine("Reading file...");
var file = lumina.GetFile(avfxPath)!;
var vfx = new AvfxFile(file.Reader, false);

foreach (var a in vfx.Main.Textures) {
    var path = a.Path.Value;
    Console.WriteLine(path);
}

public static class NativeInterop {
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
    public static extern nint LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);
}
