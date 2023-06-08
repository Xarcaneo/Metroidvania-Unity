using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.DropsAndPickups;
using PixelCrushers;
using PixelCrushers.DialogueSystem;
using QFSW.QC;
using UnityEngine;

public class ConsoleCommands : MonoBehaviour
{
    [Command("Give-item")]
    private static void GiveItem(string itemName, int ammount)
    {
        // Get the Item Object Spawner anywhere in your code.
        var itemObjectSpawner = InventorySystemManager.GetGlobal<ItemObjectSpawner>(1);

        ItemInfo itemInfo = new ItemInfo(itemName, ammount);

        // Spawn an Item using an item Info and a position.
        itemObjectSpawner.Spawn(itemInfo, Player.Instance.transform.position);
    }

    [Command("Kill-me")]
    private static void KillMe()
    {
        Player.Instance.StateMachine.ChangeState(Player.Instance.DeathState);
    }

    [Command("Teleport-Me")]
    private static void TeleportMe(Vector3 position)
    {
        Player.Instance.transform.position = position;
    }

    [Command("Save-Game")]
    private static void SaveGame()
    {
        Player.Instance.GetComponent<PlayerPositionSaver>().isCheckpoint = true;
        SaveSystem.SaveToSlot(GameManager.Instance.currentSaveSlot);
    }

    [Command("Load-Game")]
    private static void LoadGame()
    {
        SaveSystem.LoadFromSlot(GameManager.Instance.currentSaveSlot);
    }

    [Command("Load-Scene")]
    private static void LoadScene(string sceneName)
    {
        SaveSystem.LoadScene(sceneName);
    }

    [Command("Load-Conversation")]
    private static void LoadConversation(string conversationID)
    {
        DialogueManager.StartConversation(conversationID);
    }
}
