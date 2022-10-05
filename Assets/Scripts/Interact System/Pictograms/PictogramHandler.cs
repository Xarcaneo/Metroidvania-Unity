using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictogramHandler : MonoBehaviour
{
    [SerializeField] private PictogramEnabler[] pictograms = null;
    [Min(0)][SerializeField] private int defaultPictogram = 0;

    private int currentPictogram;

    private void Awake()
    {
        currentPictogram = defaultPictogram;
    }

    public void ShowPictogram(int pictogramIndex)
    {
        pictograms[currentPictogram].enabled = false;
        currentPictogram = pictogramIndex;
        pictograms[currentPictogram].enabled = true;
    }

    public void HidePictogram() => pictograms[currentPictogram].enabled = false;
}
