using UnityEngine;

namespace Game {

  public class FXSpawner: MonoBehaviour {
    [System.Serializable]
    public class FXData {
      public string name;
      public GameObject vfxPrefab;
      public GameObject[] vfxRandom;
    }
    [SerializeField] FXData[] fxData = {};

    FXData GetFXData(string name) {
      foreach (var data in fxData) {
        if (data.name == name) {
          return data;
        }
      }
      return new FXData();
    }

    public void Spawn(string name) {
      Spawn(name, transform.position, transform.rotation);
    }

    public void Spawn(string name, Vector3 pos, Quaternion quat) {
      var fxData = GetFXData(name);
      var vfx = fxData.vfxPrefab;
      if (fxData.vfxRandom != null && fxData.vfxRandom.Length > 0) {
        vfx = fxData.vfxRandom[Random.Range(0, fxData.vfxRandom.Length)];
      }
      if (vfx) {
        Instantiate(vfx, pos,  quat);
      }
    }
  }
  
}
