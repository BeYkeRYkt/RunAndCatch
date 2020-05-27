
public class PerkInfo
{
    public string name;
    public uint price;
    public ItemState state;

    public PerkInfo(string name, uint price, ItemState state)
    {
        this.name = name;
        this.price = price;
        this.state = state;
    }
    public PerkInfo() {
        this.name = "";
        this.price = 0;
        this.state = ItemState.Sale;
    }
}
