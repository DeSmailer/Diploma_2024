using UnityEngine;

public class EntityFactory<T> : IEntityFactory<T> where T : Entity {
  private EntityData[] data;

  public EntityFactory(EntityData data) {
    this.data = new EntityData[] { data };
  }

  public EntityFactory(EntityData[] data) {
    this.data = data;
  }

  public T Create(Transform spawnPoint) {
    EntityData entityData = data[Random.Range(0, data.Length)];
    GameObject instanceGO = GameObject.Instantiate(entityData.Prefab, spawnPoint.position, spawnPoint.rotation);
    T instanceT = instanceGO.GetComponent<T>();
    instanceT.Initialize(entityData);
    return instanceT;
  }
}