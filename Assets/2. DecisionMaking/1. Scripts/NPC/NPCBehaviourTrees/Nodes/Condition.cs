using System;
using UnityEngine;

namespace DecisionMaking.BehaviorTree
{
    public class Condition : IStrategy
    {
        readonly Func<bool> predicate;

        public Condition(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        public Node.Status Process() => predicate() ? Node.Status.Success : Node.Status.Failure;

        public void Reset()
        {
            Debug.Log("Condition Reset");
        }
    }

}
