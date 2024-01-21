using SML;

namespace AutoPlayerNotes.Util;

public static class ModSettingsHelper
{
    public static bool TryGetBool(string name, bool defaultValue = false)
    {
        try
        {
            return ModSettings.GetBool(name);
        }
        catch
        {
            return defaultValue;
        }
    }

    public static string TryGetString(string name, string defaultValue = null)
    {
        try
        {
            return ModSettings.GetString(name);
        }
        catch
        {
            return defaultValue;
        }
    }
}