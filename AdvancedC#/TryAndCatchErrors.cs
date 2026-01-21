namespace TryAndCatch;
public class TryCatch
{
    public static void tryfunc(int a, int b)
    {
         try
        {
            
            int hasil = a / b; // error
            Console.WriteLine(hasil);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Terjadi error: " + ex.Message);
            // trying error inside catch
           // error
        

        }
        finally
        {
            Console.WriteLine("try and catch error selesai");

        }
    }
}