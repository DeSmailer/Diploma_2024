using System.Linq;
using UnityEngine;

public abstract class EntitySpawnerManager : MonoBehaviour {
  [SerializeField] protected SpawnPointStrategyType spawnPointStrategyType = SpawnPointStrategyType.Liner;
  [SerializeField] protected Transform[] spawnPoints;

  [SerializeField] protected ISpawnPointStrategy spawnPointStrategy;

  protected enum SpawnPointStrategyType {
    Liner,
    Random,
    Sequential
  }

  protected virtual void Awake() {
    SelectSpawnPointStrategy();
  }

  public virtual void SelectSpawnPointStrategy() {
    switch (spawnPointStrategyType) {
      case SpawnPointStrategyType.Liner:
        spawnPointStrategy = new LinearSpawnPointStrategy(spawnPoints);
        break;
      case SpawnPointStrategyType.Random:
        spawnPointStrategy = new RandomSpawnPointStrategy(spawnPoints);
        break;
      case SpawnPointStrategyType.Sequential:
        spawnPointStrategy = new SequentialSpawnPointStrategy(spawnPoints.First());
        break;
      default:
        break;
    }
  }

  protected abstract void Spawn();
}
