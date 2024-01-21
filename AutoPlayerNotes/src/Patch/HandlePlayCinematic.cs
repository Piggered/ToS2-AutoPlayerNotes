using Cinematics.Players;
using Game.Simulation;
using HarmonyLib;
using JetBrains.Annotations;
using Server.Shared.Cinematics.Data;
using Server.Shared.Messages;
using Server.Shared.State;
using Services;
using Mod = AutoPlayerNotes.AutoPlayerNotes;

namespace AutoPlayerNotes.Patch;

[HarmonyPatch(typeof(GameSimulation), "HandlePlayCinematic")]
public class HandlePlayCinematic
{
    [UsedImplicitly]
    public static void Postfix(PlayCinematicMessage message)
    {
        var entry = message.cinematicEntry;

        Mod.Logger.LogInfo($"Cinematic {entry.type} is playing", "HandlePlayCinematic");
        Service.Game.Cinematic.OnCinematicWillEnd += HandleCinematicEnd;

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (entry.type)
        {
            case CinematicType.DeputyKill:
            {
                var data = entry.GetData() as DeputyKillCinematicData;

                if (Pepper.GetMyPosition() != data!.deputyPosition)
                {
                    Mod.Marker.Mark(data.deputyPosition, Role.DEPUTY, Setting.AutoNoteTownReveal);
                }

                break;
            }

            case CinematicType.MayorReveal:
            {
                var data = entry.GetData() as MayorRevealCinematicData;

                if (Pepper.GetMyPosition() != data!.playerPosition)
                {
                    Mod.Marker.Mark(data.playerPosition, Role.MAYOR, Setting.AutoNoteTownReveal);
                }

                break;
            }

            case CinematicType.MarshalTribunal:
            {
                var data = entry.GetData() as MarshalTribunalCinematicData;

                if (Pepper.GetMyPosition() != data!.playerPosition)
                {
                    Mod.Marker.Mark(data.playerPosition, Role.MARSHAL, Setting.AutoNoteTownReveal);
                }

                break;
            }

            default:
                return;
        }
    }

    private static void HandleCinematicEnd(CinematicPlayer _)
    {
        Mod.Logger.LogInfo("Cinematic has ended", "HandlePlayCinematic");
        Mod.Marker.ProcessMarked();

        Service.Game.Cinematic.OnCinematicWillEnd -= HandleCinematicEnd;
    }
}