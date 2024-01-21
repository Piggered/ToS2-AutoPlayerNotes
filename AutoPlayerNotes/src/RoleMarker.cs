using System.Collections.Generic;
using AutoPlayerNotes.Util;
using BMG.UI;
using Game.Interface;
using Server.Shared.State;
using Services;
using SML;
using UnityEngine;
using Mod = AutoPlayerNotes.AutoPlayerNotes;

// ReSharper disable Unity.PerformanceCriticalCodeInvocation

namespace AutoPlayerNotes;

public class RoleMarker
{
    private const string NotepadMainPanelObj = "Hud/NotepadElementsUI(Clone)/MainPanel";
    private const string NotepadBgObj = $"{NotepadMainPanelObj}/NotepadCommonElements/Background/ScaledBackground";
    private const string NotepadPlayerListObj = $"{NotepadBgObj}/PlayerNoteBackground/Scroll View/Viewport/Content";
    private const string LastWillCommonObj = "Hud/LastWillElementsUI(Clone)/MainPanel/LastWillCommonElements";

    private readonly Queue<(int, Role, Setting.AutoNote)> _rolesToMark = new();

    public void Mark(int position, Role role, Setting.AutoNote setting)
    {
        Mod.Logger.LogInfo($"Mark player {position} as {role}", "RoleMarker");

        _rolesToMark.Enqueue((position, role, setting));
    }

    public void ProcessMarked()
    {
        Mod.Logger.LogInfo("Process queued marked players", "RoleMarker");

        while (_rolesToMark.Count != 0)
        {
            var (position, role, setting) = _rolesToMark.Dequeue();

            Mod.Logger.LogInfo($"Process player {position}", "RoleMarker");

            if (!ModSettings.GetBool(setting.Enabled))
            {
                Mod.Logger.LogInfo($"Player {position} fails mark condition, skipping...", "RoleMarker");
                continue;
            }

            var playerInput = GetPlayerNoteInput(position);
            var decodedString = GetDecodedString(setting.Text, position, role);
            var revealMode = ModSettingsHelper.TryGetString(setting.Mode);

            if (revealMode == null || revealMode == "Overwrite" || string.IsNullOrWhiteSpace(playerInput.text))
            {
                playerInput.SetText(decodedString);
            }
            else
                switch (revealMode)
                {
                    case "Prepend":
                        playerInput.SetText($"{decodedString} {playerInput.text}");
                        break;

                    case "Append":
                        playerInput.SetText($"{playerInput.text} {decodedString}");
                        break;
                }
        }
    }

    public void Reset()
    {
        _rolesToMark.Clear();
    }

    private static string GetDecodedString(string settingName, int position, Role role)
    {
        var mentionsProvider = GameObject.Find(LastWillCommonObj)
            .GetComponentInChildren<LastWillCommonElementsPanel>()
            .mentionsPanel
            .mentionsProvider;

        var rawString = ModSettings.GetString(settingName)
            .Replace("%player.name%", Service.Game.Sim.simulation.GetDisplayName(position))
            .Replace("%player.position%", (position + 1).ToString())
            .Replace("%player.mention%", $"[[@{position + 1}]]")
            .Replace("%role.name%", SharedRoleData.GetRoleData(role).roleName)
            .Replace("%role.mention%", $"[[#{(byte)role}]]");

        return mentionsProvider.DecodeText(rawString);
    }

    private static BMG_InputField GetPlayerNoteInput(int position)
    {
        return GameObject.Find(NotepadPlayerListObj).transform.GetChild(position + 1).gameObject
            .GetComponentInChildren<BMG_InputField>();
    }
}