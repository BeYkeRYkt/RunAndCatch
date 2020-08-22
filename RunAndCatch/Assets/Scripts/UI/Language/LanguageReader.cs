using System;
using System.Collections;
using System.Xml;
using UnityEngine;

/// <summary>
/// This class is the core of MLS, it allow us to read the XML file (using System.Xml) and to transfer all it contains in a hashtable.
/// We can get all this datas thanks to the "getLine()" function, that reads the hashtable with the index that we give as parameter and return us 
/// the string content
/// 
/// For more information on the hashtables read: https://en.wikipedia.org/wiki/Hash_table
/// </summary>
public class LanguageReader {
    
    Hashtable XML_Strings; //The hashtable that we create to contain the data

    /// <summary> This constructor is called when we instantiate this class in the "LanguageManager" class.
    ///It's called when we set a new language to read it thanks to the functions "SetLanguageWeb" and "SetLocalLanguage".
    ///
    ///The parameters are: the XML file is gived as WWW.text result for the Web and as the physical file for the local.
    ///The "language" is the language that we've selected
    ///The "isLocal" define what function to call, if the one to open a web file or the one to open a file stored on the computer
    /// </summary>
    public LanguageReader(TextAsset xmlFile, string language) {
        SetLocalLanguage(xmlFile.text, language);
    }

    ///Read a XML stored on the computer
    public void SetLocalLanguage(string xmlContent, string language) {
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(xmlContent);
        XML_Strings = new Hashtable();
        XmlElement element = xml.DocumentElement;
        if (element != null) {
            var elemEnum = element.GetEnumerator();

            while (elemEnum.MoveNext()) {
                var current = elemEnum.Current as XmlElement;
                if (current == null) continue;
                if (current.NodeType != XmlNodeType.Comment)
                {
                    XML_Strings.Add(current.GetAttribute("name"), current.InnerText.Replace(@"\n", Environment.NewLine));
                }
                //XML_Strings.Add((elemEnum.Current as XmlElement).GetAttribute("name"), (elemEnum.Current as XmlElement).InnerText.Replace(@"\n", Environment.NewLine));
            }
        } else { 
            Debug.LogError("The specified language does not exist: " + language);
        }
    }

    /// Get a string from the hastable by the index gived in it.
    public string getString(string _name) {
        if (!XML_Strings.ContainsKey(_name)) {
            Debug.LogWarning("This string is not present in the XML file where you're reading: " + _name);
            return "#" + _name;
        }
        return (string)XML_Strings[_name];
    }

}