using UnityEngine;
using UnityUtils;

namespace DecisionMaking.RandomSelectBased {
  public class NPCStunnedState : NPCBaseState {
    CountdownTimer stunTimer;

    public NPCStunnedState(NPCRandomSelectBased npc, Animator animator, CountdownTimer stunTimer) : base(npc, animator) {
      this.stunTimer = stunTimer;
      this.stunTimer.OnTimerStop += () => IsComplete = true;
    }

    public override void OnEnter() {
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
}
