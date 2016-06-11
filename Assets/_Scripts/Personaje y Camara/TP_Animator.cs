using UnityEngine;
using System.Collections;

public class TP_Animator : MonoBehaviour
{
	//Estados, en este caso, las direcciones del personaje
	public enum Direction
	{
		Stationary, Forward, Backward, Left, Right,
		LeftForward, RightForward, LeftBackward, RightBackward
	}

	public enum CharacterState
	{
		Idle, Running, WalkBackwards, StrafingLeft, StrafingRight, Jumping,
		Falling, Landing, Climbing, Sliding, Using, Dead, ActionLocked
	}
	public static TP_Animator Instance;

	//propiedad que guarda nuestra direccion
	public Direction MoveDirection {get; set; }

	public CharacterState State {get; set; }
	public CharacterState PrevState {get; set; }

	public void SetState(CharacterState newState) {
		PrevState = State;
		State = newState;
	}

	void Awake()
	{
		Instance = this;
	}

	//Determina el Estado de Direction dependiendo del vector de direccion
	public void DetermineCurrentMoveDirection()
	{
		var forward = false;
		var backward = false;
		var left = false;
		var right = false;

		if(TP_Motor.Instance.MoveVector.z > 0) //nos movemos hacia adelante
			forward = true;
		if(TP_Motor.Instance.MoveVector.z < 0) //nos movemos hacia atras
			backward = true;
		if(TP_Motor.Instance.MoveVector.x > 0) //nos movemos hacia la derecha
			right = true;
		if(TP_Motor.Instance.MoveVector.x < 0) //nos movemos hacia la izquierda
			left = true;

		if (forward)
		{
			if(left)
				MoveDirection = Direction.LeftForward;
			else if (right)
				MoveDirection = Direction.RightForward;
			else
				MoveDirection = Direction.Forward;
		}
		else if (backward)
		{
			if(left)
				MoveDirection = Direction.LeftBackward;
			else if (right)
				MoveDirection = Direction.RightBackward;
			else
				MoveDirection = Direction.Backward;
		}
		else if (left)
			MoveDirection = Direction.Left;
		else if (right)
			MoveDirection = Direction.Right;
		//No nos movemos
		else
			MoveDirection = Direction.Stationary;
	}

	void Update()
	{
		DetermineCurrentState();
		ProcessCurrentState();
	}

	private void DetermineCurrentState()
	{
		if(State == CharacterState.Dead)
			return;

		if(!TP_Controller.CharacterController.isGrounded)
		{
			if(State != CharacterState.Falling &&
			   State != CharacterState.Jumping && 
			   State != CharacterState.Landing)
			{
				//Debemos estar callendo
				Fall();
			}
		}

		if(State != CharacterState.Falling &&
		   State != CharacterState.Jumping && 
		   State != CharacterState.Landing &&
		   State != CharacterState.Using &&
		   State != CharacterState.Climbing &&
		   State != CharacterState.Sliding)
		{
			switch(MoveDirection)
			{
			case Direction.Stationary:
				SetState(CharacterState.Idle);
				break;
			case Direction.Forward:
				SetState(CharacterState.Running);
				break;
			case Direction.Backward:
				SetState(CharacterState.WalkBackwards);
				break;
			case Direction.Left:
				SetState(CharacterState.StrafingLeft);
				break;
			case Direction.Right:
				SetState(CharacterState.StrafingRight);
				break;
			case Direction.LeftForward:
				SetState(CharacterState.Running);
				break;
			case Direction.RightForward:
				SetState(CharacterState.Running);
				break;
			case Direction.LeftBackward:
				SetState(CharacterState.WalkBackwards);
				break;
			case Direction.RightBackward:
				SetState(CharacterState.WalkBackwards);
				break;
			
			}
		}
	}

	private void ProcessCurrentState()
	{
		switch(State)
		{
		case CharacterState.Idle:
			Idle();
			break;
		case CharacterState.Running:
			Running();
			break;
		case CharacterState.WalkBackwards:
			WalkBackwards();
			break;
		//Caminar hacia al lado sin dejar de mirar al mismo sitio
		case CharacterState.StrafingLeft:
//			StrafingLeft();
			break;
		case CharacterState.StrafingRight:
//			StrafingRight();
			break;
		case CharacterState.Jumping:
			Jumping();
			break;
		case CharacterState.Falling:
			Falling();
			break;
		case CharacterState.Landing:
			Landing();
			break;
		case CharacterState.Climbing:
			break;
		case CharacterState.Sliding:
			break;
		case CharacterState.Using:
			break;
		case CharacterState.Dead:
			break;
		case CharacterState.ActionLocked:
			break;
		}
	}

	#region Character State Methods

	void Idle()
	{
		Animator anim=GetComponent<Animator>();
		anim.SetBool("isRunning", false);
		anim.SetBool("isWalkingBackwards", false);
	}

	void Running()
	{
		Animator anim=GetComponent<Animator>();
		anim.SetBool("isRunning", true);
		anim.SetBool("isWalkingBackwards", false);
	}

	void WalkBackwards()
	{
		Animator anim=GetComponent<Animator>();
		anim.SetBool("isRunning", false);
		anim.SetBool("isWalkingBackwards", true);
	}

	void StrafingLeft()
	{
//		Animator anim=GetComponent<Animator>();
	}

	void StrafingRight()
	{
//		Animator anim=GetComponent<Animator>();
	}

	void Jumping()
	{
		Animator anim=GetComponent<Animator>();

		if(TP_Controller.CharacterController.isGrounded)
		{
			//crouching
		}
		//Si se está ejecutando la animación de salto
		else if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Jumping"))
		{
			SetState(CharacterState.Falling);
		}
		else
		{
			SetState(CharacterState.Jumping);
			//Help if we fell too far
		}
	}

	void Falling()
	{
		if(TP_Controller.CharacterController.isGrounded)
		{
			Animator anim=GetComponent<Animator>();
			anim.SetBool("isLanding", true);
			anim.SetBool("isFalling", false);
			anim.SetBool("isJumping", false);
			SetState(CharacterState.Landing);
		}
	}

	void Landing()
	{
		Animator anim=GetComponent<Animator>();
		//Cuando hemos acabado la animación
		if(anim.GetCurrentAnimatorStateInfo(0).IsName("Landing") && 
			anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
		{
			SetState(CharacterState.Idle);
			anim.SetBool("isLanding", false);
		}
	}

	#endregion

	#region Start Action Method

	public void Jump()
	{
		if(!TP_Controller.CharacterController.isGrounded || State == CharacterState.Jumping)
			return;

		Animator anim=GetComponent<Animator>();

		if(State == CharacterState.Running)
		{
			anim.SetBool("isRunning", false);
		}
		

		anim.SetBool("isJumping", true);
		SetState(CharacterState.Jumping);
	}

	public void Fall()
	{
		SetState(CharacterState.Falling);
		//if we are too high do something
		Animator anim=GetComponent<Animator>();
		anim.SetBool("isFalling", true);
	}

	#endregion
}
