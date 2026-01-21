namespace OperatorSpace;

public class Operator
{
    public int value;
    public Operator(int Value)
    {
        value = Value;
    }

    public static Operator operator + (Operator a, Operator b)
    {
        return new Operator(a.value + b.value);
    }

    public static int operator + (Operator a, int c)
    {
        return a.value*c + c;
    }

    public static int operator - (Operator a, Operator b)
    {
        return a.value * b.value;
    }
}