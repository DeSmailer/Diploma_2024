using UnityEngine;

namespace DecisionMaking
{
    public class NPCGiveAwayResourcesState : NPCBaseState
    {
        public NPCGiveAwayResourcesState(NPC npc, Animator animator) : base(npc, animator) { }

        public bool IsComplete { get; private set; }

        public override void OnEnter()
        {
            animator.CrossFade(locomotionHash, crossFadeDuration);
            IsComplete = true;
        }

        public override void OnExit()
        {
            base.OnExit();
            IsComplete = false;
        }
    }
}