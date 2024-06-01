using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

namespace DecisionMaking.RandomSelectBased {
  public class NPCRandomSelectBased : NPC {
    protected StateMachine stateMachine = new();

    protected NPCRunToFarmState runToFarmState;
    protected NPCRunToWarehouseState runToWarehouseState;
    protected NPCKamikazeState kamikazeState;
    protected NPCWanderState wanderState;
    protected NPCStunnedState stunnedState;

    List<NPCBaseState> npcBaseStates = new List<NPCBaseState>();

    NPCBaseState current = null;

    public override void Initialize(RivalsWarehouse rivalsWarehouse, Farm[] farms, List<ICharacter> characters) {
      collisionDetector.OnDetected.AddListener(ToStunState);

      SetupVariables(rivalsWarehouse, farms, characters);
      SetupTimers();
      SetupNavMeshAgent();
      SetupStateMashine();
    }

    protected virtual void SetupStateMashine() {
      stateMachine = new StateMachine();

      runToFarmState = new NPCRunToFarmState(this, animator, agent, farms, rivalsWarehouse);
      stunnedState = new NPCStunnedState(this, animator, stunTimer);
      wanderState = new NPCWanderState(this, animator, agent, viewingRadius);
      runToWarehouseState = new NPCRunToWarehouseState(this, animator, agent, rivalsWarehouse);
      kamikazeState = new NPCKamikazeState(this, animator, otherCharacters, agent, transform, kamikazeRadius);

      npcBaseStates.Add(wanderState);
      npcBaseStates.Add(wanderState);
      npcBaseStates.Add(wanderState);
      npcBaseStates.Add(wanderState);
      npcBaseStates.Add(runToFarmState);
      npcBaseStates.Add(runToFarmState);
      npcBaseStates.Add(runToFarmState);
      npcBaseStates.Add(runToFarmState);
      npcBaseStates.Add(runToFarmState);
      npcBaseStates.Add(runToWarehouseState);
      npcBaseStates.Add(kamikazeState);

      current = wanderState;
      stateMachine.SetState(wanderState);
    }

    void Update() {
      stateMachine.Update();
      stunTimer.Tick(Time.deltaTime);
      lastCollisionStopwatchTimer.Tick(Time.deltaTime);
      UpdadeteAnimator();

      if (stateMachine.CurrentState == stunnedState) {
        if (stunnedState.IsComplete) {
          stateMachine.SetState(SelectNewState());
        }
      } else {
        if (current.IsComplete) {
          stateMachine.SetState(SelectNewState());
        }
      }
    }

    NPCBaseState SelectNewState() {
      NPCBaseState baseState = npcBaseStates[Random.Range(0, npcBaseStates.Count)];
      current = baseState;
      return baseState;
    }

    private void ToStunState() {
      if (collisionDetector.IsDetected && lastCollisionStopwatchTimer.GetTime() > delayBeforeNewCollision) {
        stateMachine.SetState(stunnedState);
      }
    }

    void FixedUpdate() {
      stateMachine.FixedUpdate();
    }

    protected override void OnDrawGizmosSelected() {
      Gizmos.color = Color.blue;
      Gizmos.DrawWireSphere(transform.position, viewingRadius);

      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(transform.position, kamikazeRadius);
    }
  }
}
