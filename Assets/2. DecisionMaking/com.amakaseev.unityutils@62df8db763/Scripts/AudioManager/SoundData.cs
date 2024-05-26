using UnityEngine;

namespace UnityUtils {

  [CreateAssetMenu(fileName = "SoundData", menuName = "AudioManager/SoundData")]
  public class SoundData: ScriptableObject {
    public AudioClip sound;
    [Range(0.1f, 1)] public float randomVolume = 0.2f;
    [Range(0.1f, 1)] public float sameTime = 0.2f;
    [Range(0.1f, 1)] public float volume = 1;

    public void Play(AudioSource audioSource) {
      audioSource.PlayOneShot(sound, volume * Random.Range(1f - randomVolume, 1f));
    }
  }

}
