using UnityEngine;

namespace DecisionMaking {
  public class NPCStunnedState : NPCBaseState {

    public NPCStunnedState(NPC npc, Animator animator) : base(npc, animator) { }

    public override void OnEnter() {
      animator.CrossFade(stunnedHash, crossFadeDuration);
      npc.Stun();
      npc.StopMovement();
    }

    public override void OnExit() {
      base.OnExit();
      npc.ResumeMovement();
      npc.StopAllForces();
    }
  }
}
