using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;

public class CharacterController2D : NPC_AI
{
    public bool enter;
    public int doorNum;
	public bool talk;

	private GameObject talkingTo;

    private void OnEnable()
    {
        Actions.Input += GetName;
    }
    private void Awake()
	{
		gameManager = FindObjectOfType<GameManager>();
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		m_Rigidbody2D.gravityScale = gravity;
		animator = GetComponent<Animator>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}
    private void FixedUpdate()
	{
		//horizontal = Input.GetAxisRaw("Horizontal") * runSpeed;
		horizontal *= canMove;

        if (Input.GetKey(KeyCode.Space) && canMove == 1)
        {
            jump = true;
            animator.SetBool("jump", jump);
        }

        animator.SetFloat("velocity", Mathf.Abs(horizontal));
        Move(horizontal * Time.fixedDeltaTime, false, jump);
		Grounded();
    }

	private void OnMove(InputValue input)
	{
		horizontal = input.Get<Vector2>().x * runSpeed;
	}
	private void OnEnter(InputValue input)
	{
		if (enabled)
		{
			if (talk) Actions.Talk.Invoke(talkingTo);
		}
        if (enter) Actions.EnterRoom.Invoke(doorNum);
    }
    private void OnTriggerEnter2D(Collider2D other)
	{
		if (enabled)
		{
			if (Int32.TryParse(other.gameObject.name, out int i))
			{
				Actions.isOverDoor.Invoke(other.gameObject, true);
                doorNum = i;
                enter = true;
			}
			else
			{
                Actions.isOverDoor.Invoke(other.gameObject, true);
                talkingTo = other.gameObject;
                talk = true;
			}
		}
	}
	private void OnTriggerExit2D(Collider2D other)
	{
		if (enabled)
		{
			if (Int32.TryParse(other.gameObject.name, out int i))
			{
				Actions.isOverDoor.Invoke(other.gameObject, false);
				enter = false;
			}
			else
			{
                Actions.isOverDoor.Invoke(other.gameObject, false);
                talk = false;
			}
		}
	}
    private void OnPause(InputValue input)
    {
		if (gameManager.canPause)
			Actions.Pause.Invoke();
    }
}
