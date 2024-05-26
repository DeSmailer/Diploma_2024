using DecisionMaking;

namespace DecisionMaking.BehaviorTree
{
    public class GiveAwayResourcesNode : Node
    {
        private NPC npc;

        public GiveAwayResourcesNode(NPC npc)
        {
            this.npc = npc;
        }

        public override NodeState Evaluate()
        {
            //npc.GiveAwayResources();

            if(npc.Inventory.FillPercentage == 0)
            {
                return NodeState.SUCCESS;
            }

            return NodeState.RUNNING;
        }
    }
}
