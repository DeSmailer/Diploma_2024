using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace DecisionMaking.TimerBased {
  public partial class NPCTimerBased {
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
  }
}


