using UnityEngine;

/*
 * 	Clase que se encarga de animar al objeto del jugador, cambiando entre estados según el movimiento y/o acciones
 */
public class TP_Animator : MonoBehaviour
{
	//Instancia de la clase (singleton)
	public static TP_Animator Instance;

	//Guarda el componente Animator del objeto de la clase
	Animator animator;

	//Estados de las direcciones del personaje
	public enum Direction
	{
		Stationary, Forward, Backward, Left, Right,
		LeftForward, RightForward, LeftBackward, RightBackward
	}

	//Estados que representan que esta haciendo el personaje
	public enum CharacterState
	{
		Idle, Running, WalkBackwards, StrafingLeft, StrafingRight, Jumping,
		Falling, Landing, Climbing, Sliding, Using, Dead, ActionLocked
	}

	public CharacterState State {get; set; }
	public CharacterState PrevState {get; set; }

	public void SetState(CharacterState newState) {
		PrevState = State;
		State = newState;
	}

	//propiedad que guarda nuestra direccion
	public Direction MoveDirection {get; set; }

	void Awake()
	{
		Instance = this;
		animator = GetComponent<Animator>();
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

	//Determina el estado
	private void DetermineCurrentState()
	{
		if(State == CharacterState.Dead)
			return;

		//corregir fallo no deja de caer
		if(!TP_Controller.Instance.onGround)
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

	//Según el estado en el que nos encontremos, ejecutamos determinadas funciones
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

	private void Idle()
	{
		animator.SetBool("isRunning", false);
		animator.SetBool("isWalkingBackwards", false);
	}

	private void Running()
	{
		animator.SetBool("isRunning", true);
		animator.SetBool("isWalkingBackwards", false);
	}

	private void WalkBackwards()
	{
		animator.SetBool("isRunning", false);
		animator.SetBool("isWalkingBackwards", true);
	}

	private void StrafingLeft()
	{

	}

	private void StrafingRight()
	{

	}

	private void Jumping()
	{
		if(TP_Controller.Instance.onGround)
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

	private void Falling()
	{
		if(TP_Controller.Instance.onGround && GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Falling"))
		{
			animator.SetBool("isLanding", true);
			animator.SetBool("isFalling", false);
			animator.SetBool("isJumping", false);
			SetState(CharacterState.Landing);
		}
	}

	private void Landing()
	{
		//Cuando hemos acabado la animación
		if(animator.GetCurrentAnimatorStateInfo(0).IsName("Landing") && 
			animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
		{
			SetState(CharacterState.Idle);
			animator.SetBool("isLanding", false);
		}
	}

	#endregion

	#region Start Action Method

	public void Jump()
	{
		if(!TP_Controller.Instance.onGround || State == CharacterState.Jumping)
			return;

		if(State == CharacterState.Running)
		{
			animator.SetBool("isRunning", false);
		}

		animator.SetBool("isJumping", true);
		SetState(CharacterState.Jumping);
	}

	public void Fall()
	{
		if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle") || GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Jumping") || GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Running"))
		{
			SetState(CharacterState.Falling);
			//if we are too high do something
			animator.SetBool("isFalling", true);
		}
	}

	#endregion
}
