using UnityEngine.AI;
using UnityEngine;

namespace DecisionMaking.BehaviorTree
{
    public class MoveToTarget : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        readonly Transform points;
        bool isPathCalculated;

        public MoveToTarget(Transform entity, NavMeshAgent agent, Transform points)
        {
            Debug.Log(" MoveToTarget ");
            this.entity = entity;
            this.agent = agent;
            this.points = points;
        }

        public Node.Status Process()
        {
            agent.SetDestination(points.position);
            entity.LookAt(points);

            if(isPathCalculated && agent.remainingDistance < 0.1f)
            {
                isPathCalculated = false;
                Debug.Log(" Node.Status.Success " + Node.Status.Success);
                return Node.Status.Success;
            }

            if(agent.pathPending)
            {
                isPathCalculated = true;
            }

            return Node.Status.Running;
        }

        public void Reset()
        {
            Debug.Log(" MoveToTarget Reset ");
            isPathCalculated = true;
        }
    }
}
