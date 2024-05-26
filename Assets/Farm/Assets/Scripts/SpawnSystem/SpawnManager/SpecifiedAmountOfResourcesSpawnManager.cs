using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

namespace FarmRunner {
  public class SpecifiedAmountOfResourcesSpawnManager : EntitySpawnerManager {

    [SerializeField] float spawnInterval = 1f;

    ResourceData resourceData;
    EntitySpawner<Resource> spawner;

    List<Resource> spawnedResources = new List<Resource>();

    CountdownTimer spawnIntervalTimer;
    CountdownTimer spawnTimer;

    [SerializeField] private int maxCount;
    int counter = 0;

    public List<Resource> SpawnedResources => spawnedResources;
    public bool ReadyForCollection { get; private set; }

    override protected void Awake() {
    }

    public void Initialize(ResourceData resourceData) {
      SelectSpawnPointStrategy();

      this.resourceData = resourceData;

      spawner = new EntitySpawner<Resource>(new EntityFactory<Resource>(resourceData), spawnPointStrategy);

      spawnTimer = new CountdownTimer(resourceData.RespawnTime);
      spawnIntervalTimer = new CountdownTimer(spawnInterval);

      spawnTimer.OnTimerStop += () => SpawnAll();
      spawnIntervalTimer.OnTimerStop += () => SpawnElement();

      SpawnAll();
    }

    void Update() {
      spawnTimer.Tick(Time.deltaTime);
      spawnIntervalTimer.Tick(Time.deltaTime);
    }

    public void Respawn() {
      spawnTimer.Start();
      ReadyForCollection = false;
    }

    void SpawnAll() {
      counter = 0;
      spawnIntervalTimer.Start();
      spawnPointStrategy.Reset();

      SpawnElement();
    }

    void SpawnElement() {
      if (counter >= spawnPoints.Length || counter >= maxCount) {
        spawnIntervalTimer.Stop();
        ReadyForCollection = true;
        return;
      }
      Spawn();
      counter++;
      spawnIntervalTimer.Start();
    }

    protected override void Spawn() {
      Resource resource = spawner.Spawn();
      resource.Initialize(resourceData);

      float rotate = Random.Range(0, 360);
      resource.transform.Rotate(Vector3.up, rotate);

      resource.transform.parent = transform;

      spawnedResources.Add(resource);
    }
  }
}
