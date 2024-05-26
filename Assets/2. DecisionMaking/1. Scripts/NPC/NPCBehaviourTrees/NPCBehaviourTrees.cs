using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

namespace DecisionMaking.BehaviorTree
{
    public class NPCBehaviourTrees : NPC
    {
        public CountdownTimer StunTimer => stunTimer;

        public override void Initialize(RivalsWarehouse rivalsWarehouse, Farm[] farms, List<ICharacter> characters)
        {
            SetupVariables(rivalsWarehouse, farms, characters);
            SetupTimers();
            SetupNavMeshAgent();

        }

        void Update()
        {
            stunTimer.Tick(Time.deltaTime);
            lastCollisionStopwatchTimer.Tick(Time.deltaTime);
            UpdadeteAnimator();
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
