using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class NPC_AI : MonoBehaviour
{
    public enum STATES
    {
        walk,
        idle
    }
    public STATES state = STATES.idle;
    Vector3 initialPos;
    public Sprite appearance;
    public string charName = "NPC";
    int wanderRange = 10;
    float idleProbability = 0.5f;
    bool speaking;
    System.Random random = new System.Random();

    public int canMove = 1;
    public float runSpeed = 20;
    protected bool jump = false;
    protected float horizontal = 0;
    public float gravity = 3;

    protected Animator animator;
    [SerializeField] protected float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0, 1)][SerializeField] protected float m_CrouchSpeed = .36f;           // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)][SerializeField] protected float m_MovementSmoothing = .05f;   // How much to smooth out the movement
    [SerializeField] protected bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] protected LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] protected Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] protected Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] protected Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

    protected const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    protected bool m_Grounded;            // Whether or not the player is grounded.
    protected const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    protected Rigidbody2D m_Rigidbody2D;
    protected bool m_FacingRight = true;  // For determining which way the player is currently facing.
    protected Vector3 m_Velocity = Vector3.zero;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    protected bool m_wasCrouching = false;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Rigidbody2D.gravityScale = gravity;
        animator = GetComponent<Animator>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();

        initialPos = transform.position;
        state = STATES.idle;
    }
    private void OnEnable()
    {
        Switch();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //used for textboxes when they interupt the AI walk cycle
        animator.SetFloat("velocity", Mathf.Abs(0));
        if (canMove == 1)
        {
            animator.SetFloat("velocity", Mathf.Abs(horizontal));
            Move(horizontal * Time.fixedDeltaTime * runSpeed, false, jump);
        }
        Grounded();
        //fix later
        m_Grounded = true;
    }
    private void Switch()
    {
        /*switch (state)
        {
            case STATES.walk:
                yield return StartCoroutine(Walk());
                break;
            case STATES.idle:
                yield return StartCoroutine(Idle());
                break;
        }*/
        if (!speaking)
        {
            //0-100 / 100
            float waitTime = (float)random.Next(101) / 100;
            //probably a more efficient way using Mathf.Ceil(waitTime * numberOfStates) and idlProbability etc...
            if (waitTime <= idleProbability)
                StartCoroutine(Idle());
            else
                StartCoroutine(Walk());
        }
    }
    private IEnumerator Walk()
    {
        float rand = (float)random.Next(-wanderRange, wanderRange + 1);
        float x = initialPos.x + rand;
        //get 1 or -1 by normalising distance
        horizontal = x < transform.position.x? -1 : 1;
        float distance = x - transform.position.x;

        while (horizontal == 1? distance > 0 : distance < 0)
        {
            distance = x - transform.position.x;
            yield return null;
        }

        //Switch();
        StartCoroutine(Idle());
    }
    private IEnumerator Idle()
    {
        //3-5
        int waitTime = random.Next(1, 4);
        horizontal = 0;
        yield return new WaitForSeconds(waitTime);

        //Switch();
        StartCoroutine(Walk());
    }

    public void Move(float move, bool crouch, bool jump)
    {
        // If crouching, check to see if the character can stand up
        if (!crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
            // If crouching
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier
                move *= m_CrouchSpeed;

                // Disable one of the colliders when crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // Enable the collider when not crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        // If the player should jump...
        if (m_Grounded && jump)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
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

    protected void GetName(string name)
    {
        charName = name;
        //only get first input, then stop listening
        Actions.Input -= GetName;
    }

    public void isLanding()
    {
        animator.SetBool("jump", false);
    }

    public void Grounded()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                jump = false;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }
}
