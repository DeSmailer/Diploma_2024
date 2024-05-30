using DecisionMaking.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityUtils;

namespace DecisionMaking.TimerBased {//сделать состояния для єой машнины конкретно для єтой. бежать к складу, феме, в стане, блуждать, камикадзе.
                                     //всем сделать бул исКомплит, и для рандома свтотреть когда комплит и давать новый стейт, а для тайма всеравно
  public class NPCTimerBased : NPC {
    protected StateMachine stateMachine = new();

    protected NPCRunToFarmState runToFarmState;
    protected NPCRunToWarehouseState runToWarehouseState;
    protected NPCKamikazeState kamikazeState;
    protected NPCWanderState wanderState;
    protected NPCStunnedState stunnedState;

    List<NPCBaseState> npcBaseStates = new List<NPCBaseState>();

    float changeStateInterval = 4f;
    CountdownTimer changeStateTimer;

    int index = 0;

    public override void Initialize(RivalsWarehouse rivalsWarehouse, Farm[] farms, List<ICharacter> characters) {
      collisionDetector.OnDetected.AddListener(ToStunState);

      SetupVariables(rivalsWarehouse, farms, characters);
      SetupTimers();
      SetupNavMeshAgent();
      SetupStateMashine();
    }

    protected override void SetupTimers() {
      base.SetupTimers();
      changeStateTimer = new CountdownTimer(changeStateInterval);
    }

    protected virtual void SetupStateMashine() {
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
      //runToWarehouseState = new StateMashine. NPCRunToWarehouseState(this, animator, agent, rivalsWarehouse);

      //kamikazeState = new StateMashine. NPCKamikazeState(this, animator, otherCharacters, agent, transform, kamikazeRadius);

      //wanderState = new StateMashine. NPCWanderState(this, animator, agent, viewingRadius);
      //stunnedState = new StateMashine.NPCStunnedState(this, animator);


      //stateMachine.AddTransition(kamikazeState, runToFarmState, new FuncPredicate(() => kamikazeState.TargetNotFound));
      //stateMachine.AddTransition(giveAwayResourcesState, kamikazeState, new FuncPredicate(() => runToWarehouseState.IsComplete && CharactersInRadius()));
      //stateMachine.AddTransition(giveAwayResourcesState, runToFarmState, new FuncPredicate(() => runToWarehouseState.IsComplete && !CharactersInRadius()));
      //stateMachine.AddTransition(runToWarehouseState, giveAwayResourcesState, new FuncPredicate(() => runToWarehouseState.IsComplete));
      //stateMachine.AddTransition(collectionState, runToWarehouseState, new FuncPredicate(() => collectionState.IsComplete && inventory.FillPercentage >= 0.7));
      //stateMachine.AddTransition(collectionState, runToFarmState, new FuncPredicate(() => collectionState.IsComplete && inventory.FillPercentage < 0.7));
      //stateMachine.AddTransition(runToFarmState, collectionState, new FuncPredicate(() => runToFarmState.IsComplete));
      //stateMachine.AddTransition(wanderState, runToFarmState, new FuncPredicate(() => wanderState.IsComplete && inventory.FillPercentage < 50));
      //stateMachine.AddTransition(stunnedState, wanderState, new FuncPredicate(() => stunTimer.IsFinished));

      //stateMachine.AddTransition(kamikazeState, stunnedState,
      //    new FuncPredicate(() => collisionDetector.IsDetected && lastCollisionStopwatchTimer.GetTime() > delayBeforeNewCollision));
      //stateMachine.AddAnyTransition(stunnedState,
      //   new FuncPredicate(() => collisionDetector.IsDetected && lastCollisionStopwatchTimer.GetTime() > delayBeforeNewCollision));


      stateMachine.SetState(wanderState);
    }

    void Update() {
      stateMachine.Update();
      stunTimer.Tick(Time.deltaTime);
      changeStateTimer.Tick(Time.deltaTime);
      lastCollisionStopwatchTimer.Tick(Time.deltaTime);
      UpdadeteAnimator();

      if (stateMachine.CurrentState == stunnedState) {
        if (stunnedState.IsComplete) {
          stateMachine.SetState(SelectNewState());
        }
      } else {
        if (changeStateTimer.IsFinished) {
          stateMachine.SetState(SelectNewState());
        }
      }
    }

    NPCBaseState SelectNewState() {
      Debug.Log("SelectNewState");
      changeStateTimer.Start();
      int i = index % npcBaseStates.Count;
      Debug.Log(i);
      NPCBaseState state = npcBaseStates[i];
      Debug.Log(state.GetType().Name);
      index++;
      return state;
      //return npcBaseStates[UnityEngine.Random.Range(0, npcBaseStates.Count)];
    }

