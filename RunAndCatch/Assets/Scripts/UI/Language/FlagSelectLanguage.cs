using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



/// <summary>
/// This class is used for the flags that allow us to change the language. Basically all we do is to call the "SelectLanguage" function from the LanguageManager when we click
/// the button (where this script is attached to)
/// and pass the parameter of the language with what we've wrote in the inspector
/// </summary>
public class FlagSelectLanguage : MonoBehaviour {

    private LanguageManager lang;
    [SerializeField] private string language;

    void Start() {
        lang = LanguageManager.Instance;
        //Create a listener for the function "SelectLanguage()" present in the LanguageManager.cs
        GetComponent<Button>().onClick.AddListener(() => { lang.SelectLanguage(language); });
    }
}
