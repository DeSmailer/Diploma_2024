using UnityEngine;

namespace FarmRunner {
  [CreateAssetMenu(fileName = "ResourceData", menuName = "SO/ResourceData")]
  public class ResourceData : EntityData {
    [SerializeField] ResourceName resourceName;
    [SerializeField] Sprite icon;
    [SerializeField] GameObject collectedPrefab;
    [SerializeField] float respawnTime;

    public ResourceName ResourceName => resourceName;
    public Sprite Icon => icon;
    public GameObject CollectedPrefab => collectedPrefab;
    public float RespawnTime => respawnTime;
  }
}