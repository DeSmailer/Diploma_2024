using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

namespace DecisionMaking.BehaviorTree
{
    public class NPCBehaviourTrees : NPC
    {
        [SerializeField] GameObject safeSpot;
        [SerializeField] GameObject treasure;
        [SerializeField] GameObject treasure2;

        Node tree;

        public CountdownTimer StunTimer => stunTimer;

        public override void Initialize(RivalsWarehouse rivalsWarehouse, Farm[] farms, List<ICharacter> characters)
        {
            collisionDetector.OnDetected.AddListener(Stun);

            SetupVariables(rivalsWarehouse, farms, characters);
            SetupTimers();
            SetupNavMeshAgent();
            SetupTree();
        }

        private void Stun()
        {
            Stun(1f);
        }

        void SetupTree()
        {
            tree = new Selector(new List<Node>
            {
                new Sequence(new List<Node>
                {
                    new CheckStun(collisionDetector, stunTimer),
                    new Stun(this, animator,  stunTimer),
                }),
                new Sequence(new List<Node>
                {
                    new GoToTarget(safeSpot.transform ,animator, agent),
                    //new TaskGoToTarget(transform),
                }),
                //new TaskPatrol(transform, waypoints),
            });
        }

        void Update()
        {
            stunTimer.Tick(Time.deltaTime);
            lastCollisionStopwatchTimer.Tick(Time.deltaTime);
            UpdadeteAnimator();
            tree.Evaluate();
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
    }
}
