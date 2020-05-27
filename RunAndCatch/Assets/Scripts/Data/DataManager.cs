using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public uint fish = 0;
    public uint record = 0;

    public string language = "eng";

    protected CharacterInfo[] characters;

    private string key = "RunAndCatch.DEV";

    public void Load()
    {
        if (PlayerPrefs.HasKey(key)) // Если игра уже запускалась до этого момента
        {
            Debug.Log("has key - true!");
            string valueDataStorage = PlayerPrefs.GetString(key);
            Debug.Log(valueDataStorage);
            DataStorage data = JsonUtility.FromJson<DataStorage>(valueDataStorage);
            if (data == null)
                Debug.Log("data is null!");

            this.characters = new CharacterInfo[data.characters_name.Length];
            this.fish = data.fish;
            this.record = data.record;
            this.language = data.language;
            for (int i = 0; i < data.characters_name.Length; i++)
            {
                this.characters[i] = new CharacterInfo();
                this.characters[i].name = data.characters_name[i];
                this.characters[i].price = data.characters_price[i];
                this.characters[i].state = data.characters_state[i];
            }
            Debug.Log("array characters initialized!");
        }
        else  // Если игра запущена в первый раз
        {
            CharactersManager cm = FindObjectOfType<CharactersManager>();
            if (cm == null)
                Debug.Log("cm is null!");
            List<Character> initCharacters = cm.ListCharacters();
            characters = new CharacterInfo[initCharacters.Count];
            for (int i = 0; i < initCharacters.Count; i++)
            {
                characters[i] = new CharacterInfo();
                characters[i].name = initCharacters[i].name;
                characters[i].price = initCharacters[i].price;
                characters[i].state = initCharacters[i].state;
            }
        }
    }

    public void Save()
    {
        CharactersManager cm = FindObjectOfType<CharactersManager>();
        if (cm == null)
            Debug.Log("cm is null!");
        List<Character> initCharacters = cm.ListCharacters();
        Debug.Log(initCharacters);
        Debug.Log(initCharacters.Count);
        characters = new CharacterInfo[initCharacters.Count];
        for (int i = 0; i < initCharacters.Count; i++)
        {
            characters[i] = new CharacterInfo();
            characters[i].name = initCharacters[i].name;
            characters[i].price = initCharacters[i].price;
            characters[i].state = initCharacters[i].state;
        }

        DataStorage storage = new DataStorage(characters.Length);
        if (storage == null)
            Debug.Log("storage = null");
        storage.fish = this.fish;
        storage.record = this.record;
        storage.language = this.language;

        for (int i = 0; i < this.characters.Length; i++)
        {
            storage.characters_name[i] = this.characters[i].name;
            storage.characters_price[i] = this.characters[i].price;
            storage.characters_state[i] = this.characters[i].state;
        }

        string valueDataStorage = JsonUtility.ToJson(storage);
        Debug.Log(valueDataStorage);
        PlayerPrefs.SetString(key, valueDataStorage);
        PlayerPrefs.Save();
        Debug.Log("Save!");
        Debug.Log("fish = " + fish);
    }
    public CharacterInfo[] GetCharacters()
    {
        Debug.Log("call GetCharacters() ");
        return (CharacterInfo[])characters.Clone();
    }
    void Awake()
    {
        Debug.Log("DataManager.Awake()");
    }
    void OnDisable()
    {
        Debug.Log("Disable DataManager");
        Save();
    }
}
class DataStorage
{
    public uint fish = 0;
    public uint record = 0;

    public string language = "rus";

    public string[] characters_name;
    public uint[] characters_price;
    public ItemState[] characters_state;

    public DataStorage(int length_storage)
    {
        characters_name = new string[length_storage];
        characters_price = new uint[length_storage];
        characters_state = new ItemState[length_storage];
    }
}