using System.Collections.Generic;
using UnityEngine;

public class LinearSpawnPointStrategy : ISpawnPointStrategy {
  private int index = 0;
  private Transform[] spawnPoints;

  public LinearSpawnPointStrategy(Transform[] spawnPoints) {
    this.spawnPoints = spawnPoints;
  }

  public void Reset() {
    index = 0;
  }

  public Transform NextSpawnPoint() {
    Transform result = spawnPoints[index];
    index = (index + 1) % spawnPoints.Length;
    return result;
  }
}