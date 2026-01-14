namespace classLainnya;

interface INotificationService
{
    void Send(string to, string message);
}


class EmailNotification : INotificationService
{
    public void Send(string to, string message)
    {
        Console.WriteLine($"📧 Email ke {to}: {message}");
    }
}

class SmsNotification : INotificationService
{
    public void Send(string to, string message)
    {
        Console.WriteLine($"📱 SMS ke {to}: {message}");
    }
}


class OrderService
{
    private readonly INotificationService _notification;

    public OrderService(INotificationService notification)
    {
        _notification = notification;
    }

    public void PlaceOrder()
    {
        _notification.Send("user@mail.com", "Order placed");
    }
}



