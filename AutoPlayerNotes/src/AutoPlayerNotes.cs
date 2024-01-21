using AutoPlayerNotes.Util;
using JetBrains.Annotations;
using SML;

namespace AutoPlayerNotes;

[Mod.SalemMod]
[UsedImplicitly]
public class AutoPlayerNotes
{
    public static readonly PlayerFlags Flags = new();
    public static readonly RoleMarker Marker = new();
    public static readonly TimestampedLogSource Logger = new("AutoPlayerNotes");

    [UsedImplicitly]
    public static void Start()
    {
        BepInEx.Logging.Logger.Sources.Add(Logger);

        Logger.LogInfo("Balls retain pee");
    }
}