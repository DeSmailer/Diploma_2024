namespace UnityUtils {

  public class Transition : ITransition {
    public IState To { get; }
    public IPredicate Condition { get; }

    public Transition(IState to, IPredicate predicate) {
      To = to;
      Condition = predicate;
    }
  }
  
}
