using UnityEngine;

public class SpawnEffects : EffectsCreator {
  private void Start() {
    Execute();
  }

  public override void Execute() {
    transform.localScale = Vector3.zero;
    //transform.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutBack);

    base.Execute();
  }
}