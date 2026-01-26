// See https://aka.ms/new-console-template for more information




int n = 200;

String DecideFoobar(int num)
{
    String finalString = "";
    if (num % 3 == 0)
    {
        finalString = finalString + "foo";
    }

    if(num % 4 == 0)
    {
        finalString = finalString + "baz";
    }

    if (num % 5 == 0)
    {
        
        finalString = finalString + "bar";
    }

    if(num % 7 == 0)
    {
        finalString = finalString + "jazz";
    }

    if(num % 9 == 0)
    {
        finalString = finalString + "huzz";
    }

    if(finalString == "")
    {
        finalString = num.ToString();
    }

    return finalString;
}


for (int i = 0; i < n; i++)
{
    string print = DecideFoobar(i);
    Console.Write(print);
    Console.Write(" ");
}
    

