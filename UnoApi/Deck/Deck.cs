using System.Collections.Generic;
using Cards;

namespace Deck;

  
    public class Deck : IDeck
    {
        // HARUS Stack<ICard> sesuai interface
        public Stack<ICard> Cards { get; private set; }

        public Deck()
        {
            Cards = new Stack<ICard>();
        }
    }




