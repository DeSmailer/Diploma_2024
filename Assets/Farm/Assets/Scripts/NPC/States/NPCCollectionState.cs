using Unity.VisualScripting;
using UnityEngine;

namespace FarmRunner {
  public class NPCCollectionState : NPCBaseState {
    public NPCCollectionState(NPC npc, Animator animator) : base(npc, animator) { }

    public bool IsComplete { get; private set; }

    public override void OnEnter() {
      animator.CrossFade(locomotionHash, crossFadeDuration);
      IsComplete = true;
    }

    public override void OnExit() {
      base.OnExit();
      IsComplete = false;
    }
  }
}