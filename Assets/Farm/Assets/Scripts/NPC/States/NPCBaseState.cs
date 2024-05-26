using UnityEngine;
using UnityUtils;

namespace FarmRunner {
  public class NPCBaseState : IState {
    protected NPC npc;
    protected Animator animator;

    protected static readonly int idleHash = Animator.StringToHash("Idle");
    protected static readonly int locomotionHash = Animator.StringToHash("Locomotion");
    protected static readonly int stunnedHash = Animator.StringToHash("Stunned");
    protected static readonly int KamikazeHash = Animator.StringToHash("Kamikaze");

    protected const float crossFadeDuration = 0.1f;

    protected NPCBaseState(NPC npc, Animator animator) {
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
}