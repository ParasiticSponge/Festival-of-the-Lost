using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{ 
    public float movementSpeed = 5;
    public string charName;
    public int canMove = 1;
    public float smoothTime = 0.125f;
    public Vector2 velocity;
    Vector2 smoothMovement;

    public Rigidbody2D rb;

    private void OnEnable()
    {
        Actions.Input += GetName;
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector3 moveDir = new Vector3(horizontal, vertical, 0);
        Vector3 targetVelocity = moveDir * movementSpeed;

        smoothMovement = Vector2.SmoothDamp(smoothMovement, targetVelocity, ref velocity, smoothTime);
        //transform.position = smoothed;

        //rb.velocity = targetVelocity * canMove;
        rb.velocity = smoothMovement * canMove;
    }

    void GetName(string name)
    {
        charName = name;
        //only get first input, then stop listening
        Actions.Input -= GetName;
    }
}
