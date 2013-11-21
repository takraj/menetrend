using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUSZ.GRAFIT.Pathfinder
{
    public class FibonacciHeap<T>
    {
        private Node minRoot;
        private Node[] rootListByRank;
        private long count;

        public class Node
        {
            public Node left;
            public Node right;
            public Node parent;
            public Node children;
            public int rank;
            public long key;
            public T data;

            public T Value
            {
                get
                {
                    return data;
                }
            }

            public bool AddChild(Node node)
            {
                if (this.children != null)
                {
                    this.children.AddSibling(node);
                }
                else
                {
                    this.children = node;
                    node.parent = this;
                    this.rank = 1;
                }

                return true;
            }

            public bool AddSibling(Node node)
            {
                Node temp = this.right;

                if (temp == null)
                {
                    return false;
                }

                temp.right = node;
                node.left = temp;
                node.parent = this.parent;
                node.right = null;

                if (this.parent != null)
                {
                    this.parent.rank++;
                }

                return true;
            }

            public bool Remove()
            {
                if (this.parent != null)
                {
                    this.parent.rank--;

                    if (this.left != null)
                    {
                        this.parent.children = this.left;
                    }
                    else if (this.right != null)
                    {
                        this.parent.children = this.right;
                    }
                    else
                    {
                        this.parent.children = null;
                    }
                }

                if (this.left != null)
                {
                    this.left.right = this.right;
                }
                if (this.right != null)
                {
                    this.right.left = this.left;
                }

                this.left = null;
                this.right = null;
                this.parent = null;

                return true;
            }
        }

        public FibonacciHeap()
        {
            this.rootListByRank = new Node[100];
            this.count = 0;

            // Initialize list of roots    
            for (int i = 0; i < 100; i++)
            {
                rootListByRank[i] = null;
            }
        }

        public Node CreateNode(T data, long key)
        {
            return new Node
            {
                children = null,
                data = data,
                key = key,
                left = null,
                parent = null,
                rank = 0,
                right = null
            };
        }

        public bool Insert(Node node)
        {
            if (node == null)
            {
                return false;
            }

            if (this.minRoot == null)
            {
                this.minRoot = node;
            }
            else
            {
                this.minRoot.AddSibling(node);

                if (this.minRoot.key > node.key)
                {
                    this.minRoot = node;
                }
            }

            this.count++;
            return true;
        }

        public void DecreaseKey(long delta, Node node)
        {
            node.key = delta;

            if (node.parent != null)
            {
                node.Remove();
                this.minRoot.AddSibling(node);
            }

            // Check if key is smaller than the key of minRoot
            if (node.key < this.minRoot.key)
            {
                this.minRoot = node;
            }
        }

        public Node FindMin()
        {
            return this.minRoot;
        }

        public bool Link(Node root)
        {
            // Insert Node into root list
            if (this.rootListByRank[root.rank] == null)
            {
                this.rootListByRank[root.rank] = root;
                return false;
            }
            else
            {
                // Link the two roots
                Node linkNode = this.rootListByRank[root.rank];
                this.rootListByRank[root.rank] = null;

                if (root.key < linkNode.key || root == this.minRoot)
                {
                    linkNode.Remove();
                    root.AddChild(linkNode);

                    if (this.rootListByRank[root.rank] != null)
                    {
                        this.Link(root);
                    }
                    else
                    {
                        this.rootListByRank[root.rank] = root;
                    }
                }
                else
                {
                    root.Remove();
                    linkNode.AddChild(root);

                    if (this.rootListByRank[linkNode.rank] != null)
                    {
                        this.Link(linkNode);
                    }
                    else
                    {
                        this.rootListByRank[linkNode.rank] = linkNode;
                    }
                }
                return true;
            }
        }

        public Node DeleteMin()
        {
            Node temp = minRoot.children.left;
            Node nextTemp = null;

            // Adding Children to root list        
            while (temp != null)
            {
                nextTemp = temp.right; // Save next Sibling
                temp.Remove();
                this.minRoot.AddSibling(temp);
                temp = nextTemp;
            }

            // Select the left-most sibling of minRoot
            temp = this.minRoot.left;

            // Remove minRoot and set it to any sibling, if there exists one
            if (temp == this.minRoot)
            {
                if (this.minRoot.right != null)
                {
                    temp = this.minRoot.right;
                }
                else
                {
                    // Heap is obviously empty
                    Node output = this.minRoot;
                    this.minRoot.Remove();
                    this.minRoot = null;
                    this.count = 0;
                    return output;
                }
            }
            Node result = this.minRoot;
            this.minRoot.Remove();
            this.minRoot = temp;

            // Initialize list of roots    
            for (int i = 0; i < 100; i++)
            {
                this.rootListByRank[i] = null;
            }

            while (temp != null)
            {
                // Check if key of current vertex is smaller than the key of minRoot
                if (temp.key < this.minRoot.key)
                {
                    this.minRoot = temp;
                }

                nextTemp = temp.right;
                this.Link(temp);
                temp = nextTemp;
            }

            this.count--;
            return result;
        }

        public long Count
        {
            get
            {
                return this.count;
            }
        }

        public void Add(T data, long key)
        {
            this.Insert(this.CreateNode(data, key));
        }

        public Node RemoveMin()
        {
            return this.DeleteMin();
        }
    }
}
