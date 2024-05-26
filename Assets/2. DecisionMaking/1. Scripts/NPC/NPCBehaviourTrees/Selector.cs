using System.Collections.Generic;

namespace DecisionMaking.BehaviorTree
{
    public class Selector : Node
    {
        private List<Node> nodes = new List<Node>();

        public Selector(List<Node> nodes)
        {
            this.nodes = nodes;
        }

        public override NodeState Evaluate()
        {
            foreach(var node in nodes)
            {
                switch(node.Evaluate())
                {
                    case NodeState.SUCCESS:
                        return NodeState.SUCCESS;
                    case NodeState.RUNNING:
                        return NodeState.RUNNING;
                    case NodeState.FAILURE:
                        continue;
                    default:
                        continue;
                }
            }

            return NodeState.FAILURE;
        }
    }
}
