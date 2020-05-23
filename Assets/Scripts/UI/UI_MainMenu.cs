﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour, UI_Interface
{
    [Header("Cover of game")]
    private GameObject startScreen;

    private int levelToLoad = 1;
    private UIManager _uIManager;
    [SerializeField] private GameObject _firstSelected;

    [SerializeField] private SFXGroup _SFXGroup;

    private void Awake()
    {
        //_uIManager = transform.parent.GetComponent<UIManager>();
        if (_SFXGroup == null)
        {
            _SFXGroup = GetComponentInChildren<SFXGroup>();
        }
    }
    private void OnEnable()
    {
        if (_uIManager == null)
        {
            _uIManager = ServiceLocator.Get<UIManager>();
        }
        if (_uIManager != null)
        {
            _uIManager.SetSelected(_firstSelected);
        }

        //startScreen = transform.Find("cover").gameObject;
    }

    private void OnDisable()
    {
        
    }

    public void ResetUI()
    {
         {
            //startScreen.gameObject.SetActive(true);
        }
    }


    public void OnLevelButtonClick(int level)
    {
        levelToLoad = level;
        //ServiceLocator.Get<GameManager>().SwitchScene(GameManager.ESceneIndex.Level1);
        StartCoroutine(LoadLevelRoutine());
    }



    public void Button_StartGame()
    {
        _SFXGroup.PlaySFX("Click");

        //Bhavil's addition Friday May 15-16
        GameEvents.current.OnDoTransitionAction(TransitionManager.TransitionType.LogoWipe, GameManager.ESceneIndex.Level1);

        //ServiceLocator.Get<GameManager>().SwitchScene(GameManager.ESceneIndex.Level1);
        //StartCoroutine(LoadLevelRoutine());
    }

    //no start screen anymore
    public void Touch_StartScreen()
    {
        if (startScreen != null)
        {

            startScreen.gameObject.SetActive(false);
            //        if (_uIManager != null)
            {
                _uIManager.SetSelected(transform.Find("start").gameObject);
            }
        }

    }



    private IEnumerator LoadLevelRoutine()
    {
        yield return SceneManager.LoadSceneAsync(levelToLoad);
    }
}
