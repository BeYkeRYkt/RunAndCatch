using UnityEngine;

public class Character : MonoBehaviour
{
    public string name;
    public uint price;
    public ItemState state;

    public Character(string name, uint price, ItemState state)
    {
        this.name = name;
        this.price = price;
        this.state = state;
    }
    public Character()
    {
        this.name = "";
        this.price = 0;
        this.state = ItemState.Sale;
    }
    public Character(Character value)
    {
        this.name = value.name;
        this.price = value.price;
        this.state = value.state;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    void Start()
    {
        Debug.Log(this.ToString());
    }

}
