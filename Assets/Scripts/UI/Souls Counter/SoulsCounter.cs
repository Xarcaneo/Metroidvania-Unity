using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SoulsCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private float counterSpeed = 0.05f;
    private int currentSouls = 0;
    private Queue<int> soulsQueue = new Queue<int>();
    private bool isUpdating = false;

    private void OnEnable()
    {
        GameEvents.Instance.onSoulsReceived += OnSoulsReceived;
        GameEvents.Instance.onNewSession += OnNewSession;
    }

    private void OnDisable()
    {
        GameEvents.Instance.onSoulsReceived -= OnSoulsReceived;
        GameEvents.Instance.onNewSession -= OnNewSession;
    }
    private void OnNewSession()
    {
        StartCoroutine(UpdateData());
    }

    IEnumerator UpdateData()
    {
        yield return new WaitForEndOfFrame();

        soulsQueue.Clear();
        currentSouls = DialogueLua.GetVariable("Souls").asInt;
        counter.text = currentSouls.ToString();
    }

    private void OnSoulsReceived(int soulsAmount)
    {
        DialogueLua.SetVariable("Souls", currentSouls + soulsAmount);

        soulsQueue.Enqueue(soulsAmount);
        if (!isUpdating)
        {
            StartCoroutine(UpdateCounter());
        }
    }

    private IEnumerator UpdateCounter()
    {
        isUpdating = true;

        while (soulsQueue.Count > 0)
        {
            int targetSouls = currentSouls + soulsQueue.Dequeue();

            while (currentSouls < targetSouls)
            {
                currentSouls++;
                counter.text = currentSouls.ToString();
                yield return new WaitForSeconds(counterSpeed); // Adjust the delay to control the speed of increment
            }
        }

        isUpdating = false;
    }
}
