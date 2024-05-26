using System.Collections.Generic;
using DecisionMaking.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace DecisionMaking.StateMashine
{
  public class NPCRunToFarmState : NPCBaseState {
    NavMeshAgent agent;
    Farm[] farms;

    public bool IsComplete { get; private set; }

    public NPCRunToFarmState(NPCStateMashine npc, Animator animator, NavMeshAgent agent, Farm[] farms) : base(npc, animator) {
      this.agent = agent;
      this.farms = farms;
    }

    public override void OnEnter() {
      IsComplete = false;
      animator.CrossFade(locomotionHash, crossFadeDuration);
      SelectFarm();
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

    void SelectFarm() {
      var selectedFarms = new List<Farm>(farms);
      selectedFarms.RemoveAll(farm => !farm.CanHarvest);

      selectedFarms.Sort((a, b) =>
       Vector3.Distance(a.transform.position, npc.transform.position)
       .CompareTo(Vector3.Distance(b.transform.position, npc.transform.position)));

      selectedFarms = selectedFarms.GetRange(0, Mathf.Min(selectedFarms.Count, 3));

      var randomFarm = selectedFarms[Random.Range(0, selectedFarms.Count)];
      Vector3 result = randomFarm.Position;
      result = RandomVector.GetRandomPointInRing(result, 0.7f, 3f);

      agent.SetDestination(result);
    }
  }
}