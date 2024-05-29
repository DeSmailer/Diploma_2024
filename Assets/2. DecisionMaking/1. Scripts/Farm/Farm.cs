using System.Collections.Generic;
using UnityEngine;

namespace DecisionMaking
{
    public class Farm : MonoBehaviour
    {
        [SerializeField] ResourceData resourceData;
        [SerializeField] SpecifiedAmountOfResourcesSpawnManager resourceSpawnManager;

        [SerializeField] Transform position;

        public bool CanHarvest => resourceSpawnManager.ReadyForCollection;
        public Vector3 Position => position.position;
        public ResourceData ResourceData => resourceData;

        private void Start()
        {
            resourceSpawnManager.Initialize(resourceData);
        }

        public void TryCollectResources(Inventory inventory)
        {
            if(resourceSpawnManager.ReadyForCollection && inventory.CanGetResources)
            {
                CollectResources(inventory);
            }
        }

        private void CollectResources(Inventory inventory)
        {

            //Сортируем список по расстоянию до инвентаря
            resourceSpawnManager.SpawnedResources.Sort((a, b) =>
              Vector3.Distance(a.transform.position, inventory.transform.position)
              .CompareTo(Vector3.Distance(b.transform.position, inventory.transform.position)));

            List<CollectedResource> collectedResources = new List<CollectedResource>(resourceSpawnManager.SpawnedResources.Count);

            for(int i = 0; i < resourceSpawnManager.SpawnedResources.Count; i++)
            {
                CollectedResource collectedResource;
                resourceSpawnManager.SpawnedResources[i].Collect(out collectedResource);
                collectedResources.Add(collectedResource);
            }

            inventory.CollectResources(collectedResources);

            resourceSpawnManager.SpawnedResources.Clear();
            resourceSpawnManager.Respawn();
        }
    }
}
