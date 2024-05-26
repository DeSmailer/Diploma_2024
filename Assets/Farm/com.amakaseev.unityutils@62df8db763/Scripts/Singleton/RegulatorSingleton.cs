using UnityEngine;

namespace UnityUtils {
  // Persistent Regulator singleton, will destroy any other older components of the same type it finds on awake
  public class RegulatorSingleton<T> : MonoBehaviour where T : Component {
    protected static T instance;

    public static bool HasInstance => instance != null;
    public float InitializationTime { get; private set; }

    public static T Instance {
      get {
        if (instance == null) {
          instance = FindAnyObjectByType<T>();
          if (instance == null) {
            var go = new GameObject(typeof(T).Name + " Auto-Generated");
            go.hideFlags = HideFlags.HideAndDontSave;
            instance = go.AddComponent<T>();
          }
        }
        return instance;
      }
    }

    // Make sure to call base.Awake() in override if you need awake.
    protected virtual void Awake() {
      InitializeSingleton();
    }

    protected virtual void InitializeSingleton() {
      if (!Application.isPlaying) return;
      InitializationTime = Time.time;
      DontDestroyOnLoad(gameObject);

      T[] oldInstances = FindObjectsByType<T>(FindObjectsSortMode.None);
      foreach (T old in oldInstances) {
        if (old.GetComponent<RegulatorSingleton<T>>().InitializationTime < InitializationTime) {
          Destroy(old.gameObject);
        }
      }

      if (instance == null) {
        instance = this as T;
      }
    }
  }
}