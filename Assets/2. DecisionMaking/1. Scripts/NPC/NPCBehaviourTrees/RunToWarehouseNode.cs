using DecisionMaking;
using UnityEngine.AI;

namespace DecisionMaking.BehaviorTree
{
    public class RunToWarehouseNode : Node
    {
        private NPC npc;
        private NavMeshAgent agent;
        private RivalsWarehouse warehouse;

        public RunToWarehouseNode(NPC npc, NavMeshAgent agent, RivalsWarehouse warehouse)
        {
            this.npc = npc;
            this.agent = agent;
            this.warehouse = warehouse;
        }

        public override NodeState Evaluate()
        {
            if(agent.remainingDistance < agent.stoppingDistance)
            {
                return NodeState.SUCCESS;
            }

            if(!agent.hasPath)
            {
                agent.SetDestination(warehouse.Position);
            }

            return NodeState.RUNNING;
        }
    }
}
