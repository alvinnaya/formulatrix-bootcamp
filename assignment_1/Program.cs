// See https://aka.ms/new-console-template for more information




int n = 15;

void DecideFoobar(int num)
{
    if (num % 3 == 0)
    {
        Console.Write("foo");
    }else if (num % 5 == 0)
    {
        
        Console.Write(" bar");
    }else if((num % 3 == 0)&&(num % 5 == 0))
    {
        Console.Write(" foobar");
    }
    else
    {
        Console.Write($" {num}");
    }
}


for (int i = 0; i < n; i++)
{
    DecideFoobar(i);
}
    

