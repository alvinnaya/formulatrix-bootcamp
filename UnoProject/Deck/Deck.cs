using System.Collections.Generic;
using Cards;

namespace Deck;

    public interface IDeck
    {
        Stack<ICard> Cards { get; }
    }

    public class Deck : IDeck
    {
        // HARUS Stack<ICard> sesuai interface
        public Stack<ICard> Cards { get; private set; }

        public Deck()
        {
            Cards = new Stack<ICard>();
        }
    }




