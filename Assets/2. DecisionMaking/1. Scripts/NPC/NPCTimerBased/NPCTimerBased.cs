using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

namespace DecisionMaking.TimerBased
{
    public partial class NPCTimerBased : NPC
    {
        protected StateMachine stateMachine = new();

        protected NPCRunToFarmState runToFarmState;
        protected NPCRunToWarehouseState runToWarehouseState;
        protected NPCKamikazeState kamikazeState;
        protected NPCWanderState wanderState;
        protected NPCStunnedState stunnedState;

        List<NPCBaseState> npcBaseStates = new List<NPCBaseState>();

        [SerializeField] float changeStateInterval = 9f;
        CountdownTimer changeStateTimer;

        int index = 0;

        public override void Initialize(RivalsWarehouse rivalsWarehouse, Farm[] farms, List<ICharacter> characters)
        {
            collisionDetector.OnDetected.AddListener(ToStunState);

            SetupVariables(rivalsWarehouse, farms, characters);
            SetupTimers();
            SetupNavMeshAgent();
            SetupStateMashine();
        }

        protected override void SetupTimers()
        {
            base.SetupTimers();
            changeStateTimer = new CountdownTimer(changeStateInterval);
        }

        protected virtual void SetupStateMashine()
        {
            stateMachine = new StateMachine();

            runToFarmState = new NPCRunToFarmState(this, animator, agent, farms, rivalsWarehouse);
            stunnedState = new NPCStunnedState(this, animator, stunTimer);
            wanderState = new NPCWanderState(this, animator, agent, viewingRadius);
            runToWarehouseState = new NPCRunToWarehouseState(this, animator, agent, rivalsWarehouse);
            kamikazeState = new NPCKamikazeState(this, animator, otherCharacters, agent, transform, kamikazeRadius);

            npcBaseStates.Add(wanderState);
            npcBaseStates.Add(runToFarmState);
            npcBaseStates.Add(runToWarehouseState);
            npcBaseStates.Add(kamikazeState);

            stateMachine.SetState(wanderState);
        }

        void Update()
        {
            stateMachine.Update();
            stunTimer.Tick(Time.deltaTime);
            changeStateTimer.Tick(Time.deltaTime);
            lastCollisionStopwatchTimer.Tick(Time.deltaTime);
            UpdadeteAnimator();

            if(stateMachine.CurrentState == stunnedState)
            {
                if(stunnedState.IsComplete)
                {
                    stateMachine.SetState(SelectNewState());
                }
            }
            else
            {
                if(changeStateTimer.IsFinished)
                {
                    stateMachine.SetState(SelectNewState());
                }
            }
        }

        NPCBaseState SelectNewState()
        {
            changeStateTimer.Start();
            int i = index % npcBaseStates.Count;
            NPCBaseState state = npcBaseStates[i];
            index++;
            return state;
            //return npcBaseStates[UnityEngine.Random.Range(0, npcBaseStates.Count)];
        }

        private void ToStunState()
        {
            if(collisionDetector.IsDetected && lastCollisionStopwatchTimer.GetTime() > delayBeforeNewCollision)
            {
                changeStateTimer.Pause();
                stateMachine.SetState(stunnedState);
            }
        }

        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
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


