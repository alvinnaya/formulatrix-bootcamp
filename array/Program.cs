using library;
using System;
int[] arr = {1,5,2,1,5,7,8,9,5};
int[] bs = {1,2,3,4,5,12,14,16,19,20,27,34,57};





int left = 0;
int right = arr.Length-1;
int maxWater = 0;
int maxLeft = arr[left];
int maxRight = arr[right];
int length = 0;

while (left <= right)
{
    int LeftValue = arr[left];
    int RightValue = arr[right];
    int currentMax = Math.Min(LeftValue,RightValue)*(right-left);

    if(currentMax > maxWater)
    {
        maxWater = currentMax;
        maxLeft = LeftValue;
        maxRight = RightValue;
        length = right-left;

    }

    if(RightValue >= LeftValue)
    {
        right--;
    }
    else
    {
        left++;
    }
   



    
    
}

Console.WriteLine($"max value : {maxWater}, with number : {maxLeft},{maxRight}, and length :{length}");

int num = binarySearch.findLowestRecursive(bs,20,0,bs.Length-1);

Console.WriteLine($"your number at index : {num}, ");

string animal = "elephant";

Console.WriteLine($"ini huruf ke : {animal.Length-1} {animal[animal.Length-1]}");

Console.Write("selesai? ");

string input = Console.ReadLine();

Console.WriteLine("program selesai");

