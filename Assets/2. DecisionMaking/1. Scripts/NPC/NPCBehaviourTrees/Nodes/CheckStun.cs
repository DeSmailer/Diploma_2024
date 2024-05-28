using DecisionMaking.StateMashine;
using DecisionMaking.Utils;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityUtils;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace DecisionMaking.BehaviorTree
{
    public class CheckStun : Node
    {
        private CollisionDetector collisionDetector;
        private CountdownTimer stunTimer;

        public CheckStun(CollisionDetector collisionDetector, CountdownTimer stunTimer)
        {
            this.collisionDetector = collisionDetector;
            this.stunTimer = stunTimer;
        }

        public override NodeState Evaluate()
        {
            Debug.Log("CheckStun 1");
            if(collisionDetector.IsDetected)
            {
                Debug.Log("CheckStun 2");
                state = NodeState.SUCCESS;
                return state;
            }
            else if(stunTimer.IsRunning)
            {
                state = NodeState.SUCCESS;
                return state;
            }

            Debug.Log("CheckStun 3");
            state = NodeState.FAILURE;
            return state;
        }
    }

    public class Stun : Node
    {
        private NPCBehaviourTrees npc;
        private Animator animator;
        private CountdownTimer stunTimer;

        public Stun(NPCBehaviourTrees npc, Animator animator, CountdownTimer stunTimer)
        {
            this.npc = npc;
            this.animator = animator;
            this.stunTimer = stunTimer;
        }

        public override NodeState Evaluate()
        {
            Debug.Log("Stun");
            if(!stunTimer.IsRunning)
            {
                Enter();
            }

            Debug.Log("Stun 2");
            animator.Play(stunnedHash);
            state = NodeState.RUNNING;

            Debug.Log("Stun 3");
            return state;
        }

        public void Enter()
        {
            Debug.Log("Stun 1");
            animator.Play(stunnedHash);
            npc.Stun();
            npc.StopMovement();
        }

    }

    public class GoToTarget : Node
    {
        private Vector3 target;
        private NavMeshAgent agent;
        private Animator animator;
        private NPCBehaviourTrees npc;

        public GoToTarget(NPCBehaviourTrees npc, Vector3 target, Animator animator, NavMeshAgent agent)
        {
            this.agent = agent;
            this.target = target;
            this.animator = animator;
            this.npc = npc;
        }

        public override NodeState Evaluate()
        {
            Resume();

            Debug.Log("GoToTarget");
            agent.SetDestination(target);
            animator.Play(locomotionHash);

            state = NodeState.RUNNING;
            return state;
        }

        public void Resume()
        {
            Debug.Log("GoToTarget 3");
            npc.ResumeMovement();
            npc.StopAllForces();
        }
    }

    public class CheckNeedRunToFarm : Node
    {
        private NPCBehaviourTrees npc;
        private float percent;
        private Farm[] farms;

        public CheckNeedRunToFarm(NPCBehaviourTrees npc, float percent, Farm[] farms)
        {
            this.npc = npc;
            this.percent = percent;
            this.farms = farms;
        }

        public override NodeState Evaluate()
        {
            int c = 0;
            foreach(var farm in farms)
            {
                if(farm.CanHarvest)
                {
                    c++;
                }
            }
            if(c == 0)
            {
                state = NodeState.FAILURE;
                return state;
            }

            Debug.Log("CheckNeedRunToFarm 1");
            if(npc.Inventory.FillPercentage < percent)
            {
                Debug.Log("CheckNeedRunToFarm 2");
                state = NodeState.SUCCESS;
                return state;
            }

            Debug.Log("CheckNeedRunToFarm 3");
            state = NodeState.FAILURE;
            return state;
        }
    }

    public class GoToFarm : Node
    {
        private NavMeshAgent agent;
        private Animator animator;
        NPCBehaviourTrees npc;
        Farm[] farms;
        Farm targetFarm;
        Vector3 target = Vector3.zero;

        public GoToFarm(NPCBehaviourTrees npc, Animator animator, NavMeshAgent agent, Farm[] farms)
        {
            this.agent = agent;
            this.animator = animator;
            this.npc = npc;
            this.farms = farms;
        }

        public override NodeState Evaluate()
        {
            Debug.Log("GoToFarm");
            Resume();

            //Debug.Log("GoToFarm 2 " + targetFarm);
            //Debug.Log("GoToFarm 2 " + targetFarm.CanHarvest);
            //Debug.Log("GoToFarm 2 " + target);
            agent.SetDestination(target);

            if(targetFarm == null || !targetFarm.CanHarvest)
            {
                targetFarm = SelectFarm();
                target = GetRandomPoint(targetFarm.transform);
                agent.SetDestination(target);
                animator.Play(locomotionHash);
            }

            state = NodeState.RUNNING;
            return state;
        }

        public void Resume()
        {
            Debug.Log("GoToFarm 3");
            npc.ResumeMovement();
            npc.StopAllForces();
        }

        Farm SelectFarm()
        {
            var selectedFarms = new List<Farm>(farms);
            selectedFarms.RemoveAll(farm => !farm.CanHarvest);

            selectedFarms.Sort((a, b) =>
             Vector3.Distance(a.transform.position, npc.transform.position)
             .CompareTo(Vector3.Distance(b.transform.position, npc.transform.position)));

            selectedFarms = selectedFarms.GetRange(0, Mathf.Min(selectedFarms.Count, 3));

            var randomFarm = selectedFarms[Random.Range(0, selectedFarms.Count)];
            return randomFarm;
        }

        Vector3 GetRandomPoint(Transform farm)
        {
            Vector3 result = farm.position;
            result = RandomVector.GetRandomPointInRing(result, 0.7f, 2f);
            return result;
        }
    }

    public class CheckNeedRunToWarehouse : Node
    {
        private NPCBehaviourTrees npc;
        private float percent;

        public CheckNeedRunToWarehouse(NPCBehaviourTrees npc, float percent)
        {
            this.npc = npc;
            this.percent = percent;
        }

        public override NodeState Evaluate()
        {
            Debug.Log("CheckNeedRunToWarehouse 1");
            if(npc.Inventory.FillPercentage >= percent)
            {
                Debug.Log("CheckNeedRunToWarehouse 2");
                state = NodeState.SUCCESS;
                return state;
            }

            Debug.Log("CheckNeedRunToWarehouse 3");
            state = NodeState.FAILURE;
            return state;
        }
    }

    public class CantDoAnything : Node
    {
        Farm[] farms;

        public CantDoAnything(Farm[] farms)
        {
            this.farms = farms;
        }

        public override NodeState Evaluate()
        {
            int c = 0;
            foreach(var farm in farms)
            {
                if(farm.CanHarvest)
                {
                    c++;
                }
            }
            if(c == 0)
            {
                Debug.Log("CantDoAnything 1");
                state = NodeState.SUCCESS;
                return state;
            }

            Debug.Log("CantDoAnything 3");
            state = NodeState.FAILURE;
            return state;
        }
    }
}
