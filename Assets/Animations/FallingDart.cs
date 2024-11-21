using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingDart : MonoBehaviour
{
    Rigidbody2D m_rigidbody;
    float[] horizontal = { -4.5f, 4.5f };
    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        int temp = (int)Mathf.Ceil(horizontal[1]);
        int range = Functions.random.Next(-temp * 2, temp * 2 + 1);
        float randomX = (float)range / 2 - horizontal[1] % temp;
        Vector3 randomPos = new Vector3(randomX, 12.5f, -1);
        transform.position = randomPos;
        StartCoroutine(Fall());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Fall()
    {
        yield return new WaitForSeconds(1);
        m_rigidbody.gravityScale = 3;
    }

    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        m_rigidbody.gravityScale = 0;
        if (collision.gameObject.GetComponent<CharacterController2D>())
        {
            collision.gameObject.GetComponent<CharacterController2D>().DealDamage(10);
        }
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(1);
        Destroy(this);
    }
}
