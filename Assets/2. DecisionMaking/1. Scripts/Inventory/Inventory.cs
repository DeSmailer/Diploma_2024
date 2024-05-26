using System;
using System.Collections.Generic;
using DecisionMaking.Utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace DecisionMaking
{

    public class Inventory : ResourcesStorage
    {

        [SerializeField] List<CollectedResource> flyingResources = new List<CollectedResource>();
        [SerializeField] List<CollectedResource> flownResources = new List<CollectedResource>();

        [SerializeField] LocatorOfCollectedResources locatorOfCollectedResources;
        [SerializeField] int maxCapacity = 100;

        [SerializeField] LayerMask groundLayer;

        [SerializeField] float innerScatterRadius = 5f;
        [SerializeField] float outerScatterRadius = 5f;

        float lastCollectionTime = 0;

        public List<CollectedResource> Resources
        {
            get
            {
                List<CollectedResource> result = new List<CollectedResource>(flownResources);
                result.AddRange(flyingResources);
                return result;
            }
        }
        public override int ResourcesCount
        {
            get
            {
                return flownResources.Count + flyingResources.Count;
            }
        }
        public int MaxCapacity => maxCapacity;
        public int FreeSlots => MaxCapacity - ResourcesCount;
        /// <summary>
        /// [0,1]
        /// </summary>
        public float FillPercentage => (float)ResourcesCount / MaxCapacity;
        public bool CanGetResources => FreeSlots > 0;
        public override LocatorOfCollectedResources LocatorOfCollectedResources => locatorOfCollectedResources;

        public UnityEvent OnInventoryFull;
        public UnityEvent OnInventoryEmpty;

        public UnityEvent OnResourcesCountChanged;

        public void CollectResources(List<CollectedResource> resources)
        {
            float timeDifference = Time.time - lastCollectionTime;
            lastCollectionTime = Time.time;

            float delay = GlobalConstants.delayBetweenCollectToInventory * (flyingResources.Count - 1) - timeDifference;
            if(delay < 0)
            {
                delay = 0;
            }

            bool startCanGetResources = CanGetResources;

            foreach(var item in resources)
            {
                if(CanGetResources)
                {
                    flyingResources.Add(item);
                    delay += GlobalConstants.delayBetweenCollectToInventory;
                    item.AddToInventory(this, delay);
                    item.OnInventoryMovementEnded.AddListener(OnInventoryMovementEnded);
                }
                else
                {
                    item.Disappear();
                }
            }
            if(startCanGetResources != CanGetResources)
            {
                OnInventoryFull?.Invoke();
            }

            OnResourcesCountChanged?.Invoke();
        }

        private void OnInventoryMovementEnded(CollectedResource resource)
        {
            flownResources.Add(resource);
            flyingResources.Remove(resource);
            resource.OnInventoryMovementEnded.RemoveListener(OnInventoryMovementEnded);
        }

        public List<CollectedResource> GiveAllResources()
        {
            List<CollectedResource> result = new List<CollectedResource>(Resources);
            foreach(CollectedResource item in flyingResources)
            {
                item.OnInventoryMovementEnded.RemoveListener(OnInventoryMovementEnded);
            }
            flyingResources.Clear();
            flownResources.Clear();
            result.Reverse();

            if(ResourcesCount == 0)
            {
                OnInventoryEmpty?.Invoke();
            }

            OnResourcesCountChanged?.Invoke();
            return result;
        }

        public void DropOnGround(float dropPercent = 0.5f)
        {
            if(Physics.Raycast(locatorOfCollectedResources.InventoryPosition.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {

                float y = hit.point.y;

                List<CollectedResource> resourcesForDrop = new List<CollectedResource>(flownResources);
                resourcesForDrop.Reverse();

                int count = (int)(resourcesForDrop.Count * dropPercent);

                if(count == 0 && resourcesForDrop.Count > 0)
                {
                    count = 1;
                }

                for(int i = 0; i < count; i++)
                {
                    CollectedResource item = resourcesForDrop[i];
                    Vector3 center = new Vector3(transform.position.x, 0, transform.position.z);
                    Vector3 randomPosition = RandomVector.GetRandomPointInRing(center, innerScatterRadius, outerScatterRadius);
                    randomPosition = new Vector3(randomPosition.x, y, randomPosition.z);

                    float degree180 = 180f;
                    Vector3 randomRotation = RandomVector.GetRandomVector3(-degree180, degree180, -degree180, degree180, -degree180, degree180);

                    item.DropOnGround(randomPosition, randomRotation, 0.02f * 1);
                    RemoveCollectedResource(item);
                }

                for(int i = 0; i < Resources.Count; i++)
                {
                    Resources[i].Reindex(i);
                }

                if(ResourcesCount == 0)
                {
                    OnInventoryEmpty?.Invoke();
                }
                OnResourcesCountChanged?.Invoke();
            }
        }

        private void RemoveCollectedResource(CollectedResource item)
        {
            if(flownResources.Contains(item))
            {
                flownResources.Remove(item);
            }
            else if(flyingResources.Contains(item))
            {
                item.OnInventoryMovementEnded.RemoveListener(OnInventoryMovementEnded);
                flyingResources.Remove(item);
            }

            if(ResourcesCount == 0)
            {
                OnInventoryEmpty?.Invoke();
            }
            OnResourcesCountChanged?.Invoke();
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 center = new Vector3(transform.position.x, 0, transform.position.z);
            int segments = 64;
            Gizmos.color = Color.yellow;
            DrawRingGizmos(center, innerScatterRadius, outerScatterRadius, segments);
        }

        void DrawRingGizmos(Vector3 center, float innerRadius, float outerRadius, int segments)
        {
            float angleStep = 360f / segments;

            for(int i = 0; i < segments; i++)
            {
                float angle1 = Mathf.Deg2Rad * i * angleStep;
                float angle2 = Mathf.Deg2Rad * (i + 1) * angleStep;

                Vector3 innerPoint1 = new Vector3(center.x + innerRadius * Mathf.Cos(angle1), center.y, center.z + innerRadius * Mathf.Sin(angle1));
                Vector3 innerPoint2 = new Vector3(center.x + innerRadius * Mathf.Cos(angle2), center.y, center.z + innerRadius * Mathf.Sin(angle2));

                Vector3 outerPoint1 = new Vector3(center.x + outerRadius * Mathf.Cos(angle1), center.y, center.z + outerRadius * Mathf.Sin(angle1));
                Vector3 outerPoint2 = new Vector3(center.x + outerRadius * Mathf.Cos(angle2), center.y, center.z + outerRadius * Mathf.Sin(angle2));

                Gizmos.DrawLine(innerPoint1, innerPoint2);
                Gizmos.DrawLine(outerPoint1, outerPoint2);

                Gizmos.DrawLine(innerPoint1, outerPoint1);
                Gizmos.DrawLine(innerPoint2, outerPoint2);
            }
        }
    }
}

//public bool RemoveResources(ResourceData resource, int count = 1) {
//  if (HasResources(resource, count)) {
//    int removedCount = 0;

//    for (int i = flyingResources.Count - 1; i >= 0; i--) {
//      if (flyingResources[i].ResourceData == resource) {
//        flyingResources.RemoveAt(i);
//        removedCount++;
//        if (removedCount >= count) {
//          return true;
//        }
//      }
//    }

//    for (int i = flownResources.Count - 1; i >= 0; i--) {
//      if (flownResources[i].ResourceData == resource) {
//        flownResources.RemoveAt(i);
//        removedCount++;
//        if (removedCount >= count) {
//          return true;
//        }
//      }
//    }
//  }
//  return false;
//}

//public void AddResource(CollectedResource resource) {
//  if (CanGetResources) {
//    flyingResources.Add(resource);
//    float delay = GlobalConstants.delayBetweenCollectToInventory * (flyingResources.Count - 1);
//    resource.AddToInventory(this, delay);
//    resource.OnInventoryMovementEnded.AddListener(OnInventoryMovementEnded);
//  } else {
//    Destroy(resource.gameObject);
//  }
//}


//public bool HasResources(ResourceData resourceData, int count = 1) {
//  int foundCount = 0;
//  foreach (CollectedResource item in Resources) {
//    if (item.ResourceData == resourceData) {
//      foundCount++;
//      if (foundCount >= count) {
//        return true;
//      }
//    }
//  }
//  return false;
//}

//public ResourceData RemoveResource(ResourceData resource) {
//  foreach (CollectedResource item in flyingResources) {
//    if (item.ResourceData == resource) {
//      flyingResources.Remove(item);
//      return item.ResourceData;
//    }
//  }
//  foreach (CollectedResource item in flownResources) {
//    if (item.ResourceData == resource) {
//      flownResources.Remove(item);
//      return item.ResourceData;
//    }
//  }

//  return null;
//}