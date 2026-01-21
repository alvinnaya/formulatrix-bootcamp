namespace Delegate;

public class Delegate
{
    public static int Square(int x) { return x * x; }

    // public delegate int Transformer(int x);

    public delegate TResult Transformer<TArg, TResult>(TArg arg);

    public static Action <string> publish;

    


     public static void Transform<T>(T[] values, Transformer<T, T> t) // Uses generic delegate
    {
        for (int i = 0; i < values.Length; i++)
            values[i] = t(values[i]);
    }

    


    public int SquareInstance(int x) => x * x;
}


