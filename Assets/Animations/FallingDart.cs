using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingDart : MonoBehaviour
{
    Rigidbody2D m_rigidbody;
    public float horizontal = 4.5f;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Fall());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Fall()
    {
        int temp = (int)Mathf.Floor(horizontal);
        int range = Functions.random.Next(-temp * 2, temp * 2 + 1);
        float randomX = (float)range / 2;
        Vector3 randomPos = new Vector3(randomX, 12.5f, -1);
        transform.localPosition = randomPos;

        Vector3 movePos = transform.localPosition + new Vector3(0, -1.5f, 0);
        yield return StartCoroutine(Functions.MoveCubic(transform.localPosition, movePos, value => transform.localPosition = value));

        GetComponent<Animator>().Play("FallingDart", 0, 0);
        yield return new WaitForSeconds(1);

        print("change gravity");
        m_rigidbody.gravityScale = 3;
    }

    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        print("should destroy");
        if (collision.gameObject.GetComponent<CharacterController2D>())
        {
            collision.gameObject.GetComponent<CharacterController2D>().DealDamage(10);
        }
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
