
namespace Event;

public delegate void PriceChangedHandler(decimal oldPrice, decimal newPrice);


public class PriceChangedEventArgs : EventArgs
    {
        public decimal LastPrice { get; }
        public decimal NewPrice { get; }

        public PriceChangedEventArgs(decimal lastPrice, decimal newPrice)
        {
            LastPrice = lastPrice;
            NewPrice = newPrice;
        }
    }

public class IdontknowEventArgs : EventArgs
{
    
}

public class Stock
{
    private readonly string _symbol;
    private decimal _price;
    public event EventHandler<PriceChangedEventArgs> PriceChanged;
    public event EventHandler<IdontknowEventArgs> IdontKnow;

    
    public Stock(string symbol)
    {
        _symbol = symbol;
    }

    protected virtual void OnPriceChanged(PriceChangedEventArgs e)
    {
        // Thread-safe invocation
        PriceChanged?.Invoke(this, e);
    }

    protected virtual void OnIdontKnowChanged(IdontknowEventArgs e)
    {
        IdontKnow?.Invoke(this, e);
    }

    


      public decimal Price
        {
            get => _price;
            set
            {
                if (_price == value)
                    return;

                decimal oldPrice = _price;
                _price = value;

                // Raise the event
                OnPriceChanged(
                    new PriceChangedEventArgs(oldPrice, _price)
                );

                OnIdontKnowChanged(
                    new IdontknowEventArgs()
                );
            }
        }


}



public class Event
{
     public static event Action<string> Clicked;

    public static void Click(string a)
    {
        Clicked?.Invoke(a);
    }


}