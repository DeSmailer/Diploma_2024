using UnityEngine;

public abstract class EffectsCreator : MonoBehaviour, IEffectsCreator {
  [SerializeField] protected GameObject vfx;
  [SerializeField] protected AudioClip audioClip;
  [SerializeField] protected float animationDuration = 1f;

  public virtual void Execute() {
    if (vfx != null) {
      Instantiate(vfx, transform.position, Quaternion.identity);
    }
    if (audioClip != null) {
      AudioSource.PlayClipAtPoint(audioClip, transform.position);
    }
  }
}
