using Cards;



 public interface IDiscardPile
    {
        Stack<ICard> Cards { get; }
    }


public class DiscardPile : IDiscardPile
    {
        public Stack<ICard> Cards { get; private set; }

        public DiscardPile()
        {
            Cards = new Stack<ICard>();
        }
    }