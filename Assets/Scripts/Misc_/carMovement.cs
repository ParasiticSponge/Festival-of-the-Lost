using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carMovement : MonoBehaviour
{
    Animator animator;
    SpriteRenderer render;

    public string charName;
    public int canMove = 1;
    //the magnitude of the force gravity
    float gravity_magnitude = -1f; // This is usually a global/project value
    //detect whether the object is on the ground or not
    bool is_on_ground = false;
    //define the mass of the object
    public float mass = 1.0f;
    //the jump speed of the object
    public float jump_speed = 6.0f;
    //how fast the velocity of the object will be
    public float movement_speed_per_second = 3.0f;
    //how much the acceleration will increase by
    static float acceleration_magnitude_per_second = 0.01f;
    //the maximum velocity potential
    public float maxSpeed = 10;

    //vector that stores the increasing velocity over time
    Vector3 velocity_per_second = Vector3.zero;
    //vector holding the gravitational force to be acted upon the y axis of an object
    Vector3 gravity_per_second = Vector3.zero;
    //calculate how the acceleration force will affect the object
    Vector3 acceleration_per_second = new Vector3(acceleration_magnitude_per_second, 0, 0);
    private void OnEnable()
    {
        Actions.Input += GetName;
    }
    private void Start()
    {
        //apply gravity
        gravity_per_second = new Vector3(0, gravity_magnitude, 0);
        animator = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector3 currentPos = transform.position;
        // -------------------------------------------------------------------------------------------------------------------//
        // Input (AI).
        float horizontal = Input.GetAxis("Horizontal"); // -1 for left, 1 for right
        float jump = Input.GetAxis("Jump"); // 0 for no input, 1 for input

        // direction
        Vector3 horizontal_velocity_direction = new Vector3(horizontal, 0.0f, 0.0f).normalized;

        // initial velocity multiplied by frame time
        Vector3 horizontal_velocity_this_frame = horizontal_velocity_direction * (movement_speed_per_second * Time.deltaTime);

        // Calculate jump
        bool is_jumping = jump > 0.0f;
        bool should_apply_jump_velocity = is_jumping && is_on_ground;
        if (should_apply_jump_velocity)
        {
            Vector3 jump_velocity_this_frame = new Vector3(0.0f, jump_speed, 0.0f);
            velocity_per_second += jump_velocity_this_frame;
        }


        // Apply acceleration for this frame.
        Vector3 acceleration_this_frame = (gravity_per_second / mass) * Time.deltaTime;

        ////reach max acceleration
        ////if the horizontal input is right, multiply by 1 otherwise by -1. Or if not moving, multiply by 0
        //acceleration_this_frame += (acceleration_per_second * horizontal);
        //if the horizontal input is right
        if (horizontal == 1)
        {
            render.flipX = false;
            //reach max acceleration
            acceleration_this_frame += acceleration_per_second;
        }
        //if the horizontal input is left
        else if (horizontal == -1)
        {
            render.flipX = true;
            //accelerating right = negative values
            acceleration_this_frame -= acceleration_per_second;
        }
        else
        //when the object is dormant
        {
            //reset velocity
            velocity_per_second.x = 0;
        }

        // While velocity increases, so does acceleration
        velocity_per_second += acceleration_this_frame;

        // Apply velocity for this frame.
        Vector3 velocity_this_frame = (velocity_per_second * Time.deltaTime) + horizontal_velocity_this_frame;
        transform.position += velocity_this_frame * canMove;

        print(velocity_this_frame);
        //// -------------------------------------------------------------------------------------------------------------------//

        // Apply collision logic (AI).
        bool is_below_ground = transform.position.y < -2.5f;
        bool should_place_on_ground = is_below_ground;
        if (should_place_on_ground)
        {
            transform.position = new Vector3(transform.position.x, -2.5f, 0.0f);
            velocity_per_second = new Vector3(velocity_per_second.x, 0.0f, 0.0f);
            is_on_ground = true;
        }
        else
        {
            is_on_ground = false;
        }

        if (velocity_per_second.x > maxSpeed)
            velocity_per_second.x = maxSpeed;
        else if (velocity_per_second.x < -maxSpeed)
            velocity_per_second.x = -maxSpeed;

        animator.SetFloat("velocity", Mathf.Abs(horizontal));
    }

    void GetName(string name)
    {
        charName = name;
        //only get first input, then stop listening
        Actions.Input -= GetName;
    }
}


