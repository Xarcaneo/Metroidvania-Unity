using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<bool> OnPaused;

    public int currentSaveSlot;

    private bool _isPaused;

    public bool isPaused
    {
        get
        {
            return _isPaused;
        }
        set
        {
            _isPaused = value;
            OnPaused?.Invoke(_isPaused);
        }
    }

    #region Instance variables
    private static GameManager _instance;
    public static GameManager Instance { get => _instance; }
    #endregion

    #region Initialize functions
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
    #endregion
}
