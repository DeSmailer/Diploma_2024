using System;

namespace FarmRunner {
  [Serializable]
  public struct CostInResources {
    public ResourceData Resource;
    public int Amount;
  }
}