using UnityEngine;

public class SequentialSpawnPointStrategy : ISpawnPointStrategy {
  private Transform spawnPoint;

  public SequentialSpawnPointStrategy(Transform spawnPoint) {
    SetNewSpawnPoint(spawnPoint);
    Debug.Log(spawnPoint.parent.name);
  }

  public void Reset() {
    // Do nothing
  }

  public void SetNewSpawnPoint(Transform spawnPoint) {
    this.spawnPoint = spawnPoint;
  }

  public Transform NextSpawnPoint() {
    return spawnPoint;
  }
}
