﻿using System;

namespace DecisionMaking.BehaviorTree
{
    public class ActionStrategy : IStrategy
    {
        readonly Action doSomething;

        public ActionStrategy(Action doSomething)
        {
            this.doSomething = doSomething;
        }

        public Node.Status Process()
        {
            doSomething();
            return Node.Status.Success;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

}