namespace DecisionMaking.BehaviorTree
{
    public class UntilFail : Node
    {
        public UntilFail(string name) : base(name) { }

        public override Status Process()
        {
            if(children[0].Process() == Status.Failure)
            {
                Reset();
                return Status.Failure;
            }
            return Status.Running;
        }
    }


}
//public class RandomSelector : Node
//{
//    public RandomSelector(string name, int priority = 0) : base(name, priority) { }

//    public override Status Process()
//    {
//        if(currentChild < children.Count)
//        {
//            switch(children[currentChild].Process())
//            {
//                case Status.Running:
//                    return Status.Running;
//                case Status.Success:
//                    Reset();
//                    return Status.Success;
//                default:
//                    currentChild++;
//                    return Status.Running;
//            }
//        }
//        Reset();
//        return Status.Failure;
//    }
//}