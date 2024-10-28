using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
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
    }
}
