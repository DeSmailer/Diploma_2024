namespace DecisionMaking.BehaviorTree
{
    public interface IStrategy
    {
        Node.Status Process();
        void Reset();
    }
}
