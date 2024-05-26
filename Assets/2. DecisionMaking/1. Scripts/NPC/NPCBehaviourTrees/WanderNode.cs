using UnityEngine;
using UnityEngine.AI;

namespace DecisionMaking.BehaviorTree
{
    public class WanderNode : Node
    {
        private NPC npc;
        private NavMeshAgent agent;
        private float wanderRadius;

        public WanderNode(NPCBehaviourTrees npc, NavMeshAgent agent, float wanderRadius)
        {
            this.npc = npc;
            this.agent = agent;
            this.wanderRadius = wanderRadius;
        }

        public override NodeState Evaluate()
        {
            Debug.Log("WanderNode");
            Debug.Log("remainingDistance " + agent.remainingDistance);

            if(agent.remainingDistance < agent.stoppingDistance)
            {
                Debug.Log("WanderNode SUCCESS");
                return NodeState.SUCCESS;
            }

            if(!agent.hasPath)
            {
                Debug.Log("WanderNode !hasPath");
                Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
                randomDirection += npc.transform.position;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
                agent.SetDestination(hit.position);
                npc.ResumeMovement();
            }

            return NodeState.RUNNING;
        }
    }
}

