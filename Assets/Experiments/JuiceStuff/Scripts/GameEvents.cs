﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;
    private void Awake()
    {
        current = this;
    }
    
    #region Action Declareations
    public event Action<Color> DoFlashColor;
    public event Action DoShakeCamera;
    public event Action<Transform> EnemyDied;
    public event Action<TransitionManager.TransitionType, GameManager.ESceneIndex> DoTransition;
    public event Action PlayerDied;
    #endregion

    #region Action Functions
    public void OnDoFlashColorAction(Color color)
    {
        DoFlashColor?.Invoke(color);
    }
    public void OnDoShakeCameraAction()
    {
        DoShakeCamera?.Invoke();
    }
    public void OnEnemyDiedAction(Transform enemyPos)
    {
        EnemyDied?.Invoke(enemyPos);
    }

    public void OnDoTransitionAction(TransitionManager.TransitionType type, GameManager.ESceneIndex buildIndex)
    {
        DoTransition?.Invoke(type, buildIndex);
    }

    public void OnPlayerDeath()
    {
        PlayerDied?.Invoke();
    }

    #endregion
}
