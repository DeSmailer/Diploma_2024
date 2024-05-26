using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityUtils;

namespace DecisionMaking
{
    public abstract class NPC : MonoBehaviour, IStunned, ICharacter
    {
        [Header("Refarences")]
        [SerializeField] protected NavMeshAgent agent;
        [SerializeField] protected Rigidbody rb;
        [SerializeField] protected Animator animator;

        [SerializeField] protected CollisionDetector collisionDetector;

        [SerializeField] protected Inventory inventory;

        [Header("Speed")]
        [SerializeField] protected float speed = 3.5f;
        [SerializeField] protected float angularSpeed = 120f;
        [SerializeField] protected float stoppingDistance = 0.2f;

        [SerializeField] protected float stunDuration = 1.5f;
        [SerializeField] protected float delayBeforeNewCollision = 0.5f;

        [SerializeField] protected float viewingRadius = 10f;
        [SerializeField] protected float kamikazeRadius = 5f;

        protected CountdownTimer stunTimer;
        protected StopwatchTimer lastCollisionStopwatchTimer;

        [SerializeField] protected float pushForce = 3f;
        [SerializeField] protected CharacterInfo characterInfo;

        protected RivalsWarehouse rivalsWarehouse;
        protected Farm[] farms;
        protected List<ICharacter> otherCharacters;

        public virtual float Speed => speed;
        public virtual Transform Transform => transform;
        public virtual Inventory Inventory => inventory;
        public virtual CharacterInfo CharacterInfo => characterInfo;

        public UnityEvent OnStunned;

        protected static readonly int animatorParameterSpeed = Animator.StringToHash("Speed");

        public abstract void Initialize(RivalsWarehouse rivalsWarehouse, Farm[] farms, List<ICharacter> characters);

        protected virtual void SetupVariables(RivalsWarehouse rivalsWarehouse, Farm[] farms, List<ICharacter> characters)
        {
            this.rivalsWarehouse = rivalsWarehouse;
            this.farms = farms;
            otherCharacters = new List<ICharacter>(characters);
            otherCharacters.Remove(this);
        }

        protected virtual void SetupTimers()
        {
            stunTimer = new CountdownTimer(stunDuration);
            lastCollisionStopwatchTimer = new StopwatchTimer();

            lastCollisionStopwatchTimer.Start();
            stunTimer.OnTimerStop += () =>
            {
                lastCollisionStopwatchTimer.Reset();
            };
        }

        protected virtual void SetupNavMeshAgent()
        {
            agent.speed = speed;
            agent.angularSpeed = angularSpeed;
            agent.stoppingDistance = stoppingDistance;
            ResumeMovement();
        }

        public virtual void ResumeMovement() => agent.isStopped = false;

        public virtual void StopMovement() => agent.isStopped = true;

        protected virtual void UpdadeteAnimator()
        {
            animator.SetFloat(animatorParameterSpeed, Speed);
        }


        public virtual void Stun(float duration = 1)
        {
            stunTimer.Start();
            inventory.DropOnGround();
            rb.AddForce(collisionDetector.PushDirection * pushForce, ForceMode.Impulse);
            OnStunned?.Invoke();
        }

        protected virtual bool CharactersInRadius()
        {
            foreach(var character in otherCharacters)
            {
                if(Vector3.Distance(character.Transform.position, transform.position) < kamikazeRadius)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void StopAllForces()
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, viewingRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, kamikazeRadius);
        }
    }
}