using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using UnityEngine.Windows;

public class CharacterController2D : NPC_AI
{
    public int doorNum;
	public int choice = 0;
	public bool enter;
	[SerializeField] int health = 100;

	private GameObject talkingTo;
	public List<Collider2D> collisions = new List<Collider2D>();

	//references to buttons
	private PlayerInput playerInput;
	private InputAction touchPress;
	private InputAction keyboardPress;

	Vector3 touchPosition;
    private void OnEnable()
    {
        Actions.Input += GetName;
		//touchPress.performed += OnTouchBegin;
		touchPress.canceled += OnTouchExit;
		keyboardPress.canceled += OnTouchExit;
	}
    private void OnDisable()
    {
        Actions.Input -= GetName;
		//touchPress.performed -= OnTouchBegin;
		touchPress.canceled -= OnTouchExit;
        keyboardPress.canceled -= OnTouchExit;

        collisions.Clear();
    }
    private void Awake()
	{
		playerInput = GetComponent<PlayerInput>();
		touchPress = playerInput.actions["Touch"];
		keyboardPress = playerInput.actions["Move"];

		gameManager = FindObjectOfType<GameManager>();
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		m_Rigidbody2D.gravityScale = gravity;
		animator = GetComponent<Animator>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();

		UnityEngine.Input.simulateMouseWithTouches = true;
	}
    private void Update()
    {
		if (collisions.Count > 0)
		{
			foreach (Collider2D collider in collisions)
			{
				Collisions(collider, false);
            }
			Collider2D other = ReturnSmallestDistance(collisions.ToArray());
            Collisions(other, true);
        }

		/*if (UnityEngine.Input.touchCount > 0)
		{
			Vector3 touchPosition = UnityEngine.Input.GetTouch(0).position;
			float width = Screen.width / 3;
			if (touchPosition.x > width * 2)
				horizontal = 1 * runSpeed;
			else if (touchPosition.x < width)
				horizontal = -1 * runSpeed;
			else
				print("middle");
				//OnEnter(Input.GetTouch(0));
		}
		else
			horizontal = 0;*/
	}
	public void Collisions(Collider2D collider, bool condition)
	{
        if (Int32.TryParse(collider.gameObject.name, out int j))
        {
            Actions.isOverDoor.Invoke(collider.gameObject, condition);
            enter = condition;
        }
        if (collider.CompareTag("Plushie"))
        {
            if (collider.transform.childCount > 0) Actions.isOverDoor.Invoke(collider.gameObject, condition);
			if (!condition) choice = 0;
			else
			{
                talkingTo = collider.gameObject;
                choice = 2;
            }
        }
        else if (collider.gameObject.GetComponent<NPC_AI>())
        {
            Actions.isOverDoor.Invoke(collider.gameObject, condition);
            if (!condition) choice = 0;
			else
			{
                talkingTo = collider.gameObject;
                choice = 1;
            }
        }
    }
	public Collider2D ReturnSmallestDistance(Collider2D[] colliderList)
	{
		Collider2D target = colliderList[0];
		float length = (colliderList[0].gameObject.transform.position - transform.position).magnitude;
        foreach (Collider2D collider in colliderList)
        {
			float temp = (collider.gameObject.transform.position - transform.position).magnitude;
			if (temp < length)
			{
				target = collider;
				length = temp;
			}
        }

		return target;
    }
    private void FixedUpdate()
	{
		//horizontal = Input.GetAxisRaw("Horizontal") * runSpeed;
		horizontal *= canMove;

        if (UnityEngine.Input.GetKey(KeyCode.Space) && canMove == 1 && canJump == true)
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
		if (enabled)
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
			{
                TextBox text = FindObjectOfType<TextBox>();
				if (text == null)
				{
					Actions.EnterRoom.Invoke(doorNum);
                }
            }
		}
    }
	private void OnTouch(InputValue input)
	{
        if (enabled)
        {
            touchPosition = Touchscreen.current.position.ReadValue();

            float width = Screen.width / 3;
            if (touchPosition.x > width * 2)
                horizontal = 1 * runSpeed;
            else if (touchPosition.x < width)
                horizontal = -1 * runSpeed;
            else
                OnEnter(input);
        }
    }
    private void OnTouchExit(InputAction.CallbackContext input)
	{
		horizontal = 0;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (enabled)
		{
			collisions.Add(other);

			if (other.name == "Bucket")
			{
				gameManager.GainDarts();
			}
		}

		/*if (enabled)
		{
			*//*if (Int32.TryParse(other.gameObject.name, out int i))
			{
				Actions.isOverDoor.Invoke(other.gameObject, true);
				print("entered");
				enter = true;
			}*//*
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
		}*/
	}
	private void OnTriggerExit2D(Collider2D other)
	{
		if (enabled)
		{
			Collisions(other, false);

			Collider2D target = other;
            foreach (Collider2D collider in collisions)
            {
                if (other == collider)
                    target = collider;
            }
            collisions.Remove(target);
        }
	}
    private void OnPause(InputValue input)
    {
		if (gameManager.canPause)
			Actions.Pause.Invoke();
    }

	public void DealDamage(int damage)
	{
		health -= damage;
		if (health <= 0)
			gameManager.ResetBossFight();
	}
}
