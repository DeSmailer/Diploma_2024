﻿using DecisionMaking.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace DecisionMaking.StateMashine
{
  public class NPCRunToWarehouseState : NPCBaseState {
    NavMeshAgent agent;
    RivalsWarehouse warehouse;

    public bool IsComplete { get; private set; }

    public NPCRunToWarehouseState(NPCStateMashine npc, Animator animator, NavMeshAgent agent, RivalsWarehouse warehouse) : base(npc, animator) {
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
        //SelectFarm();
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
}