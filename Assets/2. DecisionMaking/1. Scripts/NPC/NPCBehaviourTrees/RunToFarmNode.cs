using DecisionMaking;
using DecisionMaking.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DecisionMaking.BehaviorTree
{
    public class RunToFarmNode : Node
    {
        private NPC npc;
        private NavMeshAgent agent;
        private Farm[] farms;

        public RunToFarmNode(NPC npc, NavMeshAgent agent, Farm[] farms)
        {
            this.npc = npc;
            this.agent = agent;
            this.farms = farms;
        }

        public override NodeState Evaluate()
        {
            Debug.Log("RunToFarmNode");
            if(agent.remainingDistance < agent.stoppingDistance)
            {
                Debug.Log("RunToFarmNode SUCCESS");
                return NodeState.SUCCESS;
            }

            if(!agent.hasPath)
            {
                Debug.Log("RunToFarmNode !agent.hasPath");
                Vector3 vector3 = SelectFarm();
                agent.SetDestination(vector3);
            }

            return NodeState.RUNNING;
        }

        Vector3 SelectFarm()
        {
            var selectedFarms = new List<Farm>(farms);
            selectedFarms.RemoveAll(farm => !farm.CanHarvest);

            selectedFarms.Sort((a, b) =>
             Vector3.Distance(a.transform.position, npc.transform.position)
             .CompareTo(Vector3.Distance(b.transform.position, npc.transform.position)));

            selectedFarms = selectedFarms.GetRange(0, Mathf.Min(selectedFarms.Count, 3));

            var randomFarm = selectedFarms[Random.Range(0, selectedFarms.Count)];
            Vector3 result = randomFarm.Position;
            result = RandomVector.GetRandomPointInRing(result, 0.7f, 3f);

            return result;
            //agent.SetDestination(result);
        }

        //private void SelectFarm()
        //{
        //    var nearestFarm = farms[0];
        //    float minDistance = Vector3.Distance(npc.transform.position, nearestFarm.transform.position);

        //    foreach(var farm in farms)
        //    {
        //        float distance = Vector3.Distance(npc.transform.position, farm.transform.position);
        //        if(distance < minDistance)
        //        {
        //            minDistance = distance;
        //            nearestFarm = farm;
        //        }
        //    }

        //    npc.TargetFarm = nearestFarm;
        //}
    }
}
