using UnityEngine;
using UnityEngine.Events;

namespace UnityUtils {

  public abstract class EventListener<T> : MonoBehaviour {
    [SerializeField] EventChanel<T> eventChanel;
    [SerializeField] UnityEvent<T> unityEvent;

    protected void Awake() {
      eventChanel.Register(this);
    }

    protected void OnDestroy() {
      eventChanel.Deregister(this);
    }

    public void Raise(T value) {
      unityEvent?.Invoke(value);
    }
  }

  public class EventListener : EventListener<Empty> { }
}
