using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DecisionMaking {
  public abstract class AmountOfResourcesView : MonoBehaviour {
    [SerializeField] protected Image image;
    [SerializeField] protected TMP_Text text;

    public ResourceData Resource { get; private set; }

    public virtual void Initialize(ResourceData resource) {
      Resource = resource;
      image.sprite = Resource.Icon;
      text.text = "0";
    }

    public virtual void UpdateAmount(int amount) {
      text.text = amount.ToString();
    }
  }
}
