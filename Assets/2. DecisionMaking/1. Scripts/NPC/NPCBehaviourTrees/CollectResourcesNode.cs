namespace DecisionMaking.BehaviorTree
{
    public class CollectResourcesNode : Node
    {
        private NPC npc;

        public CollectResourcesNode(NPC npc)
        {
            this.npc = npc;
        }

        public override NodeState Evaluate()
        {
            //npc.CollectResources();

            if(!npc.Inventory.CanGetResources)
            {
                return NodeState.SUCCESS;
            }

            return NodeState.RUNNING;
        }
    }
}

