using UnityEngine;
using UnityEngine.AI;

namespace DecisionMaking.BehaviorTree
{
    public class WanderNode : Node
    {
        private NPC npc;
        private NavMeshAgent agent;
        private float wanderRadius;

        public WanderNode(NPC npc, NavMeshAgent agent, float wanderRadius)
        {
            this.npc = npc;
            this.agent = agent;
            this.wanderRadius = wanderRadius;
        }

        public override NodeState Evaluate()
        {
            if(agent.remainingDistance < agent.stoppingDistance)
            {
                return NodeState.SUCCESS;
            }

            if(!agent.hasPath)
            {
                Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
                randomDirection += npc.transform.position;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
                agent.SetDestination(hit.position);
            }

            return NodeState.RUNNING;
        }
    }
}

