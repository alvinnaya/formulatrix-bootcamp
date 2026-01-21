using Delegate;
using Event;
using TryAndCatch;
using OperatorSpace;

// int Square(int x) => x * x;
// int Cube(int x) => x * x * x;
// string Text(int x) => (x*x).ToString();
// void haaa(String a) => Console.WriteLine($"{a}");
// void haaa2(String a) => Console.WriteLine($"{a}222");
// Delegate.Delegate.publish = haaa;
// Delegate.Delegate.publish += haaa2;

// Delegate.Delegate d = new Delegate.Delegate();
int? x = null;
int y = x ?? 5;

Console.WriteLine($"{y}");

//practice event in main

Console.WriteLine("======== Event ============");

Publisher pub = new Publisher();
Event.Subscriber sub = new Subscriber();
Event.Subscriber2 sub2 = new Subscriber2();
sub.Subscribe(pub);
sub2.Subscribe(pub);

pub.TriggerEvent("geerrrr");
pub.TriggerEvent("how are you");
pub.SecondTriggerEvents("hello nigga", 2);






int Calc(int x) => 10 / x;

int Calc2(int x) => 10 / x;

TransformObj t = Calc;
t += Calc;


Console.WriteLine("============Try and Catch Error===========");

TryAndCatch.TryCatch.tryfunc(5,0);





Console.WriteLine("===============enumerator=================");
var list = new List<int> { 1,2,4,23,45,3,30 };

var enumerator = list.GetEnumerator();

 while (enumerator.MoveNext()) // Move to the next element
    {
        var element = enumerator.Current; // Get the current element
        enumerator.MoveNext();
        Console.WriteLine(element);
    }




Console.WriteLine("============ Operator ===========");

Operator a = new Operator(10);
Operator b = new Operator(4);
Operator c = new Operator(12);

Operator d = a + b + c ;
int e = d - a;
int f = d + 100;

Console.WriteLine($"a + b + c = {d.value}");

Console.WriteLine($" d - a = {e}");

Console.WriteLine($" f = d + 100 ____ {f}");







delegate int TransformObj(int x);

public delegate TResult Transformer<TArg, TResult>(TArg arg);
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


