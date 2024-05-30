using UnityEngine;
using UnityEngine.AI;

namespace DecisionMaking.RandomSelectBased {
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
