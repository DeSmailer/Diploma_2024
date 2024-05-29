using System.Collections.Generic;
using DecisionMaking.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace DecisionMaking.StateMashine
{
    public class NPCRunToFarmState : NPCBaseState
    {
        NavMeshAgent agent;
        Farm[] farms;
        Farm selectedFarm;
        RivalsWarehouse rivalsWarehouse;

        public bool IsComplete { get; private set; }

        public NPCRunToFarmState(NPCStateMashine npc, Animator animator, NavMeshAgent agent, Farm[] farms, RivalsWarehouse rivalsWarehouse) : base(npc, animator)
        {
            this.agent = agent;
            this.farms = farms;
            this.rivalsWarehouse = rivalsWarehouse;
        }

        public override void OnEnter()
        {
            IsComplete = false;
            animator.CrossFade(locomotionHash, crossFadeDuration);
            SelectFarm();
        }

        public override void OnUpdate()
        {
            if(HasReachedDestination())
            {
                IsComplete = true;
            }
            if(!selectedFarm.CanHarvest)
            {
                IsComplete = true;
                //SelectFarm();
            }
        }

        bool HasReachedDestination()
        {
            return !agent.pathPending
                   && agent.remainingDistance <= agent.stoppingDistance
                   && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        }

        void SelectFarm()
        {
            IFarmSelector farmSelector = new RequiredResourceFarmSelector(farms, npc, npc.transform, rivalsWarehouse);
            //IFarmSelector farmSelector = new RandomFromClosestFarmSelector(farms, npc.transform, 3);
            selectedFarm = farmSelector.SelectFarm();
            Vector3 result = GetPositionInRadius(selectedFarm.transform);

            agent.SetDestination(result);
        }

        public Vector3 GetPositionInRadius(Transform center, float innerRadius = 0.1f, float outerRadius = 2f)
        {
            Vector3 result = RandomVector.GetRandomPointInRing(center.position, innerRadius, outerRadius);
            return result;
        }
    }
}