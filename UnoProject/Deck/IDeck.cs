using System.Collections.Generic;
using Cards;

namespace Deck;

    public interface IDeck
    {
        Stack<ICard> Cards { get; }
    }





