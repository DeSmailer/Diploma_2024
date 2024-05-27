using UnityEngine.AI;
using UnityEngine;

namespace DecisionMaking.BehaviorTree
{
    public class Wander : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        Vector3 target;
        bool isPathCalculated;
        float wanderRadius;

        public Wander(Transform entity, NavMeshAgent agent, float wanderRadius)
        {
            Debug.Log(" Wander ");
            this.entity = entity;
            this.agent = agent;
            this.wanderRadius = wanderRadius;
            SelectNewDestination();
        }

        public Node.Status Process()
        {
            agent.SetDestination(target);
            //entity.LookAt(target);

            if(isPathCalculated && agent.remainingDistance < 0.1f)
            {
                isPathCalculated = false;
                Debug.Log(" Wander " + Node.Status.Success);
                return Node.Status.Success;
            }

            if(agent.pathPending)
            {
                isPathCalculated = true;
            }

            return Node.Status.Running;
        }

        void SelectNewDestination()
        {
            var randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += entity.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
            var finalPosition = hit.position;

            target = finalPosition;
        }

        public void Reset()
        {
            Debug.Log(" MoveToTarget Reset ");
            SelectNewDestination();
            isPathCalculated = true;
        }
    }
}
