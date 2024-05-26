using System.Collections;
using FarmRunner.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace FarmRunner {
  public class CollectedResource : MonoBehaviour {

    [SerializeField] AnimationCurve flySpeedToInventoryCurve;
    [SerializeField] AnimationCurve flySpeedToWarehouseCurve;
    [SerializeField] AnimationCurve dropSpeedCurve;

    [SerializeField] int index;
    ResourceData resourceData;

    Coroutine addToInventoryCoroutine = null;
    Coroutine addToWarehouseCoroutine = null;
    Coroutine dropOnGroundCoroutine = null;

    public ResourceData ResourceData => resourceData;

    public UnityEvent<CollectedResource> OnInventoryMovementEnded;
    public UnityEvent<CollectedResource> OnWarehouseMovementEnded;
    public UnityEvent<CollectedResource> OnGroundMovementEnded;


    public void Initialize(ResourceData resourceData) {
      this.resourceData = resourceData;
    }

    public void AddToInventory(ResourcesStorage inventory, float delay) {
      StopAllMyCoroutine();
      addToInventoryCoroutine = StartCoroutine(MoveToInventory(inventory, delay));
    }

    public void AddToWarehouse(ResourcesStorage warehouse, float delay) {
      addToWarehouseCoroutine = StartCoroutine(MoveToWarehouse(warehouse, delay));
    }

    public void DropOnGround(Vector3 endPosition, Vector3 endRotation, float delay) {
      StopAllMyCoroutine();
      dropOnGroundCoroutine = StartCoroutine(MoveOnGround(endPosition, endRotation, delay));
    }

    IEnumerator MoveToInventory(ResourcesStorage inventory, float delay) {
      index = inventory.ResourcesCount - 1;
      Vector3 startPosition = transform.position;
      Quaternion startQuaternion = transform.rotation;

      yield return new WaitForSeconds(delay);

      float elapsedTime = 0f;

      Vector3 endPosition;
      while (elapsedTime < GlobalConstants.flyToInventoryDuration) {
        float t = elapsedTime / GlobalConstants.flyToInventoryDuration;

        float curveValue = flySpeedToInventoryCurve.Evaluate(t);

        endPosition = inventory.LocatorOfCollectedResources.GetPosition(index);
        Vector3 peakPosition = (startPosition + endPosition) / 2 + Vector3.up * GlobalConstants.heightOfPeakPositionOfParabola;

        Vector3 currentPosition = CalculateParabolicPosition(startPosition, peakPosition, endPosition, curveValue);

        transform.position = currentPosition;
        transform.rotation = CalculateRotation(startQuaternion, inventory.LocatorOfCollectedResources.transform, curveValue);
        elapsedTime += Time.deltaTime;
        yield return null;
      }

      endPosition = inventory.LocatorOfCollectedResources.GetPosition(index);
      transform.position = endPosition;
      transform.parent = inventory.LocatorOfCollectedResources.transform;
      transform.rotation = inventory.LocatorOfCollectedResources.transform.rotation;

      OnInventoryMovementEnded.Invoke(this);
    }

    IEnumerator MoveToWarehouse(ResourcesStorage warehouse, float delay) {
      yield return new WaitForSeconds(delay);

      StopMyCoroutine(addToInventoryCoroutine);
      StopMyCoroutine(dropOnGroundCoroutine);

      index = warehouse.ResourcesCount;
      //warehouse.AddResource(this);

      Vector3 startPosition = transform.position;
      Quaternion startQuaternion;

      float elapsedTime = 0f;

      Vector3 endPosition = warehouse.LocatorOfCollectedResources.GetPosition(index);
      Vector3 peakPosition = (startPosition + endPosition) / 2 + Vector3.up * GlobalConstants.heightOfPeakPositionOfParabola;
      while (elapsedTime < GlobalConstants.flyToWarehouseDuration) {
        float t = elapsedTime / GlobalConstants.flyToWarehouseDuration;
        startQuaternion = transform.rotation;

        float curveValue = flySpeedToWarehouseCurve.Evaluate(t);

        Vector3 currentPosition = CalculateParabolicPosition(startPosition, peakPosition, endPosition, curveValue);

        transform.position = currentPosition;
        transform.rotation = CalculateRotation(startQuaternion, warehouse.LocatorOfCollectedResources.transform, curveValue);
        elapsedTime += Time.deltaTime;
        yield return null;
      }

      transform.position = endPosition;
      transform.parent = warehouse.transform;
      transform.rotation = warehouse.LocatorOfCollectedResources.transform.rotation;

      OnWarehouseMovementEnded.Invoke(this);
      Destroy(gameObject);
    }

    IEnumerator MoveOnGround(Vector3 endPosition, Vector3 finalRotation, float delay) {
      yield return new WaitForSeconds(delay);
      transform.parent = null;
      Vector3 startPosition = transform.position;
      Quaternion startQuaternion;
      Quaternion endRotation = Quaternion.Euler(finalRotation);
      float elapsedTime = 0f;

      Vector3 peakPosition = (startPosition + endPosition) / 2 + Vector3.up * GlobalConstants.heightOfPeakPositionOfParabolaOfDroppOfDropOnGround;
      while (elapsedTime < GlobalConstants.dropOnGroundDuration) {
        float t = elapsedTime / GlobalConstants.dropOnGroundDuration;
        startQuaternion = transform.rotation;

        float curveValue = dropSpeedCurve.Evaluate(t);

        Vector3 currentPosition = CalculateParabolicPosition(startPosition, peakPosition, endPosition, curveValue);

        transform.position = currentPosition;
        transform.rotation = CalculateRotation(startQuaternion, endRotation, curveValue);
        elapsedTime += Time.deltaTime;
        yield return null;
      }

      transform.position = endPosition;
      transform.rotation = endRotation;

      OnGroundMovementEnded.Invoke(this);

      Destroy(gameObject, 5f + Random.Range(-0.5f, 0.5f));
    }

    private void SwayFromSideToSide() {
      transform.LeanMoveLocalX(Random.Range(-0.1f, 0.1f), 1f).setEaseInOutSine().setLoopPingPong();
    }

    public void Disappear(float delay = 1.5f) {
      transform.LeanScale(Vector3.zero, delay).setEaseOutCubic().setOnComplete(() => Destroy(gameObject));
    }

    Vector3 CalculateParabolicPosition(Vector3 start, Vector3 peak, Vector3 end, float t) {
      Vector3 startToPeak = Vector3.Lerp(start, peak, t);
      Vector3 peakToEnd = Vector3.Lerp(peak, end, t);
      return Vector3.Lerp(startToPeak, peakToEnd, t);
    }

    Quaternion CalculateRotation(Quaternion startQuaternion, Transform target, float t) {
      return CalculateRotation(startQuaternion, target.rotation, t);
    }

    Quaternion CalculateRotation(Quaternion startQuaternion, Quaternion endRotation, float t) {
      return Quaternion.Lerp(startQuaternion, endRotation, t);
    }

    private void StopMyCoroutine(Coroutine coroutine) {
      if (coroutine != null) {
        StopCoroutine(coroutine);
        coroutine = null;
      }
    }

    private void StopAllMyCoroutine() {
      StopMyCoroutine(addToInventoryCoroutine);
      StopMyCoroutine(addToWarehouseCoroutine);
      StopMyCoroutine(dropOnGroundCoroutine);
    }

    public void Reindex(int newIndex) {
      index = newIndex;
    }
  }
}
