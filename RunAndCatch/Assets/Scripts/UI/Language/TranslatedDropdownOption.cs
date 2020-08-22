using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// This class will allow us to have a "DropDown" totally translated.
/// For the dropdown was not possible to have a class like "TranslatedText.cs", because it works with Options and not present textes.
/// This class allow us to have a dropdown with any lenght you want
/// </summary>
public class TranslatedDropdownOption : MonoBehaviour {

    public LanguageManager lang;

    //This list will contain all the Indexs of the string that we need to be in the dropdown element, THIS is the list that you have to fulfill.
    public List<string> OptionsKeys;

    //This list will contain the Translated Textes of the keys that we've inserted in the editor
    public List<string> TranslatedKeys;

    private Dropdown thisDrop; //then we get a reference to the dropdown element

    // Use this for initialization
    void Start()
    {

        lang = LanguageManager.Instance;
        LanguageManager.OnLanguageChange += OnLanguageChange;
        thisDrop = GetComponent<Dropdown>();
        //Like always, at the start we update the dropdown
        if (lang != null && lang.langReader != null)
                UpdateDropDown();
    }

    void OnLanguageChange() {
        //We check if we should update the dropdown
        if (lang != null && lang.langReader != null)
           UpdateDropDown();

    }

    /// <summary>
    /// This function will update the dropdown by adding the result of the keys that we've inserted in the "TranslatedKeys" list
    /// </summary>
    void UpdateDropDown()
    {
        TranslatedKeys.Clear(); //Firstly we remove every translated text

        //Secondly we translate everything we've inserted and put them in the TranslatedKeys list
        for (int i = 0; i < OptionsKeys.Count; i++)
        {
            TranslatedKeys.Add(lang.langReader.getString(OptionsKeys[i]));
        }

        thisDrop.ClearOptions(); //we remove all the previous options
        thisDrop.AddOptions(TranslatedKeys); //we add the new options
    }

    private void OnDestroy() {
        LanguageManager.OnLanguageChange -= OnLanguageChange;
    }
}
