using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class FishAI : MonoBehaviour
{
    public enum STATES
    {
        idleSwim,
        swim,
        idle,
        follow,
    }
    public enum TYPE
    {
        angler = 1,
        bass = 3,
        clown = 5
    }
    public STATES state = STATES.idleSwim;
    public TYPE type;

    Vector3 initialPos;
    Vector3 position;

    int wanderRange = 10;
    float idleProbability = 0.5f;

    System.Random random = new System.Random();
    public GameManager gameManager;
    GameObject character;

    public float swimSpeed = 0.1f;
    public float gravity = 0;

    protected Animator animator;
    [Range(0, .3f)][SerializeField] protected float m_MovementSmoothing = .05f;   // How much to smooth out the movement

    protected Rigidbody2D m_Rigidbody2D;
    protected bool m_FacingRight = false;  // For determining which way the player is currently facing.
    protected Vector3 m_Velocity = Vector3.zero;

    float distance;
    public bool hooked;
    bool swim = true;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        //m_Rigidbody2D.gravityScale = gravity;
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        character = FindObjectOfType<CharacterController2D>().gameObject;

        initialPos = transform.position;
    }
    private void OnEnable()
    {
        Switch();
    }
    // Update is called once per frame
    private void Switch()
    {
        switch (state)
        {
            case STATES.idleSwim:
                StartCoroutine(Swim());
                break;
            case STATES.idle:
                StartCoroutine(Idle());
                break;
        }
    }
    private IEnumerator Swim()
    {
        position = Functions.Around(initialPos, wanderRange);
        //position = new Vector3(-200, -140, 0);
        Vector3 vector = position - transform.position;
        distance = vector.magnitude;

        Vector3 normalized = vector.normalized;
        float angle = Mathf.Atan2(normalized.y, normalized.x) * Mathf.Rad2Deg;
        if (angle >= -90 && angle <= 90)
        {
            Vector3 theScale = transform.localScale;
            theScale.x = -1;
            transform.localScale = theScale;
            transform.Rotate(0, 0, angle);
        }
        else
        {
            Vector3 theScale = transform.localScale;
            theScale.x = 1;
            transform.localScale = theScale;
            transform.Rotate(0, 0, 180 - angle);
        }

        while (distance > 0.1f)
        {
            vector = position - transform.position;
            distance = vector.magnitude;
            angle = Mathf.Atan2(position.y, position.x);
            transform.localRotation = Quaternion.Euler(0, 0, angle);

            transform.position += vector.normalized * swimSpeed;
            yield return null;
        }

        //Switch();
        initialPos = transform.position;
        StartCoroutine(Idle());
    }
    private IEnumerator Idle()
    {
        //3-5
        int waitTime = random.Next(1, 2);
        yield return new WaitForSeconds(waitTime);

        Switch();
    }
    protected void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private IEnumerator Follow(GameObject target)
    {
        Vector3 distance = (target.transform.position - transform.position);
        float magnitude = distance.magnitude;

        while (magnitude > 0.1f)
        {
            if (character.GetComponent<RodController>().hasFish && !hooked) 
            {
                StartCoroutine(Swim());
                yield break; 
            }
            distance = (target.transform.position - transform.position);
            magnitude = distance.magnitude;
            transform.position += distance.normalized * swimSpeed;
            yield return null;
        }
        StopAllCoroutines();
        hooked = true;
        transform.parent = target.transform;
        Actions.Reel.Invoke(type);
    }

    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        //position = vector.normalized * magnitude;
        //y = y/-1
        //x = x/y but 0 needs to be 1

        if (collision.gameObject.GetComponent<TemporaryEdgeDetection>())
        {
            Vector3 vector = (position - transform.position);
            Vector2 vector2D = new Vector2(vector.x, vector.y);
            Vector2 normal = collision.gameObject.GetComponent<TemporaryEdgeDetection>().normal;
            Vector2 reflection = Functions.ReflectionVector(normal, vector2D);
            Vector3 reflection3D = new Vector3(reflection.x, reflection.y, 0);
            print(reflection3D);

            position = (transform.position + reflection3D);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Bobber")
        {
            StopAllCoroutines();
            state = STATES.follow;
            StartCoroutine(Follow(collision.gameObject));
        }
    }

    /*IEnumerator countdown()
    {
        for (float timer = 5; timer >= 0; timer -= Time.deltaTime)
        {
            if (secondChanceUsed)
            {
                win();
                secondChanceUsed = false;
                yield break;
            }
            yield return null;
        }
        enableLooseUI();
        disablePreLooseUI();
        updateTotalPoints();
        pointsystem.resetLocalscore();
    }*/
}
