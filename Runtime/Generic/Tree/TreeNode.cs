using System;
using System.Collections.Generic;
using System.Linq;
using Aori.Exception;

namespace Aori.DSA.Generic
{
    public sealed class TreeNode<T>
    {
        public TreeNode<T> Parent { get; }
        public T Value { get; }

        private readonly HashSet<TreeNode<T>> _children;

        public IReadOnlyList<TreeNode<T>> Children => _children.ToArray();

        public int Height => _children.Count > 0
            ? _children.Max(child => child.Height) + 1
            : 0;

        public TreeNode(T value, TreeNode<T> parent)
        {
            Value = value;
            Parent = parent;

            _children = new HashSet<TreeNode<T>>();
        }

        internal void AddChild(TreeNode<T> node)
        {
            if (node == null)
            {
                throw new ArgumentException("Children cannot be null!");
            }

            if (!_children.Add(node))
            {
                throw new DuplicatedElementException("Child is already attached to this node");
            }
        }

        internal void RemoveChild(TreeNode<T> node)
        {
            if (node == null)
            {
                throw new ArgumentException("Children cannot be null!");
            }

            if (!_children.Remove(node))
            {
                throw new ElementNotFoundException("Child is not attached to this node");
            }
        }
    }
}