using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// This class allow us to modify the text foreach languages by having a string "param" that automatically will take our text and translate it to the language that we've selected.
/// </summary>
public class TranslatedText : MonoBehaviour {

    private LanguageManager lang;
    private Text myText;

    [Tooltip("The string name (Index) in the XML field.")]
    [SerializeField] private string param;
    
	void Start () {
        lang = LanguageManager.Instance;
        LanguageManager.OnLanguageChange += OnLanguageChange;
        myText = GetComponent<Text>();
        //Every time that we 
        if (lang != null && lang.langReader != null) {
            UpdateText();
        }
    }
    
    void OnLanguageChange () {
        //Here we check if the language has changed, if so, update the text again
        if (lang != null && lang.langReader != null) {
            UpdateText();
        }

	}

    /// <summary>
    /// This function will update our 2D text or 3D text depending on what we have.
    /// We change the message thanks to the "getString" function of LanguageReader.cs
    /// </summary>
    void UpdateText() {
        if (myText != null) {
            myText.text = lang.langReader.getString(param);
        } else if (GetComponent<TextMesh>() != null)
            GetComponent<TextMesh>().text = lang.langReader.getString(param);
    }

    private void OnDestroy() {
        LanguageManager.OnLanguageChange -= OnLanguageChange;
    }
}
