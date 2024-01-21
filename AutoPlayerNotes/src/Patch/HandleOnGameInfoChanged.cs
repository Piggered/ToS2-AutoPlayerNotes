using System.Collections;
using System.Linq;
using System.Text;
using BMG.UI;
using Game.Interface;
using Game.Simulation;
using HarmonyLib;
using JetBrains.Annotations;
using Server.Shared.Info;
using Server.Shared.State;
using Services;
using SML;
using UnityEngine;
using Mod = AutoPlayerNotes.AutoPlayerNotes;

// ReSharper disable Unity.PerformanceCriticalCodeInvocation

namespace AutoPlayerNotes.Patch;

[HarmonyPatch(typeof(GameSimulation), "HandleOnGameInfoChanged")]
public class HandleOnGameInfoChanged
{
    private const string NotepadMainPanelObj = "Hud/NotepadElementsUI(Clone)/MainPanel";
    private const string NotepadBgObj = $"{NotepadMainPanelObj}/NotepadCommonElements/Background/ScaledBackground";
    private const string NotepadInputObj = $"{NotepadBgObj}/PaperBackground/TextPanel/BmgInputField";

    private static readonly int State = Animator.StringToHash("State");

    [UsedImplicitly]
    public static void Postfix(GameInfo gameInfo)
    {
        if (gameInfo.gamePhase != GamePhase.PLAY)
        {
            Mod.Marker.Reset();
            Mod.Flags.Reset();

            Mod.Logger.LogInfo($"Reset mod state (GamePhase = {gameInfo.gamePhase})", "HandleOnGameInfoChanged");

            return;
        }

        Mod.Logger.LogInfo($"PlayPhase = {gameInfo.playPhase}", "HandleOnGameInfoChanged");

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (gameInfo.playPhase)
        {
            case PlayPhase.FIRST_DAY:
            {
                var notepadPanel = GameObject.Find(NotepadMainPanelObj).GetComponentInChildren<NotepadPanel>();

                Mod.Logger.LogInfo("Prepare notepad", "HandleOnGameInfoChanged");
                notepadPanel.StartCoroutine(PrepareNotepad(notepadPanel));

                break;
            }

            case PlayPhase.TRIAL:
            {
                var trial = Pepper.GetCurrentTrialData();

                if (!trial.IsProsecution())
                {
                    goto case PlayPhase.WHO_DIED_AND_HOW;
                }

                Mod.Logger.LogInfo($"Trial is prosecution, finding {Role.PROSECUTOR}...", "HandleOnGameInfoChanged");

                var position = trial.prosecutorPosition;

                if (trial.prosecutorPosition != Pepper.GetMyPosition() &&
                    !Mod.Flags.Has(position, Flag.HasProsecuted))
                {
                    Mod.Marker.Mark(position, Role.PROSECUTOR, Setting.AutoNoteTownReveal);
                    Mod.Flags.Add(position, Flag.HasProsecuted);
                }

                goto case PlayPhase.WHO_DIED_AND_HOW;
            }

            case PlayPhase.DISCUSSION:
            case PlayPhase.VOTING:
            case PlayPhase.VOTING_RESULTS:
            case PlayPhase.WHO_DIED_AND_HOW:
            {
                Mod.Marker.ProcessMarked();

                break;
            }
        }
    }

    private static IEnumerator PrepareNotepad(NotepadPanel notepadPanel)
    {
        yield return null;
        yield return null;
        yield return null;

        // -- toggle player notes --
        if (ModSettings.GetBool(Setting.AutoTogglePlayerNotes))
        {
            notepadPanel.playerListAnimator.SetInteger(State, 1);
        }

        if (ModSettings.GetBool(Setting.AutoHideDeadPlayers))
        {
            notepadPanel.HideDeadPlayersToggle.isOn = true;
        }

        yield return null;

        // -- add self and team tags --
        var selfPosition = Pepper.GetMyPosition();
        var selfRole = Pepper.GetMyRole();
        var selfFaction = Pepper.GetMyFaction();

        var kvpTeam = Service.Game.Sim.simulation.knownRolesAndFactions.Get()
            .Where(kvp => kvp.Key != selfPosition && kvp.Value.Item2 == selfFaction);

        Mod.Marker.Mark(selfPosition, selfRole, Setting.AutoNoteSelf);

        yield return null;

        foreach (var kvp in kvpTeam)
        {
            Mod.Marker.Mark(kvp.Key, kvp.Value.Item1, Setting.AutoNoteTeam);

            yield return null;
        }

        Mod.Marker.ProcessMarked();

        if (ModSettings.GetBool(Setting.FactionHeaders))
        {
            AddFactionHeaders();
        }
    }

    private static void AddFactionHeaders()
    {
        var notepadInput = GameObject.Find(NotepadInputObj).GetComponentInChildren<BMG_InputField>();
        var notepadContent = new StringBuilder();

        notepadContent.AppendLine("[Town]");
        notepadContent.Append("\n\n");

        if (!Pepper.AmICoven())
        {
            notepadContent.AppendLine("[Coven]");
            notepadContent.Append("\n\n");
        }

        if (Pepper.GetMyFaction() != FactionType.APOCALYPSE)
        {
            notepadContent.AppendLine("[Apocalypse]");
            notepadContent.Append("\n\n");
        }

        notepadContent.AppendLine("[Neutral]");

        notepadInput.SetText(notepadContent.ToString());
    }
}