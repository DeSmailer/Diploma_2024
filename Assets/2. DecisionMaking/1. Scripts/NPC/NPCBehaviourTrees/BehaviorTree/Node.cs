using System.Collections.Generic;
using UnityEngine;

namespace DecisionMaking.BehaviorTree
{
    public class Node
    {
        protected static readonly int idleHash = Animator.StringToHash("Idle");
        protected static readonly int locomotionHash = Animator.StringToHash("Locomotion");
        protected static readonly int stunnedHash = Animator.StringToHash("Stunned");
        protected static readonly int KamikazeHash = Animator.StringToHash("Kamikaze");

        protected NodeState state;

        public Node parent;
        protected List<Node> children = new List<Node>();

        public Node()
        {
            parent = null;
        }

        public Node(List<Node> children)
        {
            foreach(Node child in children)
                Attach(child);
        }

        private void Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE;
    }
}
