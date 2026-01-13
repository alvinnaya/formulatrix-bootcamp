// See https://aka.ms/new-console-template for more information




int n = 15;

void DecideFoobar(int num)
{
    if (num % 3 == 0)
    {
        Console.WriteLine("foo");
    }else if (num % 5 == 0)
    {
        
        Console.WriteLine("Bar");
    }else if((num % 3 == 0)&&(num % 5 == 0))
    {
        Console.WriteLine("foobar");
    }
    else
    {
        Console.WriteLine(num);
    }
}


for (int i = 1; i <= n; i++)
{
    DecideFoobar(i);
}
    

