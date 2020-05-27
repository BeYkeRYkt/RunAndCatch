
public class CharacterInfo
{
    public string name;
    public uint price;
    public ItemState state;

    public CharacterInfo(string name, uint price, ItemState state)
    {
        this.name = name;
        this.price = price;
        this.state = state;
    }
    public CharacterInfo() {
        this.name = "";
        this.price = 0;
        this.state = ItemState.Sale;
    }
}
