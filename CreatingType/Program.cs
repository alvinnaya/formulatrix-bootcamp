using storage;


BankAccount account1 = new BankAccount();
BankAccount account2 = new BankAccount();


account1.addDeposit(100000000);
account2.addDeposit(200000000);


account1.checkDeposit();
account2.checkDeposit();

BankAccount.checkTotalBalance();

account1.takeDeposit(20000054);
BankAccount.checkTotalBalance();


Widget widget = new Widget();


widget.Foo();


interface I1 { void Foo(); }
interface I2 { int Foo(); }


public class Widget : I1, I2 // Implements both interfaces
{
    // Implicit implementation for I1.Foo()
    public void Foo()
    {
        Console.WriteLine("Widget's implementation of I1.Foo");
    }

    // Explicit implementation for I2.Foo()
    // Note the interface name preceding the member name, and no access modifier.
    int I2.Foo()
    {
        Console.WriteLine("Widget's implementation of I2.Foo");
        return 42;
    }
}












class Octopus
{
    string name;       // A private field
    public int Age = 10; // A public field initialized to 10

    Octopus juniorOctopus;

    public Octopus(string n)
    {
        name = n;
        Age = 10;
    }

    public Octopus(string n, int age)
    {
        name = n;
        Age = age;

    }

    public void createJunior(string name, int age )
    {
        juniorOctopus = new Octopus(name,age);
    }

    public String getName()
    {
        return this.name;
    }

    public int getAge()
    {
        return this.Age;
    }

    public Octopus getJunior()
    {
        return juniorOctopus;
    }

    public void consoleWrite()
    {
        Console.WriteLine($"name : {this.name}, age : {this.Age}");
    }

    public void setJunior(Octopus junior)
    {
        this.juniorOctopus = junior;
    }

}


