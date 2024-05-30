using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

namespace DecisionMaking.RandomSelectBased {
  public class NPCKamikazeState : NPCBaseState {
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
    }

    private Transform SelectTarget() {
      foreach (var character in characters) {
        if (Vector3.Distance(character.Transform.position, transform.position) < kamikazeRadius) {
          if (character.Inventory.FillPercentage > 0.5f) {
            return character.Transform;
          }
        }
      }
      return characters.First().Transform;
    }
  }
}
