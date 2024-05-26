using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityUtils;

namespace FarmRunner
{
    public class NPC : MonoBehaviour, IStunned, ICharacter
    {

        [Header("Refarences")]
        [SerializeField] protected NavMeshAgent agent;
        [SerializeField] Rigidbody rb;
        [SerializeField] Animator animator;

        [SerializeField] CollisionDetector collisionDetector;

        [SerializeField] Inventory inventory;

        [Header("Speed")]
        [SerializeField] protected float speed = 3.5f;
        [SerializeField] protected float angularSpeed = 120f;
        [SerializeField] protected float stoppingDistance = 0.2f;

        [SerializeField] float stunDuration = 1.5f;
        [SerializeField] float delayBeforeNewCollision = 0.5f;

        [SerializeField] protected float viewingRadius = 10f;
        [SerializeField] protected float kamikazeRadius = 5f;

        CountdownTimer stunTimer;
        StopwatchTimer lastCollisionStopwatchTimer;

        [SerializeField] float pushForce = 3f;
        [SerializeField] CharacterInfo characterInfo;

        RivalsWarehouse rivalsWarehouse;
        Farm[] farms;
        List<ICharacter> otherCharacters;

        StateMachine stateMachine = new();

        NPCRunToFarmState runToFarmState;
        NPCCollectionState collectionState;
        NPCRunToWarehouseState runToWarehouseState;
        NPCGiveAwayResourcesState giveAwayResourcesState;
        NPCKamikazeState kamikazeState;
        NPCWanderState wanderState;
        NPCStunnedState stunnedState;

        public float Speed => speed;
        public Transform Transform => transform;
        public Inventory Inventory => inventory;
        public CharacterInfo CharacterInfo => characterInfo;

        public UnityEvent OnStunned;

        static readonly int animatorParameterSpeed = Animator.StringToHash("Speed");

        public void Initialize(RivalsWarehouse rivalsWarehouse, Farm[] farms, List<ICharacter> characters)
        {
            SetupVariables(rivalsWarehouse, farms, characters);
            SetupTimers();
            SetupNavMeshAgent();
            SetupStateMashine();
        }

        protected void SetupVariables(RivalsWarehouse rivalsWarehouse, Farm[] farms, List<ICharacter> characters)
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

        protected virtual void SetupStateMashine()
        {
            stateMachine = new StateMachine();

            runToFarmState = new NPCRunToFarmState(this, animator, agent, farms);
            collectionState = new NPCCollectionState(this, animator);

            runToWarehouseState = new NPCRunToWarehouseState(this, animator, agent, rivalsWarehouse);
            giveAwayResourcesState = new NPCGiveAwayResourcesState(this, animator);

            kamikazeState = new NPCKamikazeState(this, animator, otherCharacters, agent, transform, kamikazeRadius);

            wanderState = new NPCWanderState(this, animator, agent, viewingRadius);
            stunnedState = new NPCStunnedState(this, animator);



            stateMachine.AddTransition(giveAwayResourcesState, kamikazeState, new FuncPredicate(() => runToWarehouseState.IsComplete && CharactersInRadius()));
            stateMachine.AddTransition(giveAwayResourcesState, runToFarmState, new FuncPredicate(() => runToWarehouseState.IsComplete && !CharactersInRadius()));
            stateMachine.AddTransition(runToWarehouseState, giveAwayResourcesState, new FuncPredicate(() => runToWarehouseState.IsComplete));
            stateMachine.AddTransition(collectionState, runToWarehouseState, new FuncPredicate(() => collectionState.IsComplete && inventory.FillPercentage >= 0.7));
            stateMachine.AddTransition(collectionState, runToFarmState, new FuncPredicate(() => collectionState.IsComplete && inventory.FillPercentage < 0.7));
            stateMachine.AddTransition(runToFarmState, collectionState, new FuncPredicate(() => runToFarmState.IsComplete));
            stateMachine.AddTransition(wanderState, runToFarmState, new FuncPredicate(() => wanderState.IsComplete && inventory.FillPercentage < 50));
            stateMachine.AddTransition(stunnedState, wanderState, new FuncPredicate(() => stunTimer.IsFinished));

            stateMachine.AddTransition(kamikazeState, stunnedState,
                new FuncPredicate(() => collisionDetector.IsDetected && lastCollisionStopwatchTimer.GetTime() > delayBeforeNewCollision));
            stateMachine.AddAnyTransition(stunnedState,
               new FuncPredicate(() => collisionDetector.IsDetected && lastCollisionStopwatchTimer.GetTime() > delayBeforeNewCollision));


            stateMachine.SetState(wanderState);
        }

        void Update()
        {
            stateMachine.Update();
            stunTimer.Tick(Time.deltaTime);
            lastCollisionStopwatchTimer.Tick(Time.deltaTime);
            UpdadeteAnimator();
        }

        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        public void ResumeMovement() => agent.isStopped = false;

        public void StopMovement() => agent.isStopped = true;

        void UpdadeteAnimator()
        {
            animator.SetFloat(animatorParameterSpeed, Speed);
        }


        public void Stun(float duration = 1)
        {
            stunTimer.Start();
            inventory.DropOnGround();
            rb.AddForce(collisionDetector.PushDirection * pushForce, ForceMode.Impulse);
            OnStunned?.Invoke();
        }

        protected bool CharactersInRadius()
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

        public void StopAllForces()
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