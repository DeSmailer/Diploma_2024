using UnityEngine;
using UnityEngine.Events;

namespace UnityUtils {

  public class Health : MonoBehaviour {
    [SerializeField] float maxHealth = 100;

    float currentHealth;

    public bool IsDead { get; private set; }

    public UnityEvent<float, float> OnChange; // currentHealth, maxHealth
    public UnityEvent<float> OnTakeDamage;    // damage
    public UnityEvent OnDead;

    void Awake() {
      currentHealth = maxHealth;
      IsDead = false;
    }

    public void SetMaxHealth(float health, bool changeCurrent = true) {
      maxHealth = health;

      if (changeCurrent) {
        currentHealth = maxHealth;
      }
    }

    public void TakeDamage(float damage) {
      if (!IsDead) {
        currentHealth -= damage;
        OnTakeDamage.Invoke(damage);
        OnChange.Invoke(currentHealth, maxHealth);
        if (currentHealth <= 0) {
          IsDead = true;
          OnDead.Invoke();
        }
      }
    }
  }  
}
