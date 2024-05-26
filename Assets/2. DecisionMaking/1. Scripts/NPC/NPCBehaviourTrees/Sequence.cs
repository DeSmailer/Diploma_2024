using System.Collections.Generic;

namespace DecisionMaking.BehaviorTree
{
    public class Sequence : Node
    {
        private List<Node> nodes = new List<Node>();

        public Sequence(List<Node> nodes)
        {
            this.nodes = nodes;
        }

        public override NodeState Evaluate()
        {
            bool anyChildRunning = false;

            foreach(var node in nodes)
            {
                switch(node.Evaluate())
                {
                    case NodeState.FAILURE:
                        return NodeState.FAILURE;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildRunning = true;
                        continue;
                    default:
                        return NodeState.SUCCESS;
                }
            }

            return anyChildRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        }
    }
}
