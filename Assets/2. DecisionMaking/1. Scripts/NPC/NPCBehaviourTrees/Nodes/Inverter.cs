namespace DecisionMaking.BehaviorTree
{
    public class Inverter : Node
    {
        public Inverter(string name) : base(name) { }

        public override Status Process()
        {
            switch(children[0].Process())
            {
                case Status.Running:
                    return Status.Running;
                case Status.Failure:
                    return Status.Success;
                default:
                    return Status.Failure;
            }
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