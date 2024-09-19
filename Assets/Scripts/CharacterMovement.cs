using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{ 
    public float movementSpeed = 4;
    public string charName;
    public int canMove = 1;
    public float smoothTime = 0.125f;
    public Vector2 velocity;

    Rigidbody2D rb;

    private void OnEnable()
    {
        Actions.Input += GetName;
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector3 moveDir = new Vector3(horizontal, vertical, 0);
        Vector3 targetVelocity = moveDir * movementSpeed;

        //Vector3 smoothed = Vector2.SmoothDamp(transform.position, targetVelocity, ref velocity, smoothTime);
        //transform.position = smoothed;

        rb.velocity = targetVelocity * canMove;
    }

    void GetName(string name)
    {
        charName = name;
        //only get first input, then stop listening
        Actions.Input -= GetName;
    }
}
