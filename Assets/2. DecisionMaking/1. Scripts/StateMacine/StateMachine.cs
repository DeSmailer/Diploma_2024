using System;
using System.Collections.Generic;

namespace UnityUtils {

  public class StateMachine {
    StateNode current;
    Dictionary<Type, StateNode> nodes = new();
    HashSet<ITransition> anyTransitions = new();

    public IState CurrentState => current?.State;
    
    public void Update() {
      var transition = GetTransition();
      if (transition != null) {
        ChangeState(transition.To);
      }

      current.State?.OnUpdate();
    }

    public void FixedUpdate() {
      current.State?.OnFixedUpdate();
    }

    public void SetState(IState state) {
      if (current != null) {
        current.State?.OnExit();
      }

      current = GetOrAddNode(state);
      current.State?.OnEnter();
    }

    void ChangeState(IState state) {
      if (state == current.State)
        return;

      var prevState = current.State;
      var nextState = nodes[state.GetType()].State;

      prevState?.OnExit();
      nextState?.OnEnter();

      current = nodes[state.GetType()];
    }

    ITransition GetTransition() {
      foreach (var transition in anyTransitions) {
        if (transition.Condition.Evaluate()) {
          return transition;
        }
      }

      foreach (var transition in current.Transitions) {
        if (transition.Condition.Evaluate()) {
          return transition;
        }
      }

      return null;
    }

    public void AddTransition(IState from, IState to, IPredicate condition) {
      GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
    }

    public void AddAnyTransition(IState to, IPredicate condition) {
      anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
    }

    StateNode GetOrAddNode(IState state) {
      var node = nodes.GetValueOrDefault(state.GetType());

      if (node == null) {
        node = new StateNode(state);
        nodes.Add(state.GetType(), node);
      }

      return node;
    }

    class StateNode {
      public IState State { get; }
      public HashSet<ITransition> Transitions { get; }

      public StateNode(IState state) {
        State = state;
        Transitions = new HashSet<ITransition>();
      }

      public void AddTransition(IState to, IPredicate condition) {
        Transitions.Add(new Transition(to, condition));
      }
    }
  }

}
