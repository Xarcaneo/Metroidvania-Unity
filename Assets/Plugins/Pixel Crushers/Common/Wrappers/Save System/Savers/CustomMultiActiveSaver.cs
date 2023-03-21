using PixelCrushers;

public class CustomMultiActiveSaver : MultiActiveSaver
{
    public override void ApplyData(string s)
    {
        if (!CustomMultiActiveSaverManager.IsLoadingSavedGame)
        {
            base.ApplyData(s);
        }
    }
}
