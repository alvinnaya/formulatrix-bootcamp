using Delegate;
using Event;



// int Square(int x) => x * x;
// int Cube(int x) => x * x * x;
// string Text(int x) => (x*x).ToString();
// void haaa(String a) => Console.WriteLine($"{a}");
// void haaa2(String a) => Console.WriteLine($"{a}222");
// Delegate.Delegate.publish = haaa;
// Delegate.Delegate.publish += haaa2;

// Delegate.Delegate d = new Delegate.Delegate();

Event.Stock stock = new Stock("NVDIA");
stock.Price = 50;


int Calc(int x) => 10 / x;

try
{
int x = Calc(0);

Console.WriteLine(x);

}
catch (DivideByZeroException e)
{
    Console.WriteLine("tidak boleh dibagi 0");
}


// Delegate.Delegate.Transformer<int,int> squ = Square;
// squ += Cube;

// Delegate.Delegate.publish("boom");
// Event.Event.Clicked += haaa;
// Event.Event.Clicked += haaa2;

// Event.Event.Click("woosh");



// void Transform(int[] values, Transformer t) // 't' is a delegate parameter
// {
//     for (int i = 0; i < values.Length; i++)
//         values[i] = t(values[i]); // Invoke the plug-in method
// }


// int[] values = { 1, 2, 3,4,5,6 };

// Delegate.Delegate.Transform(values,Square);
// foreach (int i in values)
//     Console.Write(i + "  ");

// delegate int Transformer(int x); // Delegate type declaration