    private void ToStunState() {
      if (collisionDetector.IsDetected && lastCollisionStopwatchTimer.GetTime() > delayBeforeNewCollision) {
        changeStateTimer.Pause();
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

    public class NPCKamikazeState : NPCBaseState {
      public bool TargetNotFound;

      List<ICharacter> characters;
      NavMeshAgent agent;
      Transform target;
      float kamikazeRadius;
      Transform transform;

      float oldStoppingDistance;

      public NPCKamikazeState(NPCTimerBased npc, Animator animator, List<ICharacter> characters, NavMeshAgent agent, Transform transform, float kamikazeRadius) : base(npc, animator) {
        this.characters = characters;
        this.agent = agent;
        this.transform = transform;
        this.kamikazeRadius = kamikazeRadius;

        oldStoppingDistance = agent.stoppingDistance;
        agent.stoppingDistance = 0.0f;
      }

      public override void OnEnter() {
        TargetNotFound = false;
        animator.CrossFade(KamikazeHash, crossFadeDuration);
        target = SelectTarget();
      }

      public override void OnUpdate() {
        agent.SetDestination(target.position);
        if (HasReachedDestination()) {
          target = SelectTarget();
        }
      }
      bool HasReachedDestination() {
        return !agent.pathPending
               && agent.remainingDistance <= agent.stoppingDistance
               && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
      }

      public override void OnExit() {
        base.OnExit();
        npc.StopAllForces();
        agent.stoppingDistance = oldStoppingDistance;
        TargetNotFound = false;
      }

      private Transform SelectTarget() {
        foreach (var character in characters) {
          if (Vector3.Distance(character.Transform.position, transform.position) < kamikazeRadius) {
            if (character.Inventory.FillPercentage > 0.5f) {
              return character.Transform;
            }
          }
        }
        TargetNotFound = true;
        return characters.First().Transform;
      }
    }

    public class NPCRunToWarehouseState : NPCBaseState {
      NavMeshAgent agent;
      RivalsWarehouse warehouse;

      public NPCRunToWarehouseState(NPCTimerBased npc, Animator animator, NavMeshAgent agent, RivalsWarehouse warehouse) : base(npc, animator) {
        this.agent = agent;
        this.warehouse = warehouse;
      }

      public override void OnEnter() {
        IsComplete = false;
        animator.CrossFade(locomotionHash, crossFadeDuration);
        SelectNewDestination();
      }

      public override void OnUpdate() {
        if (HasReachedDestination()) {
          IsComplete = true;
          SelectNewDestination();
        }
      }

      bool HasReachedDestination() {
        return !agent.pathPending
               && agent.remainingDistance <= agent.stoppingDistance
               && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
      }

      void SelectNewDestination() {
        Vector3 result = warehouse.Position;
        result = RandomVector.GetRandomPointInRing(result, 0.7f, 3f);
        agent.SetDestination(result);
      }
    }

    public class NPCStunnedState : NPCBaseState {
      CountdownTimer stunTimer;

      public NPCStunnedState(NPCTimerBased npc, Animator animator, CountdownTimer stunTimer) : base(npc, animator) {
        this.stunTimer = stunTimer;
        this.stunTimer.OnTimerStop += () => IsComplete = true;
      }

      public override void OnEnter() {
        Debug.Log("NPCStunnedState");
        IsComplete = false;
        animator.CrossFade(stunnedHash, crossFadeDuration);
        npc.Stun();
        npc.StopMovement();
      }

      public override void OnExit() {
        IsComplete = true;
        base.OnExit();
        npc.ResumeMovement();
        npc.StopAllForces();
      }
    }

    public class NPCWanderState : NPCBaseState {
      readonly NavMeshAgent agent;
      readonly Vector3 startPoint;
      readonly float wanderRadius;

      public NPCWanderState(NPCTimerBased npc, Animator animator, NavMeshAgent agent, float wanderRadius) : base(npc, animator) {
        Debug.Log("NPCWanderState");
        this.agent = agent;
        this.startPoint = npc.transform.position;
        this.wanderRadius = wanderRadius;
      }

      public override void OnEnter() {
        IsComplete = false;

        animator.CrossFade(locomotionHash, crossFadeDuration);
        //npc.ResumeMovement();
        SelectNewDestination();
      }

      public override void OnUpdate() {
        if (HasReachedDestination()) {
          IsComplete = true;
          SelectNewDestination();
        }
      }

      bool HasReachedDestination() {
        return !agent.pathPending
               && agent.remainingDistance <= agent.stoppingDistance
               && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
      }

      void SelectNewDestination() {
        IsComplete = false;
        var randomDirection = UnityEngine.Random.insideUnitSphere * wanderRadius;
        randomDirection += startPoint;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
        var finalPosition = hit.position;

        agent.SetDestination(finalPosition);
      }

      public override void OnExit() {
        IsComplete = false;
      }
    }
  }
}


