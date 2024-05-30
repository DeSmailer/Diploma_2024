using UnityEngine;
using UnityEngine.AI;
using DecisionMaking.Utils;

namespace DecisionMaking.RandomSelectBased {
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
      }
    }

    bool HasReachedDestination() {
      return !agent.pathPending
             && agent.remainingDistance <= agent.stoppingDistance
             && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
    }

    void SelectNewDestination() {
      Vector3 result = warehouse.Position;
      result = RandomVector.GetRandomPointInRing(result, 0.1f, 2f);
      agent.SetDestination(result);
    }

  }
}
