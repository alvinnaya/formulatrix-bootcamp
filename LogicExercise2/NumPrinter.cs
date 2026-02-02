// See https://aka.ms/new-console-template for more information

namespace NumPrinter;


class NumPrinterClass{

Dictionary<int, string> rules = new Dictionary<int, string>();

public void AddRule(int divisor, string text)
    {
        rules[divisor] = text;
    }
String DecideFoobar(int num)
{
     string finalString = "";

        foreach (var rule in rules)
        {
            if (num % rule.Key == 0)
            {
                finalString += rule.Value;
            }
        }

        return finalString == "" ? num.ToString() : finalString;

}



public void Printing(int n)
    {

        
        for (int i = 0; i < n; i++)
        {
            string print = DecideFoobar(i);
            Console.Write(print);
            Console.Write(" ");
        }
        
    }


    

}





