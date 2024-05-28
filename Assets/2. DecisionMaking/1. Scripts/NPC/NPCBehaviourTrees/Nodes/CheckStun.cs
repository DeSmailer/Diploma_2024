using UnityEngine;
using UnityEngine.AI;
using UnityUtils;

namespace DecisionMaking.BehaviorTree
{
    public class CheckStun : Node
    {
        private CollisionDetector collisionDetector;
        private CountdownTimer stunTimer;

        public CheckStun(CollisionDetector collisionDetector, CountdownTimer stunTimer)
        {
            this.collisionDetector = collisionDetector;
            this.stunTimer = stunTimer;
        }

        public override NodeState Evaluate()
        {
            Debug.Log("CheckStun 1");
            if(collisionDetector.IsDetected)
            {
                Debug.Log("CheckStun 2");
                state = NodeState.SUCCESS;
                return state;
            }
            else if(stunTimer.IsRunning)
            {
                state = NodeState.SUCCESS;
                return state;
            }

            Debug.Log("CheckStun 3");
            state = NodeState.FAILURE;
            return state;
        }
    }

    public class Stun : Node
    {
        private NPCBehaviourTrees npc;
        private Animator animator;
        private CountdownTimer stunTimer;
        public Stun(NPCBehaviourTrees npc, Animator animator, CountdownTimer stunTimer)
        {
            this.npc = npc;
            this.animator = animator;
            this.stunTimer = stunTimer;
        }

        public override NodeState Evaluate()
        {
            Debug.Log("Stun");
            if(!stunTimer.IsRunning)
            {
                Enter();
            }

            Debug.Log("Stun 2");
            animator.Play(stunnedHash);
            state = NodeState.RUNNING;


            if(stunTimer.IsFinished)
            {
                Exit();
                state = NodeState.FAILURE;
            }

            Debug.Log("Stun 4");
            return state;
        }

        public void Enter()
        {
            Debug.Log("Stun 1");
            animator.Play(stunnedHash);
            npc.Stun();
            npc.StopMovement();
        }

        public void Exit()
        {
            Debug.Log("Stun 3");
            npc.ResumeMovement();
            npc.StopAllForces();
        }
    }

    public class GoToTarget : Node
    {
        private Transform transform;
        private NavMeshAgent agent;
        private Animator animator;

        public GoToTarget(Transform transform, Animator animator, NavMeshAgent agent)
        {
            this.agent = agent;
            this.transform = transform;
            this.animator = animator;
        }

        public override NodeState Evaluate()
        {
            Debug.Log("GoToTarget");
            agent.SetDestination(transform.position);
            animator.Play(locomotionHash);

            state = NodeState.RUNNING;
            return state;
        }

    }

}
