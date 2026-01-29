namespace Cards;

    public enum CardType
    {
        Number0, Number1, Number2, Number3, Number4,
        Number5, Number6, Number7, Number8, Number9,
        Skip, Reverse, DrawTwo, Wild, WildDrawFour
    }

    public enum CardColor
    {
        Red, Yellow, Green, Blue
    }



     public interface ICard
    {
        CardType Type { get; }
        CardColor? Color { get; }
    }


     public class Card : ICard
    {
        public CardType Type { get; private set; }
        public CardColor? Color { get; private set; }

        public Card(CardType type, CardColor? color = null)
        {
            Type = type;
            Color = color;
        }

        public override string ToString()
        {
            return Color.HasValue ? $"{Color} {Type}" : $"{Type}";
        }
    }



