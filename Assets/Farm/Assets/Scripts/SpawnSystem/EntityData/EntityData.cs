using UnityEngine;

public abstract class EntityData : ScriptableObject {

  [SerializeField] protected GameObject prefab;
  public GameObject Prefab => prefab;
}
