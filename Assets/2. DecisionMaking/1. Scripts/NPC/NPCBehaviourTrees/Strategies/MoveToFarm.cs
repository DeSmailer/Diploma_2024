using UnityEngine.AI;
using UnityEngine;
using System.Collections.Generic;
using DecisionMaking.Utils;

namespace DecisionMaking.BehaviorTree
{
    public class MoveToFarm : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        Vector3 target;
        Farm[] farms;
        bool isPathCalculated;

        public MoveToFarm(Transform entity, NavMeshAgent agent, Farm[] farms)
        {
            Debug.Log(" MoveToTarget ");
            this.entity = entity;
            this.agent = agent;
            this.farms = farms;
        }

        public Node.Status Process()
        {
            agent.SetDestination(target);
            //entity.LookAt(target);

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

        void SelectFarm()
        {
            var selectedFarms = new List<Farm>(farms);
            selectedFarms.RemoveAll(farm => !farm.CanHarvest);

            selectedFarms.Sort((a, b) =>
             Vector3.Distance(a.transform.position, entity.transform.position)
             .CompareTo(Vector3.Distance(b.transform.position, entity.transform.position)));

            selectedFarms = selectedFarms.GetRange(0, Mathf.Min(selectedFarms.Count, 3));
            Debug.Log("selectedFarms.Count " + selectedFarms.Count);
            var randomFarm = selectedFarms[Random.Range(0, selectedFarms.Count)];
            Debug.Log("randomFarm " + randomFarm.name);
            Vector3 result = randomFarm.Position;
            result = RandomVector.GetRandomPointInRing(result, 0.7f, 3f);

            target = result;
        }

        public void Reset()
        {
            Debug.Log(" MoveToTarget Reset ");
            SelectFarm();
            isPathCalculated = true;
        }
    }
}
