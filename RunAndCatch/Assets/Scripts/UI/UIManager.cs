﻿using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // Screens to be created and loaded
    public UIScreen[] screenPrefs;

    // The standard screen that opens first
    public int defaultScreenPrefId = -1;

    // Loaded screens
    protected List<UIScreen> screensPool;

    // Current open screen
    private UIScreen currentScreen;

    protected virtual void Awake()
    {
        CreateSingleton();
        Initialize();
    }

    protected void CreateSingleton()
    {
        Instance = this;
    }

    protected virtual void Initialize()
    {
        // TODO: Switch language depend settings
        string preferLang = LanguageManager.Instance.currentLanguage;
        //LanguageManager manager = LanguageManager.Instance;
        //manager.SelectLanguage(preferLang);

        screensPool = new List<UIScreen>();
        foreach (UIScreen screenPref in screenPrefs)
        {
            UIScreen createdScreen = CreateScreen(screenPref);
            createdScreen.Initialize();
            createdScreen.Hide();
            screensPool.Add(createdScreen);
        }

        if (defaultScreenPrefId > -1)
        {
            UIScreen defScreen = screensPool[defaultScreenPrefId];
            defScreen.Show();
            currentScreen = defScreen;
        }

    }

    protected UIScreen CreateScreen(UIScreen pref)
    {
        return Instantiate(pref, transform);
    }

    public UIScreen GetScreenById(string id)
    {
        foreach (UIScreen screen in screensPool)
        {
            if (screen.mId == id)
                return screen;
        }
        throw new System.Exception("There is no screen id: " + id + ", size: " + screensPool.Capacity);
    }

    public void OpenGUI(string id)
    {
        Debug.Log("OpenGUI");
        if (currentScreen != null)
        {
            currentScreen.Hide();
        }

        UIScreen screen = GetScreenById(id);
        if(screen != null)
        {
            currentScreen = screen;
            currentScreen.Show();
        }
    }

    public UIScreen GetCurrentScreen()
    {
        return currentScreen;
    }

    public void CloseGUI()
    {
        Debug.Log("CloseGUI");
        if (currentScreen != null)
        {
            currentScreen.Hide();
        }
        currentScreen = null;
    }
}
