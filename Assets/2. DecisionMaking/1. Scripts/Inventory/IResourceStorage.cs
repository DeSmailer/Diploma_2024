namespace DecisionMaking {
  internal interface IResourceStorage {
    public int ResourcesCount { get; }
    public LocatorOfCollectedResources LocatorOfCollectedResources { get; }
  }
}