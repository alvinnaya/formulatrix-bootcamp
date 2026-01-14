using classLainnya;
using tree;

PrintBiji jumlahBiji = new PrintBiji(10);
jumlahBiji.gandaBiji();

jumlahBiji.Write();

jumlahBiji.gandaBiji();
jumlahBiji.gandaBiji();

jumlahBiji.Write();

JumlahBiji biji = jumlahBiji as JumlahBiji;


var Email = new classLainnya.EmailNotification();
var orderService = new classLainnya.OrderService(Email);
orderService.PlaceOrder();

var tree = new tree.BinarySearchTree<int>();
tree.Insert(3);
tree.Insert(7);
tree.Insert(9);


Console.WriteLine($"node value {tree.Root?.Right?.Value}");

int maxDepth = tree.maxDepth();
Console.WriteLine($"max Depth {maxDepth}");






public class JumlahBiji
{
    public int biji;

    public JumlahBiji(int b)
    {
        biji = b;
    }

    public int gandaBiji() => this.biji = this.biji*2;
}


public class PrintBiji : JumlahBiji
{
   public PrintBiji(int b) : base(b)
    {
        
    }

    public void Write()
    {
        Console.WriteLine($"bijinya ada : {biji}");
    }
}