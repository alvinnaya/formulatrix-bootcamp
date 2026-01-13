

BinaryTree tree = new BinaryTree();


tree.Insert(11);
tree.Insert(5);
tree.Insert(15);
tree.Insert(12);

Console.WriteLine(tree.Root.Data);



class Node
{
    public int Data { get; set; }
    public Node? Left { get; set; }
    public Node? Right { get; set; }

    public Node(int data)
    {
        Data = data;
    }
}


class BinaryTree
{
    public Node? Root { get; private set; }

    public void Insert(int data)
    {
        Root = InsertRec(Root, data);
    }

    private Node InsertRec(Node? node, int data)

    {
        if (node is null)
            return new Node(data);

        if (data < node.Data)
            node.Left = InsertRec(node.Left, data);
        else if (data > node.Data)
            node.Right = InsertRec(node.Right, data);

        return node;
    }

    public int maxLength(Node? node)
    {
        if (node.Left)
        {
            maxLength(node.Left);
            
        }
        
        maxLength(node.Right)

        return 1;
    }
}


