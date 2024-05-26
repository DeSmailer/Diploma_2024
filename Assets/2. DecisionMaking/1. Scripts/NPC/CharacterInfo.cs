using UnityEngine;

namespace DecisionMaking {
  [CreateAssetMenu(fileName = "CharacterInfo", menuName = "SO/CharacterInfo", order = 0)]
  public class CharacterInfo : ScriptableObject {
    [SerializeField] private string _name;
    [SerializeField] private Sprite sprite;
    
    public string Name => _name;
    public Sprite Sprite => sprite;
  }
}
