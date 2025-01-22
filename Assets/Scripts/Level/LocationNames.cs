using UnityEngine;
using System.Text.RegularExpressions;

public enum LocationName
{
    None,
    Vestibule,
    ArcaneSanctum,
    ChamberOfEchoes
}

public static class LocationNameExtensions
{
    public static string ToDisplayName(this LocationName location)
    {
        if (location == LocationName.None) return "";
        
        // Add a space before capital letters and trim any extra spaces
        string displayName = Regex.Replace(location.ToString(), "([A-Z])", " $1").Trim();
        return displayName;
    }
}
