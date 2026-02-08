using Cards;



 

public class DiscardPile : IDiscardPile
    {
        public Stack<ICard> Cards { get; private set; }

        public DiscardPile()
        {
            Cards = new Stack<ICard>();
        }
    }