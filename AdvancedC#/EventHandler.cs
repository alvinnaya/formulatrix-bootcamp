
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




// 1. Buat class EventArgs kalo butuh data tambahan
public class MessageEventArgs : EventArgs
{
    public string Message { get; }
    public MessageEventArgs(string message)
    {
        Message = message;
    }
}

public class SecondMessafeEvent : EventArgs
{
    public string Message{get;}
    public int Id{get;}

    public SecondMessafeEvent(string message, int id)
    {
        Message = message;
        Id = id;
    }
}

//Publisher 
public class Publisher
{
    // Event menggunakan EventHandler<T>
    public event EventHandler<MessageEventArgs> MessageReceived;
    public event EventHandler<SecondMessafeEvent> SecondMessage;

    public event EventHandler thirdMessage;


    public void TriggerEvent(string msg)
    {
        // ? operator -> cek null sebelum invoke
        MessageReceived?.Invoke(this, new MessageEventArgs(msg));

    }

    public void SecondTriggerEvents(string msg,int Id)
    {
        SecondMessage?.Invoke(this, new SecondMessafeEvent(msg,Id));
    }
}

//Subscriber
public class Subscriber
{
    public void Subscribe(Publisher pub)
    {
        pub.MessageReceived += HandleMessage;
    }

    private void HandleMessage(object sender, MessageEventArgs e)
    {
        Console.WriteLine($"Menerima pesan: {e.Message} dari {sender.GetType().Name}");
    }
}

public class Subscriber2
{
    
    public void Subscribe(Publisher pub)
    {
        pub.SecondMessage += HandleMessage;
    }

    private void HandleMessage(object sender, SecondMessafeEvent e)
    {
         Console.WriteLine($"Menerima pesan: {e.Message} and Id: {e.Id} dari {sender.GetType().Name}");
    }
}


//Event Accessors (Explicit Implementation)

class Engine
{
    public event EventHandler Overheated;
}

class Car
{
    private Engine engine = new Engine();

    public event EventHandler Overheated
    {
        add    { engine.Overheated += value; }
        remove { engine.Overheated -= value; }
    }
}


