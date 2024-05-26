using System.Collections;
using TMPro;
using UnityEngine;
using UnityUtils;

namespace DecisionMaking {
  public class AlertUI : Singleton<AlertUI> {
    [SerializeField] GameObject messageBox;
    [SerializeField] TMP_Text text;

    private void Start() {
      HideMessageBox();
    }

    public void ShowAlert(string message, float duration) {
      StopAllCoroutines();
      text.text = message;
      messageBox.SetActive(true);
      StartCoroutine(HideMessageBox(duration));
    }

    private IEnumerator HideMessageBox(float delay) {
      yield return new WaitForSeconds(delay);
      HideMessageBox();
    }

    public void HideMessageBox() {
      text.text = "";
      messageBox.SetActive(false);
    }
  }
}
