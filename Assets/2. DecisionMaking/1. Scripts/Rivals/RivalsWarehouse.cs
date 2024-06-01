using DecisionMaking.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace DecisionMaking
{
    public class RivalsWarehouse : ResourcesStorage
    {
        private const string LevelKey = "CurrentLevel";

        public int CurrentLevel
        {
            get
            {
                return PlayerPrefs.GetInt(LevelKey, 1);
            }
            set
            {
                PlayerPrefs.SetInt(LevelKey, value);
                PlayerPrefs.Save();
            }
        }

        [SerializeField] Transform position;
        [SerializeField] LocatorOfCollectedResources locatorOfCollectedResources;

        CostInResourcesData necessaryResourcesForVictory;

        Dictionary<ICharacter, Dictionary<ResourceData, int>> characterResources = new Dictionary<ICharacter, Dictionary<ResourceData, int>>();

        bool isShowed = false;
        public override LocatorOfCollectedResources LocatorOfCollectedResources => locatorOfCollectedResources;
        public Vector3 Position => position.position;
        public Dictionary<ICharacter, Dictionary<ResourceData, int>> CharacterResources => characterResources;

        public override int ResourcesCount => 0;

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

            if(victory && !isShowed)
            {
                isShowed = true;
                var sortedKeyValuePairs = keyValuePairs.OrderBy(kvp => kvp.Value).ToList();

                string strLine = $"-------------------------------------------------------- \n";

                string message = "Results: \n";
                message += strLine;

                int currentLevel = CurrentLevel;
                CurrentLevel++;

                int BTPlace = GetPlace(sortedKeyValuePairs, AlgoritmType.BT);
                int BTRemainsToBeAssembled = GetRemainsToBeAssembled(sortedKeyValuePairs, AlgoritmType.BT);
                int SMPlace = GetPlace(sortedKeyValuePairs, AlgoritmType.SM);
                int SMRemainsToBeAssembled = GetRemainsToBeAssembled(sortedKeyValuePairs, AlgoritmType.SM);
                int TBPlace = GetPlace(sortedKeyValuePairs, AlgoritmType.TB);
                int TBRemainsToBeAssembled = GetRemainsToBeAssembled(sortedKeyValuePairs, AlgoritmType.TB);
                int RSPlace = GetPlace(sortedKeyValuePairs, AlgoritmType.RS);
                int RSRemainsToBeAssembled = GetRemainsToBeAssembled(sortedKeyValuePairs, AlgoritmType.RS);

                WriteResultsToExcel(currentLevel,
                    BTPlace, BTRemainsToBeAssembled,
                    SMPlace, SMRemainsToBeAssembled,
                    TBPlace, TBRemainsToBeAssembled,
                    RSPlace, RSRemainsToBeAssembled);

                message += $"{AlgoritmType.BT}, p-{BTPlace}, r-{BTRemainsToBeAssembled} \n ";
                message += $"{AlgoritmType.SM}, p-{SMPlace}, r-{SMRemainsToBeAssembled} \n ";
                message += $"{AlgoritmType.TB}, p-{TBPlace}, r-{TBRemainsToBeAssembled} \n ";
                message += $"{AlgoritmType.RS}, p-{RSPlace}, r-{RSRemainsToBeAssembled} \n ";

                for(int i = 0; i < sortedKeyValuePairs.Count; i++)
                {
                    var kvp = sortedKeyValuePairs[i];
                    string str = $"Place: {i + 1}, Character: {kvp.Key.CharacterInfo.Name}, Remains to be assembled: {kvp.Value} \n";
                    message += str;
                    message += strLine;
                    Debug.Log(str);
                }

                //WriteResultsToExcel(currentLevel, )
                AlertUI.Instance.ShowAlert(message, 10f);

                OnVictory?.Invoke();
            }
        }

        private int GetPlace(List<KeyValuePair<ICharacter, int>> keyValuePairs, AlgoritmType algoritmType)
        {
            for(int i = 0; i < keyValuePairs.Count; i++)
            {
                if(keyValuePairs[i].Key.CharacterInfo.AlgoritmType == algoritmType)
                {
                    return i + 1;
                }
            }
            return 0;
        }

        private int GetRemainsToBeAssembled(List<KeyValuePair<ICharacter, int>> keyValuePairs, AlgoritmType algoritmType)
        {
            for(int i = 0; i < keyValuePairs.Count; i++)
            {
                if(keyValuePairs[i].Key.CharacterInfo.AlgoritmType == algoritmType)
                {
                    return keyValuePairs[i].Value;
                }
            }
            return 0;
        }

        private void WriteResultsToExcel(int levelNumber,
            int BTPlace, int BTRemainsToBeAssembled,
            int SMPlace, int SMRemainsToBeAssembled,
            int TBPlace, int TBRemainsToBeAssembled,
            int RSPlace, int RSRemainsToBeAssembled)
        {
            string filePath = "DecisionMaking.csv";

            bool fileExists = File.Exists(filePath);

            using(StreamWriter writer = new StreamWriter(filePath, true))
            {
                if(!fileExists)
                {
                    writer.WriteLine("Level," +
                        "Behaviour Trees Place,Behaviour Trees Remains to be assembled," +
                        "State Mashine Place,State Mashine Remains to be assembled," +
                        "Timer Based Place,Timer Based Remains to be assembled," +
                        "Random Select Place,Random Select Remains to be assembled,");
                }

                writer.WriteLine($"{levelNumber}," +
                    $"{BTPlace},{BTRemainsToBeAssembled}," +
                    $"{SMPlace},{SMRemainsToBeAssembled}," +
                    $"{TBPlace},{TBRemainsToBeAssembled}," +
                    $"{RSPlace},{RSRemainsToBeAssembled}");
            }
        }
    }
}
