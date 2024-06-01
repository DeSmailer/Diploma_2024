using UnityEngine;

namespace DecisionMaking {
  [CreateAssetMenu(fileName = "CharacterInfo", menuName = "SO/CharacterInfo", order = 0)]
  public class CharacterInfo : ScriptableObject {
    [SerializeField] private string _name;
    [SerializeField] private Sprite sprite;
    [SerializeField] private AlgoritmType algoritmType ;
    
    public string Name => _name;
    public Sprite Sprite => sprite;
    public AlgoritmType AlgoritmType => algoritmType;
    }

    public enum AlgoritmType
    {
      BT,
      SM,
      TB,
      RS
    }
}
