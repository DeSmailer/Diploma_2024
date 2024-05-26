using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DecisionMaking
{
    public class StoredResourcesView : MonoBehaviour
    {
        [SerializeField] Image resourceImage;
        [SerializeField] TMP_Text resourcesCount;

        ResourceData resourceData;

        public ResourceData ResourceData => resourceData;

        public void Initialize(ResourceData resourceData)
        {
            resourceImage.sprite = resourceData.Icon;
            this.resourceData = resourceData;
        }

        public void Display(int count)
        {
            resourcesCount.text = count.ToString();
        }
    }
}
