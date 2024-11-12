using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] float maxHealth = 100;
    [SerializeField] float health = 100;
    public float GetHealth() => health;
    private void OnEnable()
    {
        Actions.BulletHit += TakeDamage;
    }
    private void OnDisable()
    {
        Actions.BulletHit -= TakeDamage;
    }
    // Start is called before the first frame update
    void TakeDamage(float damage)
    {
        print(damage);
        health -= damage;
  
        if (health <= 0)
            Destroy(gameObject);
        else if (health <= maxHealth / 3)
            Actions.BossPhase.Invoke(2);
        else if (health <= maxHealth / 3 * 2)
            Actions.BossPhase.Invoke(3);
    }
}
