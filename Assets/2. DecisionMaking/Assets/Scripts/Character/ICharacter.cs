using UnityEngine;

namespace FarmRunner
{
    public interface ICharacter
    {
        public CharacterInfo CharacterInfo { get; }
        public Inventory Inventory { get; }
        public Transform Transform { get; }
    }
}
