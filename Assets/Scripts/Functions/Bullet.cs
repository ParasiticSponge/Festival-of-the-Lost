using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D rb;
    float damage = 10;
    bool entered;
    GameObject target;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        Actions.BulletTarget += Target;
    }
    private void OnDisable()
    {
        Actions.BulletTarget -= Target;
    }
    void Target(GameObject instance)
    {
        target = instance;
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
            if (collision.gameObject == target)
            {
                print("target");
                if (!GameManager.hitNose)
                {
                    GameManager.hitNose = true;
                    StartCoroutine(Play_Menu_Sounds.PlayClip(14, MenuManager_2.sfxVol));
                    StartCoroutine(Play_Menu_Sounds.PlayClip(15, MenuManager_2.sfxVol));
                    TextBox.Text(null, "Poppy", "OUCH! Where did that dart come from? My nose is very sensitive!", 0.005f);
                }
                else
                {
                    Actions.BulletHit.Invoke(damage);
                    entered = true;
                }
            }
        }
        if (collision.gameObject != target)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }
    }
}
