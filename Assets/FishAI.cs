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
            case STATES.follow:
                StartCoroutine(Follow(character.transform.position, 10));
                break;

        }
    }
    private IEnumerator Swim()
    {
        position = Functions.Around(initialPos, wanderRange);
        //position = new Vector3(-200, -140, 0);
        Vector3 vector = position - transform.position;
        float distance = vector.magnitude;

        Vector3 normalized = vector.normalized;
        float angle = Mathf.Atan2(normalized.y, normalized.x) * Mathf.Rad2Deg;
        if (angle >= -90 && angle <= 90)
        {
            Vector3 theScale = transform.localScale;
            theScale.x = -1;
            transform.localScale = theScale;
            transform.Rotate(0, 0, angle);
            print(angle);
        }
        else
        {
            Vector3 theScale = transform.localScale;
            theScale.x = 1;
            transform.localScale = theScale;
            transform.Rotate(0, 0, 180 - angle);
            print(180 - angle);
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
        print("idle");
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

    private IEnumerator Follow(Vector3 target, float edge)
    {
        /*Vector3 distance = (target - transform.position);
        float magnitude = distance.magnitude;

        horizontal = Mathf.Sign(distance.x) * 1 * runSpeed;
        if (magnitude <= edge)
            horizontal = 0;
        yield return null;
        Switch();*/
        //Physics2D.IsTouchingLayers(collider, layer);
        yield return null;
    }

    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        //position = vector.normalized * magnitude;
        //y = y/-1
        //x = x/y but 0 needs to be 1

        if (collision.gameObject.GetComponent<TemporaryEdgeDetection>())
        {
            Vector3 vector = (position - transform.position).normalized;
            Vector2 reflected2D = Functions.ReflectionVector(collision.gameObject.GetComponent<TemporaryEdgeDetection>().normal, vector);
            Vector3 reflected = new Vector3(reflected2D.x, reflected2D.y, 0);
            position = transform.localPosition + reflected;
        }
    }
}
