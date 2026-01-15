namespace storage;

public interface Deposit
{
    void addDeposit(int m);
    void takeDeposit(int t);
    void checkDeposit();
}


public class BankAccount : Deposit
{

    private int saldo = 0;
    private static int totalBalance = 0;
    public void addDeposit(int t)
    {
        this.saldo += t;
        totalBalance += t;
    }

    public void takeDeposit(int t)
    {
        this.saldo -= t;
        totalBalance -= t;
    }

    public void checkDeposit()
    {
        Console.WriteLine($"account1 :{this.saldo}");
    }

    public static void checkTotalBalance()
    {
        Console.WriteLine($"account1 :{totalBalance}");
    }
}


