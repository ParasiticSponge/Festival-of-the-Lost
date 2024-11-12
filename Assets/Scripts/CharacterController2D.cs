using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;

public class CharacterController2D : NPC_AI
{
    public int doorNum;
	public int choice = 0;
	public bool enter;

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

        if (Input.GetKey(KeyCode.Space) && canMove == 1 && canJump == true)
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
			switch (choice)
			{
				case 1:
					Actions.Talk.Invoke(talkingTo);
					break;
				case 2:
					Actions.FoundPlushie.Invoke(talkingTo);
					break;
			}
			if (enter)
				Actions.EnterRoom.Invoke(doorNum);
		}
    }
    private void OnTriggerEnter2D(Collider2D other)
	{
		if (enabled)
		{
			/*if (Int32.TryParse(other.gameObject.name, out int i))
			{
				Actions.isOverDoor.Invoke(other.gameObject, true);
				print("entered");
				enter = true;
			}*/
			if (other.CompareTag("Plushie"))
			{
                if (other.transform.childCount > 0) Actions.isOverDoor.Invoke(other.gameObject, true);
                talkingTo = other.gameObject;
				choice = 2;
			}
			else
			{
                Actions.isOverDoor.Invoke(other.gameObject, true);
                talkingTo = other.gameObject;
				choice = 1;
			}
		}
	}
	private void OnTriggerExit2D(Collider2D other)
	{
		if (enabled)
		{
			/*if (Int32.TryParse(other.gameObject.name, out int i))
			{
				Actions.isOverDoor.Invoke(other.gameObject, false);
				print("exited");
				enter = false;
			}*/
            if (other.CompareTag("Plushie"))
            {
                if (other.transform.childCount > 0) Actions.isOverDoor.Invoke(other.gameObject, false);
                choice = 0;
            }
            else
			{
                Actions.isOverDoor.Invoke(other.gameObject, false);
				choice = 0;
			}
		}
	}
    private void OnPause(InputValue input)
    {
		if (gameManager.canPause)
			Actions.Pause.Invoke();
    }
}
