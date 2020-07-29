namespace Hotel.Models
{
    public class Item
    {
        public int Id { set; get; }
        public Type Type { set; get; }
        public string Name { set; get; }
        public float Price { set; get; }
    }

    public enum Type
    {
        Food = 1,
        Drink = 2
    }
}