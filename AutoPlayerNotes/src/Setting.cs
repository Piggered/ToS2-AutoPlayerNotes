using System.Collections.Generic;
using JetBrains.Annotations;
using SML;
using AutoPlayerNotes.Util;
using TextInput = SML.ModSettings.TextInputSetting;
using DropdownInput = SML.ModSettings.DropdownSetting;

// ReSharper disable MemberCanBePrivate.Global

namespace AutoPlayerNotes;

[DynamicSettings]
[UsedImplicitly]
public class Setting
{
    public class AutoNote
    {
        public string Enabled { get; }
        public string Text { get; }
        public string Mode { get; }

        public AutoNote(string enabled, string text, string mode)
        {
            Enabled = enabled;
            Text = text;
            Mode = mode;
        }
    }

    private const string AvailablePlaceholders =
        "\n\nAvailable placeholders: %player.name%, %player.position%, %player.mention%, %role.name%, %role.mention%";

    public const string AutoTogglePlayerNotes = "Auto Toggle Player Notes";
    public const string AutoHideDeadPlayers = "Auto Hide Dead Players";
    public const string FactionHeaders = "Faction Headers";

    public static readonly AutoNote AutoNoteSelf = new("Auto Note Self", "Self Text", null);
    public static readonly AutoNote AutoNoteTeam = new("Auto Note Team", "Team Text", null);

    public static readonly AutoNote AutoNoteTownReveal = new(
        "Auto Note Town Reveals", "Town Reveal Text", "Town Reveal Mode");

    public static readonly AutoNote AutoNotePotionReveal = new(
        "Auto Note Potion Reveals", "Potion Reveal Text", "Potion Reveal Mode");

    public static readonly AutoNote AutoNoteWitchControl = new(
        "Auto Note Witch Controls", "Witch Control Text", "Witch Control Mode");

    public static readonly AutoNote AutoNoteRitualistBackfire = new(
        "Auto Note Ritualist Backfires", "Ritualist Backfire Text", "Ritualist Backfire Mode");

    private static TextInput CreateTextSetting
        (AutoNote setting, string description, string defaultValue) => new()
    {
        Name = setting.Text,
        Description = description + AvailablePlaceholders,
        DefaultValue = defaultValue,
        Regex = ".*",
        Available = ModSettingsHelper.TryGetBool(setting.Enabled),
        AvailableInGame = true
    };

    private static DropdownInput CreateModeSetting(AutoNote setting, string description) => new()
    {
        Name = setting.Mode,
        Description = description,
        Options = new List<string>
        {
            "Prepend",
            "Append",
            "Overwrite"
        },
        Available = ModSettingsHelper.TryGetBool(setting.Enabled),
        AvailableInGame = true
    };

    [UsedImplicitly]
    public TextInput SettingSelfText => CreateTextSetting(
        AutoNoteSelf,
        "What to write down yourself as in the Player Notes",
        "[Self]");

    [UsedImplicitly]
    public TextInput SettingTeamText => CreateTextSetting(
        AutoNoteTeam,
        "What to write down your teammates as in the Player Notes",
        "[Team]");

    [UsedImplicitly]
    public TextInput SettingTownReveal => CreateTextSetting(
        AutoNoteTownReveal,
        "What to write down revealed townies as in the Player Notes",
        "%role.mention% *");

    [UsedImplicitly]
    public TextInput SettingPotionReveal => CreateTextSetting(
        AutoNotePotionReveal,
        "What to write down roles revealed by Potion Master as in the Player Notes",
        "%role.mention% ~");

    [UsedImplicitly]
    public TextInput SettingWitchControl => CreateTextSetting(
        AutoNoteWitchControl,
        "What to write down roles controlled by Witch as in the Player Notes",
        "%role.mention% ~");

    [UsedImplicitly]
    public TextInput SettingRitualistBackfire => CreateTextSetting(
        AutoNoteRitualistBackfire,
        "What to write down the Ritualist that backfired as in the Player Notes",
        "%role.mention% !");

    [UsedImplicitly]
    public DropdownInput SettingTownRevealMode => CreateModeSetting(
        AutoNoteTownReveal, "How should the Town Reveal text be inserted in the Player Note?");

    [UsedImplicitly]
    public DropdownInput SettingPotionRevealMode => CreateModeSetting(
        AutoNotePotionReveal, "How should the Potion Reveal text be inserted in the Player Note?");

    [UsedImplicitly]
    public DropdownInput SettingWitchControlMode => CreateModeSetting(
        AutoNoteWitchControl, "How should the Witch Control text be inserted in the Player Note?");

    [UsedImplicitly]
    public DropdownInput SettingRitualistBackfireMode => CreateModeSetting(
        AutoNoteRitualistBackfire, "How should the Ritualist Backfire text be inserted in the Player Note?");
}