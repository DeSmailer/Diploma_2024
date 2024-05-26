using UnityEngine;

public interface ISpawnPointStrategy {
  public void Reset();
  public Transform NextSpawnPoint();
}