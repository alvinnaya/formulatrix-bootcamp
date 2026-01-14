namespace library;

public static class binarySearch
{
    public static int findLowestRecursive(int[] arr, int num ,int Left, int Right)
    {
        int middle = Left + (Right - Left)/ 2 ;
        int middleVal = arr[middle];



        if (Left > Right)
            return -1;

        if(middleVal == num)
        {
            return middle;
        }else if(middleVal > num )
        {
            
            int newLeft = Left;
            int newRight = middle;
           return findLowestRecursive(arr,num,newLeft,newRight);
        }else if(middleVal < num)
        {
           int newLeft = middle;
            int newRight = Right;
           return findLowestRecursive(arr,num,newLeft,newRight);
        }

        return 0;
        
        
    }
}