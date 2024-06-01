using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

namespace DecisionMaking.BehaviorTree
{
    public class NPCBehaviourTrees : NPC
    {
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

        protected override void SetupTimers()
        {
            base.SetupTimers();
            stunTimer.OnTimerStop += () =>
            {
                ResumeMovement();
                StopAllForces();
            };
        }

        private void Stun()
        {
            stunTimer.Start();
            inventory.DropOnGround();
            StopMovement();
            rb.AddForce(collisionDetector.PushDirection * pushForce, ForceMode.Impulse);
            OnStunned?.Invoke();
        }

        void SetupTree()
        {
            tree = new Selector(new List<Node>
            {
                new Sequence(new List<Node>
                {
                    new CheckStun(collisionDetector, stunTimer),
                    new Stun(this, animator,  stunTimer)
                }),
                new Sequence(new List<Node>
                {
                    new CheckNeedRunToFarm(this, 0.7f,farms),
                    new GoToFarm(this, animator, agent, farms)
                }),
                new Sequence(new List<Node>
                {
                    new CheckNeedRunToWarehouse(this, 0.7f),
                    new GoToTarget(this, rivalsWarehouse.Position, animator, agent)
                }),
                new Sequence(new List<Node>
                {
                    new CantDoAnything(farms),
                    new GoToTarget(this, rivalsWarehouse.Position, animator, agent)
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
