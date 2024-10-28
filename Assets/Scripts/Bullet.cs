using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D rb;
    float damage = 10;
    bool entered;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.up = rb.velocity;
    }
    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        if (!entered && !collision.gameObject.GetComponent<CharacterController2D>())
        {
            Actions.BulletHit.Invoke(damage);
            print("hit");
            entered = true;
        }
    }
}
