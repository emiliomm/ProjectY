using UnityEngine;

/*
 * 	Clase que se encarga de animar al objeto del jugador, cambiando entre estados según el movimiento y/o acciones
 *  Autor clase original: Tutorial Cámara 3DBuzz (https://www.3dbuzz.com/training/view/3rd-person-character-system)
 * 	Modificada por mí
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

	static int idleState = Animator.StringToHash("Idle");
	static int jumpingState = Animator.StringToHash("Jumping");
	static int landingState = Animator.StringToHash("Landing");
	static int fallingState = Animator.StringToHash("Falling");
	static int runningState = Animator.StringToHash("Running");
	static int walkbackwardsState = Animator.StringToHash("WalkBackwards");

	const int IDLE_STATE = 0;
	const int JUMPING_STATE = 1;
	const int LANDING_STATE = 2;
	const int FALLING_STATE = 3;
	const int RUNNING_STATE = 4;
	const int WALKBACKWARDS_STATE = 5;

	//propiedad que guarda nuestra direccion
	public Direction MoveDirection {get; set; }
	public Direction PrevMoveDirection {get; set; }

	public void SetMoveDirection(Direction newState) {
		PrevMoveDirection = MoveDirection;
		MoveDirection = newState;
	}

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
				SetMoveDirection(Direction.LeftForward);
			else if (right)
				SetMoveDirection(Direction.RightForward);
			else
				SetMoveDirection(Direction.Forward);
		}
		else if (backward)
		{
			if(left)
				SetMoveDirection(Direction.LeftBackward);
			else if (right)
				SetMoveDirection(Direction.RightBackward);
			else
				SetMoveDirection(Direction.Backward);
		}
		else if (left)
			SetMoveDirection(Direction.Left);
		else if (right)
			SetMoveDirection(Direction.Right);
		//No nos movemos
		else
			SetMoveDirection(Direction.Stationary);
	}

	void Update()
	{
		ProcessCurrentState(DetermineCurrentState());
	}

	private bool isInIdle(int currentState)
	{
		return idleState == currentState;
	}

	private bool isInJumping(int currentState)
	{
		return jumpingState == currentState;
	}

	private bool isInLanding(int currentState)
	{
		return landingState == currentState;
	}

	private bool isInFalling(int currentState)
	{
		return fallingState == currentState;
	}

	private bool isInRunning(int currentState)
	{
		return runningState == currentState;
	}

	private bool isInWalkbackwards(int currentState)
	{
		return walkbackwardsState == currentState;
	}

	//Determina el estado
	private int DetermineCurrentState()
	{
		AnimatorStateInfo currentBaseState = animator.GetCurrentAnimatorStateInfo(0);

		int estado = -1;

		if(isInIdle(currentBaseState.shortNameHash))
		{
			estado = IDLE_STATE;
		}
		else if(isInJumping(currentBaseState.shortNameHash))
		{
			estado = JUMPING_STATE;
		}
		else if(isInLanding(currentBaseState.shortNameHash))
		{
			estado = LANDING_STATE;
		}
		else if(isInFalling(currentBaseState.shortNameHash))
		{
			estado = FALLING_STATE;
		}
		else if(isInRunning(currentBaseState.shortNameHash))
		{
			estado = RUNNING_STATE;
		}
		else if(isInWalkbackwards(currentBaseState.shortNameHash))
		{
			estado = WALKBACKWARDS_STATE;
		}

		return estado;
	}

	//Según el estado en el que nos encontremos, ejecutamos determinadas funciones
	private void ProcessCurrentState(int estadoActual)
	{
		switch(estadoActual)
		{
		case IDLE_STATE:
			Idle();
			break;
		case JUMPING_STATE:
			Jumping();
			break;
		case LANDING_STATE:
			Landing();
			break;
		case FALLING_STATE:
			Falling();
			break;
		case RUNNING_STATE:
			Running();
			break;
		case WALKBACKWARDS_STATE:
			WalkBackwards();
			break;
		}
	}

	#region Character State Methods

	private void Idle()
	{
		if(TP_Controller.Instance.onGround)
		{
			switch(MoveDirection)
			{
			case Direction.Stationary:
			case Direction.Left:
			case Direction.Right:
				break;
			case Direction.Forward:
			case Direction.LeftForward:
			case Direction.RightForward:
				animator.SetBool("isRunning", true);
				break;
			case Direction.Backward:
			case Direction.LeftBackward:
			case Direction.RightBackward:
				animator.SetBool("isWalkingBackwards", true);
				break;
			}
		}
		else
			animator.SetBool("isFalling", true);
	}

	private void WalkBackwards()
	{
		if(TP_Controller.Instance.onGround)
		{
			switch(MoveDirection)
			{
			case Direction.Stationary:
				animator.SetBool("isWalkingBackwards", false);
				break;
			case Direction.Forward:
			case Direction.LeftForward:
			case Direction.RightForward:
				animator.SetBool("isWalkingBackwards", false);
				break;
			case Direction.Backward:
			case Direction.LeftBackward:
			case Direction.RightBackward:
			case Direction.Left:
			case Direction.Right:
				break;
			}
		}
		else
			animator.SetBool("isFalling", true);
	}

	private void Running()
	{
		if(TP_Controller.Instance.onGround)
		{
			switch(MoveDirection)
			{
			case Direction.Stationary:
				animator.SetBool("isRunning", false);
				break;
			case Direction.Forward:
			case Direction.Left:
			case Direction.Right:
			case Direction.LeftForward:
			case Direction.RightForward:
				break;
			case Direction.Backward:
			case Direction.LeftBackward:
			case Direction.RightBackward:
				animator.SetBool("isRunning", false);
				break;
			}
		}
		else
			animator.SetBool("isFalling", true);
	}

	private void StrafingLeft()
	{

	}

	private void StrafingRight()
	{

	}

	private void Jumping()
	{
		if(!TP_Controller.Instance.onGround)
		{
			animator.SetBool("isFalling", true);
		}
		else
		{
			animator.SetBool("isLanding", true);
		}
	}

	private void Falling()
	{
		animator.SetBool("isJumping", false);
		animator.SetBool("isRunning", false);
		animator.SetBool("isWalkingBackwards", false);
		animator.SetBool("isFalling", false);
		if(TP_Controller.Instance.onGround)
		{
			animator.SetBool("isLanding", true);
		}
	}

	private void Landing()
	{
		animator.SetBool("isLanding", false);
	}

	#endregion

	#region Start Action Method

	public bool Jump()
	{
		bool haSaltado = false;

		if(TP_Controller.Instance.onGround && (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("WalkBackwards") || animator.GetCurrentAnimatorStateInfo(0).IsName("Running")))
		{
			animator.SetBool("isRunning", false);
			animator.SetBool("isLanding", false);
			animator.SetBool("isFalling", false);
			animator.SetBool("isWalkingBackwards", false);

			animator.SetBool("isJumping", true);

			haSaltado = true;
		}

		return haSaltado;

//		if(!TP_Controller.Instance.onGround || State == CharacterState.Jumping)
//			return;
//
//		animator.SetBool("isRunning", false);
//		animator.SetBool("isLanding", false);
//		animator.SetBool("isFalling", false);
//		animator.SetBool("isWalkingBackwards", false);
//
//		animator.SetBool("isJumping", true);
//		SetState(CharacterState.Jumping);
	}

	public void Fall()
	{
//		if(animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Jumping") || animator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
//		{
//			animator.SetBool("isFalling", true);
//			animator.SetBool("isJumping", false);
//			SetState(CharacterState.Falling);
//			//if we are too high do something
//		}
	}

	#endregion
}
