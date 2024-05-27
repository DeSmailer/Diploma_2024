using DecisionMaking.StateMashine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityUtils;

namespace DecisionMaking.BehaviorTree
{
    public class NPCBehaviourTrees : NPC
    {
        [SerializeField] GameObject safeSpot;
        [SerializeField] GameObject treasure;
        [SerializeField] GameObject treasure2;
        [SerializeField] List<Transform> waypoints;

        [SerializeField] bool isSafe;

        public CountdownTimer StunTimer => stunTimer;
        BehaviourTree tree;

        public override void Initialize(RivalsWarehouse rivalsWarehouse, Farm[] farms, List<ICharacter> characters)
        {
            SetupVariables(rivalsWarehouse, farms, characters);
            SetupTimers();
            SetupNavMeshAgent();
            SetupBehaviourTree();
        }

        void Update()
        {
            stunTimer.Tick(Time.deltaTime);
            lastCollisionStopwatchTimer.Tick(Time.deltaTime);
            UpdadeteAnimator();
            tree.Process();

            if(Input.GetKeyDown(KeyCode.Space))
            {
                tree.Reset();
            }
        }

        public override void Stun(float duration = 1)
        {
            stunTimer.Start();
            inventory.DropOnGround();
            rb.AddForce(collisionDetector.PushDirection * pushForce, ForceMode.Impulse);
            OnStunned?.Invoke();
        }

        protected override void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, viewingRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, kamikazeRadius);
        }


        //protected NPCRunToFarmState runToFarmState;
        //protected NPCCollectionState collectionState;
        //protected NPCRunToWarehouseState runToWarehouseState;
        //protected NPCGiveAwayResourcesState giveAwayResourcesState;
        //protected NPCKamikazeState kamikazeState;
        //protected NPCWanderState wanderState;
        //protected NPCStunnedState stunnedState;


        void SetupBehaviourTree()
        {
            tree = new BehaviourTree("NPC");

            PrioritySelector actions = new PrioritySelector("Agent Logic");

            bool CanHarvest()
            {
                foreach(var farm in farms)
                {
                    if(farm.CanHarvest)
                    {
                        return true;
                    }
                }
                return false;
            };

            Sequence goToFarm = new Sequence("GoToFarm");
            goToFarm.AddChild(new Leaf("CanCollect?", new Condition(() => inventory.FillPercentage < 0.5f && CanHarvest())));
            goToFarm.AddChild(new Leaf("GoToFarm", new MoveToFarm(transform, agent, farms)));
            //goToFarm.AddChild(new Leaf("PickUpTreasure1", new ActionStrategy(() => treasure.SetActive(false))));
            actions.AddChild(goToFarm);

            Sequence goToWarehouse = new Sequence("GoToWarehouse");
            goToFarm.AddChild(new Leaf("canGiveAway?", new Condition(() => inventory.FillPercentage >= 0.5f || !CanHarvest())));
            goToFarm.AddChild(new Leaf("GoWarehouse", new MoveToTarget(transform, agent, rivalsWarehouse.transform)));
            actions.AddChild(goToWarehouse);

            tree.AddChild(actions);


            //PrioritySelector actions = new PrioritySelector("Agent Logic");

            //Sequence runToSafetySeq = new Sequence("RunToSafety", 100);
            //bool IsSafe()
            //{
            //    if(isSafe)
            //    {
            //        if(!isSafe)
            //        {
            //            runToSafetySeq.Reset();
            //            return true;
            //        }
            //    }

            //    return false;
            //}


            //runToSafetySeq.AddChild(new Leaf("isSafe?", new Condition(IsSafe)));
            //runToSafetySeq.AddChild(new Leaf("Go To Safety", new MoveToTarget(transform, agent, safeSpot.transform)));
            //actions.AddChild(runToSafetySeq);

            //PrioritySelector goToTreasure = new RandomSelector("GoToTreasure");
            //Sequence getTreasure1 = new Sequence("GetTreasure1");
            //getTreasure1.AddChild(new Leaf("isTreasure1?", new Condition(() => treasure.activeSelf)));
            //getTreasure1.AddChild(new Leaf("GoToTreasure1", new MoveToTarget(transform, agent, treasure.transform)));
            //getTreasure1.AddChild(new Leaf("PickUpTreasure1", new ActionStrategy(() => treasure.SetActive(false))));
            //goToTreasure.AddChild(getTreasure1);

            //Sequence getTreasure2 = new Sequence("GetTreasure2");
            //getTreasure2.AddChild(new Leaf("isTreasure2?", new Condition(() => treasure2.activeSelf)));
            //getTreasure2.AddChild(new Leaf("GoToTreasure2", new MoveToTarget(transform, agent, treasure2.transform)));
            //getTreasure2.AddChild(new Leaf("PickUpTreasure2", new ActionStrategy(() => treasure2.SetActive(false))));
            //goToTreasure.AddChild(getTreasure2);

            //actions.AddChild(goToTreasure);

            //Leaf patrol = new Leaf("Patrol", new PatrolStrategy(transform, agent, waypoints));
            //actions.AddChild(patrol);




            tree.AddChild(actions);

        }



        //void OnClick(RaycastHit hit)
        //{
        //    if(UnityEngine.AI.NavMesh.SamplePosition(hit.point, out UnityEngine.AI.NavMeshHit navHit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
        //    {
        //        agent.SetDestination(navHit.position);
        //    }
        //}
    }


}
