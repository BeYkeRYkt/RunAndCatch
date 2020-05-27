using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersManager : MonoBehaviour
{
    public Character[] charactersPrefs;
    protected List<Character> characters;

    protected DataManager dm;

    public void Initialize()
    {
        characters = new List<Character>();
        foreach (Character characterPref in charactersPrefs)
        {
            Character createdCharacter = Instantiate(characterPref);
            createdCharacter.Hide();
            characters.Add(createdCharacter);
        }

    }

    public void InitializeCharactersFromDataManager()
    {
        dm = FindObjectOfType<DataManager>();
        if (dm == null)
            Debug.Log("dm is null!");
        CharacterInfo[] charactersFromDataManager = dm.GetCharacters();
        if (charactersFromDataManager == null)
            Debug.Log("dm.characters is null!");
        for (int i = 0; i < charactersFromDataManager.Length; i++)
        {
            for (int j = 0; j < characters.Count; j++)
            {
                if (characters[j].name == charactersFromDataManager[i].name)
                {
                    characters[j].price = charactersFromDataManager[i].price;
                    characters[j].state = charactersFromDataManager[i].state;
                    break;
                }
            }
        }
    }

    public Character GetCharacter(int i)
    {
        if (i > characters.Count || i < 0)
        {
            Debug.Log("Error! Out of the range!");
            //return new Character();
            return null;
        }
        //return new Character(characters[i]);
        Character character = Instantiate(characters[i]);
        if (character.GetComponentInChildren<Rigidbody>() && character.GetComponentInChildren<JoystickPlayerExample>())
        {
            character.GetComponentInChildren<Rigidbody>().useGravity = false;
            character.GetComponentInChildren<JoystickPlayerExample>().enabled = false;
        }
        Debug.Log("Character was get!");
        return character;
    }
    public Character GetSelectedCharacter()
    {
        Character character = PlayableCharacter();
        if (character.GetComponentInChildren<Rigidbody>() && character.GetComponentInChildren<JoystickPlayerExample>())
        {
            character.GetComponentInChildren<Rigidbody>().useGravity = false;
            character.GetComponentInChildren<JoystickPlayerExample>().enabled = false;
        }
        return character;
    }
    public List<Character> ListCharacters()
    {
        return characters;
    }

    public void SelectCharacter(int i)
    {
        if (characters[i].state == ItemState.Selected)
        {
            Debug.Log("Character already selected!");
            return;
        }
        foreach (Character character in characters)
        {
            if (character.state == ItemState.Selected)
            {
                character.state = ItemState.Bought;
                break;
            }
        }
        if (characters[i].state == ItemState.Sale)
        {
            Debug.Log("Begin need buy character!");
            return;
        }
        characters[i].state = ItemState.Selected;
    }
    public void BuyCharacter(int i)
    {
        if (characters[i].state == ItemState.Bought || characters[i].state == ItemState.Selected)
        {
            Debug.Log("Character already bought!");
            return;
        }
        characters[i].state = ItemState.Bought;
    }

    public Character PlayableCharacter()
    {


        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].state == ItemState.Selected)
            {
                return Instantiate(characters[i]);
            }

        }
        return null;

    }
    public int CountCharacters()
    {
        return characters.Count;
    }
    void Awake()
    {
        Debug.Log("CharactersManager.Awake()");
    }
    void Start()
    {
        Debug.Log("CharactersManager.Start()");
    }

}
