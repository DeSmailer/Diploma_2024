using UnityEngine;

namespace DecisionMaking.BehaviorTree
{
    public class StunnedNode : Node
    {
        private NPCBehaviourTrees npc;

        public StunnedNode(NPCBehaviourTrees npc)
        {
            this.npc = npc;
        }

        public override NodeState Evaluate()
        {
           Debug.Log("StunnedNode");

            npc.Stun();

            if(!npc.StunTimer.IsFinished)
            {
                return NodeState.SUCCESS;
            }

            return NodeState.RUNNING;
        }
    }
}

