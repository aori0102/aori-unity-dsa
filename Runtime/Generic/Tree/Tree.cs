using System;
using System.Collections.Generic;
using System.Linq;
using Aori.Exception;

namespace Aori.DSA.Generic
{
    /// <summary>
    /// Simple hierarchical tree.
    /// </summary>
    public sealed class Tree<T>
    {
        private TreeNode<T> m_root;

        private readonly HashSet<TreeNode<T>> _nodeSet = new();

        public IEnumerable<TreeNode<T>> Nodes => _nodeSet.ToArray();
        public int Count => _nodeSet.Count;
        public int Height => _nodeSet.Max(node => node.Height);

        public int HeightOf(TreeNode<T> node)
        {
            if (!_nodeSet.Contains(node))
            {
                return -1;
            }

            return node.Height;
        }

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

        public IEnumerable<TreeNode<T>> LevelOrder()
        {
            if (m_root == null)
            {
                yield break;
            }

            var traversalQueue = new Queue<TreeNode<T>>();
            traversalQueue.Enqueue(m_root);
            while (traversalQueue.TryDequeue(out var node))
            {
                yield return node;

                foreach (var child in node.Children)
                {
                    traversalQueue.Enqueue(child);
                }
            }
        }

        public IEnumerable<TreeNode<T>> PreOrder()
        {
            var output = new List<TreeNode<T>>();
            if (m_root == null)
            {
                return output;
            }

            var traversalStack = new Stack<TreeNode<T>>();
            traversalStack.Push(m_root);
            while (traversalStack.TryPop(out var node))
            {
                output.Add(node);
                var children = node.Children;
                for (var i = children.Count - 1; i >= 0; i--)
                {
                    traversalStack.Push(children[i]);
                }
            }

            return output;
        }

        public IEnumerable<TreeNode<T>> PostOrder()
        {
            var outputStack = new Stack<TreeNode<T>>();
            if (m_root == null)
            {
                return outputStack;
            }

            var traversalStack = new Stack<TreeNode<T>>();
            traversalStack.Push(m_root);
            while (traversalStack.TryPop(out var node))
            {
                outputStack.Push(node);
                foreach (var child in node.Children)
                {
                    traversalStack.Push(child);
                }
            }

            return outputStack;
        }

        [Obsolete("Use PostOrder() instead")]
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