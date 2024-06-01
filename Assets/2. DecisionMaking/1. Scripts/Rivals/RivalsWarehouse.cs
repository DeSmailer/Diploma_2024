using DecisionMaking.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace DecisionMaking
{
    public class RivalsWarehouse : ResourcesStorage
    {
        [SerializeField] Transform position;
        [SerializeField] LocatorOfCollectedResources locatorOfCollectedResources;

        CostInResourcesData necessaryResourcesForVictory;

        Dictionary<ICharacter, Dictionary<ResourceData, int>> characterResources = new Dictionary<ICharacter, Dictionary<ResourceData, int>>();

        public override LocatorOfCollectedResources LocatorOfCollectedResources => locatorOfCollectedResources;
        public Vector3 Position => position.position;
        public Dictionary<ICharacter, Dictionary<ResourceData, int>> CharacterResources => characterResources;

        public override int ResourcesCount => 0;
        //{
        //    get
        //    {
        //        //int count = 0;
        //        //foreach(var item in Resources)
        //        //{
        //        //    count += item.Value;
        //        //}
        //        //return count;
        //        return 0;
        //    }
        //}

        public UnityEvent OnResourcesAdded;
        public UnityEvent OnVictory;

        public void Initialize(CostInResourcesData necessaryResourcesForVictory, List<ICharacter> characters)
        {
            this.necessaryResourcesForVictory = necessaryResourcesForVictory;
            characterResources = new Dictionary<ICharacter, Dictionary<ResourceData, int>>();


            foreach(var character in characters)
            {
                Dictionary<ResourceData, int> resources = new Dictionary<ResourceData, int>();
                foreach(var item in necessaryResourcesForVictory.CostInResources)
                {
                    resources.Add(item.Resource, 0);
                }
                characterResources.Add(character, resources);
            }
        }

        public void AddResources(List<CollectedResource> resources, ICharacter character)
        {
            float delay = 0;

            foreach(var item in resources)
            {
                delay += GlobalConstants.delayBetweenFlyToWarehouse;
                item.AddToWarehouse(this, delay);
                AddResource(item.ResourceData, character);
            }

            OnResourcesAdded.Invoke();
            CheckForVictory();
        }

        public void AddResource(ResourceData resource, ICharacter character)
        {
            if(CharacterResources.ContainsKey(character))
            {
                var res = CharacterResources[character];
                if(res.ContainsKey(resource))
                {
                    res[resource]++;
                }
                else
                {
                    res.Add(resource, 1);
                }
            }
            else
            {
                Dictionary<ResourceData, int> resources = new Dictionary<ResourceData, int>();
                foreach(var item in necessaryResourcesForVictory.CostInResources)
                {
                    resources.Add(item.Resource, 0);
                }
                resources[resource] = 1;
                characterResources.Add(character, resources);
            }
        }

        public Dictionary<ResourceData, int> LeftCollectResources(ICharacter character)
        {
            Dictionary<ResourceData, int> resources = new Dictionary<ResourceData, int>();
            foreach(var item in necessaryResourcesForVictory.CostInResources)
            {
                int diference = item.Amount - GetResourceAmount(item.Resource, character);
                if(diference < 0)
                {
                    diference = 0;
                }
                resources.Add(item.Resource, diference);
            }
            return resources;
        }

        public int LeftCollectResource(ResourceData resourceData, ICharacter character)
        {
            foreach(var item in necessaryResourcesForVictory.CostInResources)
            {
                if(item.Resource == resourceData)
                {
                    int diference = item.Amount - GetResourceAmount(item.Resource, character);
                    if(diference < 0)
                    {
                        diference = 0;
                    }
                    return diference;
                }
            }
            return 0;
        }

        public int GetResourceAmount(ResourceData resource, ICharacter character)
        {
            if(CharacterResources.ContainsKey(character))
            {
                var res = CharacterResources[character];
                if(res.ContainsKey(resource))
                {
                    return res[resource];
                }
            }
            return 0;
        }

        public void CheckForVictory()
        {
            Dictionary<ICharacter, int> keyValuePairs = new Dictionary<ICharacter, int>();

            bool victory = false;

            foreach(var item in CharacterResources)
            {
                int count = 0;

                foreach(var resource in item.Value)
                {
                    count += LeftCollectResource(resource.Key, item.Key);
                }
                keyValuePairs.Add(item.Key, count);
                if(count == 0)
                {
                    victory = true;
                }
            }

            if(victory)
            {
                var sortedKeyValuePairs = keyValuePairs.OrderBy(kvp => kvp.Value).ToList();

                string message = "Victory!";

                foreach(var kvp in sortedKeyValuePairs)
                {
                    string str = $"Character: {kvp.Key.CharacterInfo.Name}, Count: {kvp.Value} \n";
                    message += str;
                    Debug.Log(str);
                }

                AlertUI.Instance.ShowAlert(message, 10f);

                OnVictory?.Invoke();
            }
        }
    }
}
