using System.Collections.Generic;
using UnityEngine;

namespace UnityUtils {

  public abstract class EventChanel<T> : ScriptableObject {
    readonly HashSet<EventListener<T>> observers = new();

    public void Invoke(T value) {
      foreach (var observer in observers) {
        observer.Raise(value);
      }
    }

    public void Register(EventListener<T> observer) => observers.Add(observer);
    public void Deregister(EventListener<T> observer) => observers.Remove(observer);
  }

  public readonly struct Empty { }

  [CreateAssetMenu(menuName = "Events/EventChanel")]
  public class EventChanel : EventChanel<Empty> { }

}
