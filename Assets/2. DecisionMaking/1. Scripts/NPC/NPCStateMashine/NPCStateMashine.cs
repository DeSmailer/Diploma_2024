using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

namespace DecisionMaking.StateMashine
{
    public class NPCStateMashine : NPC
    {
        protected StateMachine stateMachine = new();

        protected NPCRunToFarmState runToFarmState;
        protected NPCCollectionState collectionState;
        protected NPCRunToWarehouseState runToWarehouseState;
        protected NPCGiveAwayResourcesState giveAwayResourcesState;
        protected NPCKamikazeState kamikazeState;
        protected NPCWanderState wanderState;
        protected NPCStunnedState stunnedState;

        public override void Initialize(RivalsWarehouse rivalsWarehouse, Farm[] farms, List<ICharacter> characters)
        {
            SetupVariables(rivalsWarehouse, farms, characters);
            SetupTimers();
            SetupNavMeshAgent();
            SetupStateMashine();
        }

        protected virtual void SetupStateMashine()
        {
            stateMachine = new StateMachine();

            runToFarmState = new NPCRunToFarmState(this, animator, agent, farms, rivalsWarehouse);
            collectionState = new NPCCollectionState(this, animator);

            runToWarehouseState = new NPCRunToWarehouseState(this, animator, agent, rivalsWarehouse);
            giveAwayResourcesState = new NPCGiveAwayResourcesState(this, animator);

            kamikazeState = new NPCKamikazeState(this, animator, otherCharacters, agent, transform, kamikazeRadius);

            wanderState = new NPCWanderState(this, animator, agent, viewingRadius);
            stunnedState = new NPCStunnedState(this, animator);


            stateMachine.AddTransition(kamikazeState, runToFarmState, new FuncPredicate(() => kamikazeState.TargetNotFound));
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

        protected override void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, viewingRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, kamikazeRadius);
        }
    }

    //сделать состояния для єой машнины конкретно для єтой. бежать к складу, феме, в стане, блуждать, камикадзе.
    //всем сделать бул исКомплит, и для рандома свтотреть когда комплит и давать новый стейт, а для тайма всеравно 
    public class NPCTimerBased : NPCStateMashine
    {
        protected StateMachine stateMachine = new();

        protected NPCRunToFarmState runToFarmState;
        protected NPCCollectionState collectionState;
        protected NPCRunToWarehouseState runToWarehouseState;
        protected NPCGiveAwayResourcesState giveAwayResourcesState;
        protected NPCKamikazeState kamikazeState;
        protected NPCWanderState wanderState;
        protected NPCStunnedState stunnedState;

        public override void Initialize(RivalsWarehouse rivalsWarehouse, Farm[] farms, List<ICharacter> characters)
        {
            SetupVariables(rivalsWarehouse, farms, characters);
            SetupTimers();
            SetupNavMeshAgent();
            SetupStateMashine();
        }

        protected virtual void SetupStateMashine()
        {
            stateMachine = new StateMachine();

            runToFarmState = new NPCRunToFarmState(this, animator, agent, farms, rivalsWarehouse);
            collectionState = new NPCCollectionState(this, animator);

            runToWarehouseState = new NPCRunToWarehouseState(this, animator, agent, rivalsWarehouse);
            giveAwayResourcesState = new NPCGiveAwayResourcesState(this, animator);

            kamikazeState = new NPCKamikazeState(this, animator, otherCharacters, agent, transform, kamikazeRadius);

            wanderState = new NPCWanderState(this, animator, agent, viewingRadius);
            stunnedState = new NPCStunnedState(this, animator);


            stateMachine.AddTransition(kamikazeState, runToFarmState, new FuncPredicate(() => kamikazeState.TargetNotFound));
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

        protected override void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, viewingRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, kamikazeRadius);
        }
    }
}