// <copyright file="Node.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

/// <summary>
/// Marcov chain node stuff
/// </summary>
namespace MarkovChain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Markov Chain node.
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Subsequent nodes.
        /// </summary>
        private Dictionary<Node, int> nextNodes = new Dictionary<Node, int>();

        /// <summary>
        /// List of nodes.
        /// </summary>
        private List<Node> nodesList;

        /// <summary>
        /// Random seed.
        /// </summary>
        private Random randomGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="Node" /> class.
        /// </summary>
        /// <param name="value">Node value.</param>
        /// <param name="nodelist">Node list object.</param>
        /// <param name="randomNumberGenerator">Provides the random number generator.</param>
        public Node(string value, List<Node> nodelist, Random randomNumberGenerator)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
            this.nodesList = nodelist ?? new List<Node>();
            this.nodesList.Add(this);
            this.randomGenerator = randomNumberGenerator ?? throw new ArgumentNullException(nameof(randomNumberGenerator));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node" /> class.
        /// </summary>
        public Node()
        {
            this.nodesList = new List<Node>
            {
                this,
            };
            this.Value = null;
            this.randomGenerator = new Random();
        }

        /// <summary>
        /// Gets the node count.
        /// </summary>
        public int? NodesCount
        {
            get => this.nodesList.Count;
        }

        /// <summary>
        /// Gets or sets the node Value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Compares if node a and b are ==.
        /// </summary>
        /// <param name="a">first node to compare.</param>
        /// <param name="b">second node to compare.</param>
        /// <returns>true of false depending if th enodes are equal or not.</returns>
        public static bool operator ==(Node a, Node b)
        {
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            if (a is null || b is null)
            {
                return false;
            }
            else
            {
                return a.Equals(b);
            }
        }

        /// <summary>
        /// Compares if node a and b are !=.
        /// </summary>
        /// <param name="a">first node to compare.</param>
        /// <param name="b">Second node to compare.</param>
        /// <returns>true or false depending on the compatison.</returns>
        public static bool operator !=(Node a, Node b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Compares Nodes values.
        /// </summary>
        /// <param name="n">Node to compare.</param>
        /// <returns>true wehn the nodes are equal, flase when the node values are different.</returns>
        public bool Equals(Node n)
        {
            if (!(n is null))
            {
                return n.Value == this.Value;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the random generator.
        /// </summary>
        /// <param name="rnd">Random object.</param>
        public void SetRandomGenerator(Random rnd)
        {
            if (rnd is null)
            {
                throw new ArgumentNullException(nameof(rnd));
            }
            else
            {
                this.randomGenerator = rnd;
            }
        }

        /// <summary>
        /// Compares Nodes values.
        /// </summary>
        /// <param name="word">string word.</param>
        /// <returns>true or false.</returns>
        public bool Equals(string word) => this.Value == word;

        /// <summary>
        /// Adds a child to the node.
        /// </summary>
        /// <param name="child">Node to add.</param>
        public void AddChild(Node child)
        {
            Node item = this.nextNodes.FirstOrDefault(x => x.Key.Value.Equals(child.Value, StringComparison.InvariantCulture)).Key;
            if (item is null)
            {
                this.nextNodes.Add(child, 1);
            }
            else
            {
                this.nextNodes[item]++;
            }
        }

        /// <summary>
        /// returns the next node.
        /// </summary>
        /// <returns>Type Node.</returns>
        public Node NextNode()
        {
            if (this.nextNodes.Count == 0)
            {
                return null;
            }

            int maxValue = this.nextNodes.Count;
            int r = this.randomGenerator.Next(0, maxValue);

            return this.nextNodes.ToArray()[r].Key;
        }

        /// <summary>
        /// Adds a String to the Chain.
        /// </summary>
        /// <param name="sequence">list of words to add.</param>
        public void AddSequence(string[] sequence)
        {
            if (sequence is null)
            {
                throw new ArgumentNullException(nameof(sequence));
            }

            if (sequence.Length > 0)
            {
                Node n = this.nodesList.Find(p => p.Value == sequence[0]);
                if (n is null)
                {
                    n = new Node(sequence[0], this.nodesList, this.randomGenerator);
                }

                this.AddChild(n);
                n.AddSequence(sequence.Skip(1).ToArray());
            }
        }

        /// <summary>
        /// Returns a sequence of words from this node on.
        /// </summary>
        /// <param name="count">Number of max hops.</param>
        /// <returns>string with all hops in strings.</returns>
        public string ReturnSequence(int count)
        {
            string next;
            if (this.NextNode() != null && count > 0)
            {
                next = this.NextNode().ReturnSequence(--count);
                if (next == null)
                {
                    next = string.Empty;
                }
            }
            else
            {
                next = string.Empty;
            }

            return $"{this.Value} {next}";
        }

        /// <summary>
        /// Returns a sequence of words from this node on.
        /// </summary>
        /// <param name="count">Number of max hops.</param>
        /// <returns>string with all hops in strings.</returns>
        public string ReturnSequenceNoSpace(int count)
        {
            string next;
            if (this.NextNode() != null && count > 0)
            {
                next = this.NextNode().ReturnSequenceNoSpace(--count);
                if (next == null)
                {
                    next = string.Empty;
                }
            }
            else
            {
                next = string.Empty;
            }

            return $"{this.Value}{next}";
        }

        /// <summary>
        /// Gets object's hash code.
        /// </summary>
        /// <returns>returns the hash.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns the value to string.
        /// </summary>
        /// <returns>String for the word.</returns>
        public override string ToString()
        {
            return this.Value;
        }

        /// <summary>
        /// Returns true when the two objects are the same.
        /// </summary>
        /// <param name="obj">object to compare to.</param>
        /// <returns>returns true if the object is equal.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is null)
            {
                return false;
            }

            return this.Equals((Node)obj);
        }
    }
}
