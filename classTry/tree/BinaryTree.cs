
namespace tree;
public class TreeNode<T>
{
    public T Value;
    public TreeNode<T>? Left;
    public TreeNode<T>? Right;

    public TreeNode(T value)
    {
        Value = value;
    }
}



public class BinarySearchTree<T> where T : IComparable<T>
{
    public TreeNode<T>? Root;

    public void Insert(T value)
    {
        Root = InsertRecursive(Root, value);
    }

    private TreeNode<T> InsertRecursive(TreeNode<T>? node, T value)
    {
        if (node == null)
            return new TreeNode<T>(value);

        if (value.CompareTo(node.Value) < 0)
            node.Left = InsertRecursive(node.Left, value);
        else if (value.CompareTo(node.Value) > 0)
            node.Right = InsertRecursive(node.Right, value);

        return node;
    }

    public int maxDepth()
    {
        return MaxDepthRecursive(this.Root);
    }
    public int MaxDepthRecursive(TreeNode<T>? Root)
    {
        if(Root == null)
        {
            return 0;
        }

        return 1 + Math.Max(MaxDepthRecursive(Root?.Right),MaxDepthRecursive(Root?.Left));
    }
}
