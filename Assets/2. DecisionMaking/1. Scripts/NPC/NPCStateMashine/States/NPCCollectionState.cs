﻿using Unity.VisualScripting;
using UnityEngine;

namespace DecisionMaking.StateMashine
{
  public class NPCCollectionState : NPCBaseState {
    public NPCCollectionState(NPCStateMashine npc, Animator animator) : base(npc, animator) { }

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