using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityUtils;

namespace DecisionMaking.BehaviorTree
{
    public class NPCBehaviourTrees : NPC
    {
        public CountdownTimer StunTimer => stunTimer;

        Node tree;

        public override void Initialize(RivalsWarehouse rivalsWarehouse, Farm[] farms, List<ICharacter> characters)
        {
            SetupVariables(rivalsWarehouse, farms, characters);
            SetupTimers();
            SetupNavMeshAgent();

            agent.path = new NavMeshPath();

            tree = SetupTree();

            //tree.Evaluate();
        }

        protected Node SetupTree()
        {
            return new Selector(new List<Node>
            {
                //new Selector(new List<Node>
                //{
                //    //new StunnedNode(this),
                //    new WanderNode(this, agent, viewingRadius),
                //}),
                new Selector(new List<Node>
                {
                    new RunToFarmNode(this, agent, farms),
                    new CollectResourcesNode(this),
                    new RunToWarehouseNode(this, agent, rivalsWarehouse),
                    new GiveAwayResourcesNode(this),
                }),
            });
        }

        void Update()
        {
            stunTimer.Tick(Time.deltaTime);
            lastCollisionStopwatchTimer.Tick(Time.deltaTime);
            UpdadeteAnimator();
            tree.Evaluate();
        }

        protected override void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, viewingRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, kamikazeRadius);
        }
    }
}
