using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

/// <summary>
/// This class is used to manage the "Message" gameObject. This use only two functions to change the message text and to close  the message panel.
/// </summary>
public class Message : MonoBehaviour {

    public LanguageManager lang;
    public Text _text;

    private void Awake() {
        lang = LanguageManager.Instance;
    }

    private void Update() {
        //For the close with "enter".
        if (gameObject.activeInHierarchy && (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) ) {
            CloseButton();
        }
    }

    /// <summary>
    /// This function only change the message with what you pass as parameter, without translating or having global keys.
    /// </summary>
    public void ChangeMessage(string message) {
        _text.text = message;
    }

    /// <summary>
    /// This function allow us to have a global index for a message for every language.
    /// When you call this function you have to pass the Index of the XML string, for example "PRESENTATION".
    /// </summary>
    public void ChangeMultiLanguageMessage(string code) {
        gameObject.SetActive(true);
        _text.text = lang.langReader.getString(code);
    }

    //If we press the "Close" button
    public void CloseButton() {
        //if the text is nothing a text that it's saying to wait due we're executing a query from the database
        //if(_text.text != lang.langReader.getString("MESSAGE_PROCESSING_REQUEST") && _text.text != lang.langReader.getString("MESSAGE_RETRIVING_DATA") &&  _text.text != lang.langReader.getString("CREATE_MESSAGE_CREATING") && _text.text != lang.langReader.getString("DELETE_MESSAGE_DELETING"))
        gameObject.SetActive(false); //disable the message gameObject.

    }

}
