using UnityEngine;

/// <summary>
/// Utility class for converting input device button/key names to shorter, more user-friendly versions.
/// </summary>
public static class InputNameUtility
{
    /// <summary>
    /// Converts gamepad button names to shorter versions
    /// </summary>
    public static string GetShortGamepadName(string fullName)
    {
        switch (fullName.ToLower())
        {
            case "left shoulder":
                return "LB";
            case "right shoulder":
                return "RB";
            case "left trigger":
                return "LT";
            case "right trigger":
                return "RT";
            case "dpad up":
                return "D-Up";
            case "dpad down":
                return "D-Down";
            case "dpad left":
                return "D-Left";
            case "dpad right":
                return "D-Right";
            case "button north":
                return "Y";
            case "button south":
                return "A";
            case "button east":
                return "B";
            case "button west":
                return "X";
            case "start":
                return "Start";
            case "select":
                return "Select";
            default:
                return fullName;
        }
    }
    
    /// <summary>
    /// Converts keyboard key names to shorter versions
    /// </summary>
    public static string GetShortKeyboardName(string fullName)
    {
        switch (fullName.ToLower())
        {
            case "escape":
                return "Esc";
            case "space":
                return "Space";
            case "left arrow":
                return "←";
            case "right arrow":
                return "→";
            case "up arrow":
                return "↑";
            case "down arrow":
                return "↓";
            case "left shift":
                return "L-Shift";
            case "right shift":
                return "R-Shift";
            case "left ctrl":
                return "L-Ctrl";
            case "right ctrl":
                return "R-Ctrl";
            case "left alt":
                return "L-Alt";
            case "right alt":
                return "R-Alt";
            case "left mouse button":
                return "LMB";
            case "right mouse button":
                return "RMB";
            case "middle mouse button":
                return "MMB";
            case "tab":
                return "Tab";
            case "enter":
                return "Enter";
            case "backspace":
                return "Bksp";
            case "delete":
                return "Del";
            case "insert":
                return "Ins";
            case "home":
                return "Home";
            case "end":
                return "End";
            case "page up":
                return "PgUp";
            case "page down":
                return "PgDn";
            default:
                return fullName;
        }
    }
}
