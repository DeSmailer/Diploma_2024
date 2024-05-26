using UnityEngine;


namespace DecisionMaking.BehaviorTree
{
    public abstract class BehaviorTree : MonoBehaviour
    {
        protected Node rootNode;

        protected virtual void Start()
        {
            rootNode = SetupTree();
        }

        protected virtual void Update()
        {
            if(rootNode != null)
            {
                rootNode.Evaluate();
            }
        }

        protected abstract Node SetupTree();
    }
}
