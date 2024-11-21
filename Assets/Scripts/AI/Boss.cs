using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] float maxHealth = 100;
    [SerializeField] float health = 100;
    int phase = 1;
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
        health -= damage;

        if (health <= 0 && phase == 3)
        {
            phase++;
            Actions.BossPhase.Invoke(4);
        }
        if (health <= 66 && phase == 1)
        {
            phase++;
            Actions.BossPhase.Invoke(2);
        }
        if (health <= 33 && phase == 2)
        {
            phase++;
            Actions.BossPhase.Invoke(3);
        }
    }
}
