using System;
using System.Collections.Generic;
using Aori.Exception;

namespace Aori.DSA.Tree
{
    /// <summary>
    /// Simple hierarchical tree.
    /// </summary>
    public sealed class Tree<T>
    {
        private TreeNode<T> m_root;

        private readonly HashSet<TreeNode<T>> _nodeSet = new();

        public int Count => _nodeSet.Count;

        public void AddNode(TreeNode<T> node)
        {
            if (node.Parent == null)
            {
                if (m_root != null)
                {
                    throw new ArgumentException("Root node already exists");
                }

                m_root = node;
            }
            else
            {
                node.Parent.AddChild(node);
            }

            if (node == null)
            {
                throw new ArgumentException("Node cannot be null");
            }

            if (!_nodeSet.Add(node))
            {
                throw new DuplicatedElementException("Node already exists in tree");
            }
        }

        public void RemoveNode(TreeNode<T> node)
        {
            if (node == null)
            {
                throw new ArgumentException("Node cannot be null");
            }

            if (!_nodeSet.Contains(node))
            {
                throw new ElementNotFoundException("Node does not exist in tree");
            }

            node.Parent?.RemoveChild(node);

            var removingStack = new Stack<TreeNode<T>>();
            removingStack.Push(node);
            while (removingStack.TryPop(out var removingNode))
            {
                _nodeSet.Remove(removingNode);
                foreach (var child in node.Children)
                {
                    removingStack.Push(child);
                }
            }
        }

        public Queue<TreeNode<T>> GetPostOrderTraversal()
        {
            var result = new Queue<TreeNode<T>>();
            if (m_root == null)
            {
                return result;
            }

            // Post order traversal
            var traversalStack = new Stack<TreeNode<T>>();
            var outputStack = new Stack<TreeNode<T>>();

            traversalStack.Push(m_root);
            while (traversalStack.TryPop(out var node))
            {
                outputStack.Push(node);
                foreach (var child in node.Children)
                {
                    traversalStack.Push(child);
                }
            }

            while (outputStack.TryPop(out var node))
            {
                result.Enqueue(node);
            }

            return result;
        }

        public void Clear()
        {
            _nodeSet.Clear();
            m_root = null;
        }
    }
}