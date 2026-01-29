namespace Players;


 public interface IPlayer
    {
        string Name { get; }
        bool HasCalledUno { get; set; }
    }



public class Player : IPlayer
    {
        public string Name { get; private set; }
        public bool HasCalledUno { get; set; }

        public Player(string name)
        {
            Name = name;
            HasCalledUno = false;
        }

        public override string ToString() => Name;
    }