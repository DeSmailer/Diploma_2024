using System.Collections.Generic;
using UnityEngine;

namespace FarmRunner {
  [CreateAssetMenu(fileName = "CostData", menuName = "SO/CostData")]
  public class CostInResourcesData : ScriptableObject {
    [SerializeField] List<CostInResources> costInResources;

    public List<CostInResources> CostInResources => costInResources;
  }
}
