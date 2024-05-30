using System.Collections.Generic;
using UnityEngine;
using UnityUtils;
using System.Linq;
using UnityEngine.AI;
using DecisionMaking.Utils;

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
      Debug.Log("SelectNewState");
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

  public class NPCBaseState : IState {
    protected NPCRandomSelectBased npc;
    protected Animator animator;

    protected static readonly int idleHash = Animator.StringToHash("Idle");
    protected static readonly int locomotionHash = Animator.StringToHash("Locomotion");
    protected static readonly int stunnedHash = Animator.StringToHash("Stunned");
    protected static readonly int KamikazeHash = Animator.StringToHash("Kamikaze");

    protected const float crossFadeDuration = 0.1f;

    public virtual bool IsComplete { get; protected set; }

    protected NPCBaseState(NPCRandomSelectBased npc, Animator animator) {
      this.npc = npc;
      this.animator = animator;
    }

    public virtual void OnEnter() {
      // noop
    }

    public virtual void OnExit() {
      // noop
    }

    public virtual void OnUpdate() {
      // noop
    }

    public virtual void OnFixedUpdate() {
      // noop
    }
  }

  public class NPCKamikazeState : NPCBaseState {
    public bool TargetNotFound;

    List<ICharacter> characters;
    NavMeshAgent agent;
    Transform target;
    float kamikazeRadius;
    Transform transform;

    float oldStoppingDistance;

    public NPCKamikazeState(NPCRandomSelectBased npc, Animator animator, List<ICharacter> characters, NavMeshAgent agent, Transform transform, float kamikazeRadius) : base(npc, animator) {
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

  public class NPCRunToFarmState : NPCBaseState {
    NavMeshAgent agent;
    Farm[] farms;
    Farm selectedFarm;
    RivalsWarehouse rivalsWarehouse;

    public NPCRunToFarmState(NPCRandomSelectBased npc, Animator animator, NavMeshAgent agent, Farm[] farms, RivalsWarehouse rivalsWarehouse) : base(npc, animator) {
      this.agent = agent;
      this.farms = farms;
      this.rivalsWarehouse = rivalsWarehouse;
    }

    public override void OnEnter() {
      Debug.Log("NPCRunToFarmState");
      IsComplete = false;
      animator.CrossFade(locomotionHash, crossFadeDuration);
      SelectFarm();
    }

    public override void OnUpdate() {
      if (HasReachedDestination()) {
        IsComplete = true;
      }
      if (!selectedFarm.CanHarvest) {
        IsComplete = true;
        SelectFarm();
      }
    }

    bool HasReachedDestination() {
      return !agent.pathPending
             && agent.remainingDistance <= agent.stoppingDistance
             && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
    }

    void SelectFarm() {
      IFarmSelector farmSelector = new RequiredResourceFarmSelector(farms, npc, npc.transform, rivalsWarehouse);
      //IFarmSelector farmSelector = new RandomFromClosestFarmSelector(farms, npc.transform, 3);
      selectedFarm = farmSelector.SelectFarm();
      Vector3 result = GetPositionInRadius(selectedFarm.transform);

      agent.SetDestination(result);
    }

    public Vector3 GetPositionInRadius(Transform center, float innerRadius = 0.1f, float outerRadius = 2f) {
      Vector3 result = RandomVector.GetRandomPointInRing(center.position, innerRadius, outerRadius);
      return result;
    }
  }

  public class NPCRunToWarehouseState : NPCBaseState {
    NavMeshAgent agent;
    RivalsWarehouse warehouse;

    public NPCRunToWarehouseState(NPCRandomSelectBased npc, Animator animator, NavMeshAgent agent, RivalsWarehouse warehouse) : base(npc, animator) {
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

    public NPCStunnedState(NPCRandomSelectBased npc, Animator animator, CountdownTimer stunTimer) : base(npc, animator) {
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

    public NPCWanderState(NPCRandomSelectBased npc, Animator animator, NavMeshAgent agent, float wanderRadius) : base(npc, animator) {
      Debug.Log("NPCWanderState");
      this.agent = agent;
      this.startPoint = npc.transform.position;
      this.wanderRadius = wanderRadius;
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
      IsComplete = false;
      var randomDirection = Random.insideUnitSphere * wanderRadius;
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
