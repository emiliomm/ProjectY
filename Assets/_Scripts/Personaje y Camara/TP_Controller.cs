using UnityEngine;
using System.Collections;

public class TP_Controller : MonoBehaviour
{
	//Radio cápsula de al menos 0.4
	public static CharacterController CharacterController; //Nos permite lidiar con colisiones sin RigidBody
	public static TP_Controller Instance; //Instancia propia de la clase

	public enum State { Normal, Dialogo, Interactuables }

	State _state = State.Normal;
	State _prevState;

	public State CurrentState {
		get { return _state; } 
	}

	public State PrevState {
		get { return _prevState; }
	}

	public void SetState(State newState) {
		_prevState = _state;
		_state = newState;
	}

	//indica si está tocando el suelo
	public bool onGround;

	//Indica con que colisiona el objeto
	public LayerMask layerMask;

	// Use this when the object is created
	void Awake ()
	{
		//Inicializamos el componente CharacterController
		//Inicializamos la variable instancia
		CharacterController = GetComponent("CharacterController") as CharacterController; 
		Instance = this;
		SetState(State.Normal);

		//creamos o buscamos una camara
		TP_Camera.UseExistingOrCreateMainCamera();
	}
	
	// Update is called once per frame
	void Update ()
	{
		onGround = isOnGround();

		//Dependiendo del estado, hacemos unas cosas u otras
		switch(_state)
		{
		case State.Normal:
			//Si no hay camara, no nos movemos
			if (Camera.main == null)
				return;

			GetLocomotionInput();//Asignamos el movimiento del input

			HandleActionInput();

			TP_Motor.Instance.UpdateMotor();//lo pasamos a coord del mundo, normalizando, etc...
			InteractuableCollider.Instance.GetNearestTaggedObject();
			break;
		case State.Dialogo:
		case State.Interactuables:
			//guardamos el valor del movevector.y ya que vamos a transformarlo a 0 despues, pero necesitamos el valor
			TP_Motor.Instance.VerticalVelocity = TP_Motor.Instance.MoveVector.y;

			//Lo igualamos a 0 para recalcularlo cada frame
			TP_Motor.Instance.MoveVector = new Vector3(0f, -1f, 0f); //Unity necesita tener gravedad siempre activa para el CharacterController
			TP_Animator.Instance.MoveDirection = TP_Animator.Direction.Stationary;

			TP_Motor.Instance.UpdateMotor();//lo pasamos a coord del mundo, normalizando, etc...
			break;
		}

	}

	//Asignamos el movimiento del input
	/*
	 * NOTA: Input.getAxis es una tecla predeterminada llamada Axis, no es el eje
	 * Para ver los Inputs, ir a Edit > Project Settings > Input
	 */

	void GetLocomotionInput()
	{
		var deadZone = 0.1f;//zona muerta del controlador

		//guardamos el valor del movevector.y ya que vamos a transformarlo a 0 despues, pero necesitamos el valor
		TP_Motor.Instance.VerticalVelocity = TP_Motor.Instance.MoveVector.y;

		//Lo igualamos a 0 para recalcularlo cada frame
		TP_Motor.Instance.MoveVector = Vector3.zero;

		//Añadimos el movimiento del axis vertical si su movimiento es mayor que la zona muerta
		if (Input.GetAxis("Vertical") > deadZone || Input.GetAxis("Vertical") < -deadZone)
			TP_Motor.Instance.MoveVector += new Vector3( 0, 0, Input.GetAxis("Vertical"));

		//Añadimos el movimiento del axis horizontal si su movimiento es mayor que la zona muerta
		if (Input.GetAxis("Horizontal") > deadZone || Input.GetAxis("Horizontal") < -deadZone)
			TP_Motor.Instance.MoveVector += new Vector3(Input.GetAxis("Horizontal"), 0, 0);

		//Determinamos la direccion ahora que tenemos el vector de movimiento
		TP_Animator.Instance.DetermineCurrentMoveDirection();
	}

	//Comprobamos si el jugador pulsa alguna tecla para realizar alguna acción, como saltar
	void HandleActionInput()
	{
		//Si el jugador pulsa el boton de salto
		if (Input.GetButton("Jump"))
		{
			Jump();
		}
	}

	//Aplicamos el salto
	void Jump()
	{
		TP_Motor.Instance.Jump(); //Ejecutamos las operaciones de salto
		TP_Animator.Instance.Jump(); //Ejecutamos la animación de salto
	}

	//Comprobamos si estamos en el suelo
	private bool isOnGround()
	{
		bool retVal = false;

		Vector3 origin = transform.position + new Vector3(0, 0.05f, 0);
		RaycastHit hit;

		if(Physics.Raycast(origin, - Vector3.up, out hit, 0.5f, layerMask))
		{
			retVal = true;
		}

		return retVal;
	}
}
