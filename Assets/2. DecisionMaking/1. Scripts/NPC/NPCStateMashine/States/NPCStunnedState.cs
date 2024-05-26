using UnityEngine;

namespace DecisionMaking.StateMashine
{
  public class NPCStunnedState : NPCBaseState {

    public NPCStunnedState(NPCStateMashine npc, Animator animator) : base(npc, animator) { }

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
