using UnityEngine;

/*
 * 	Clase que se encarga de controlar el movimiento del jugador
 *  Autor clase original: Tutorial Cámara 3DBuzz (https://www.3dbuzz.com/training/view/3rd-person-character-system)
 * 	Modificada por mí
 */
public class TPController : MonoBehaviour
{
	//Radio cápsula de al menos 0.4
	public static CharacterController characterController; //Nos permite lidiar con colisiones sin RigidBody
	public static TPController instance; //Instancia propia de la clase

	public enum State { Normal, Dialogo, Interactuables }

	private State state;
	private State prevState;

	public State CurrentState {
		get { return state; } 
	}

	public State PrevState {
		get { return prevState; }
	}

	public void SetState(State newState) {
		prevState = state;
		state = newState;
	}

	//indica si está tocando el suelo
	public bool onGround { get { return IsOnGround(); } }

	//Indica si el jugador está utilizando un transporte entre escenas
	private bool transportando;

	private RaycastHit hit;
	private static Vector3 direction = new Vector3(0, -1, 0);
	private static float distance = 1f;

	// Use this when the object is created
	private void Awake ()
	{
		transportando = false;

		//Inicializamos el componente CharacterController
		//Inicializamos la variable instancia
		characterController = GetComponent<CharacterController>(); 
		instance = this;
		SetState(State.Normal);

		//creamos o buscamos una camara
		TPCamera.UseExistingOrCreateMainCamera();
	}

	private void Update ()
	{
		//Dependiendo del estado, hacemos unas cosas u otras
		switch(state)
		{
		case State.Normal:
			//Si no hay camara, no nos movemos
			if (Camera.main == null)
				return;
			
			GetLocomotionInput();//Asignamos el movimiento del input

			HandleActionInput();

			TPMotor.instance.UpdateMotor();//lo pasamos a coord del mundo, normalizando, etc...
			InteractuableCollider.Instance.EncontrarInteractuablesCercanos();
			break;
		case State.Dialogo: //También se usa en la pantalla de Inventario, de momento
		case State.Interactuables:
			//guardamos el valor del movevector.y ya que vamos a transformarlo a 0 despues, pero necesitamos el valor
			TPMotor.instance.verticalVelocity = TPMotor.instance.moveVector.y;

			//Lo igualamos a 0 para recalcularlo cada frame
			TPMotor.instance.moveVector = new Vector3(0f, -1f, 0f); //Unity necesita tener gravedad siempre activa para el CharacterController
			TPAnimator.instance.SetMoveDirection(TPAnimator.Direction.Stationary);

			TPMotor.instance.UpdateMotor();//lo pasamos a coord del mundo, normalizando, etc...

			break;
		}
	}

	//Asignamos el movimiento del input
	/*
	 * NOTA: Input.getAxis es una tecla predeterminada llamada Axis, no es el eje
	 * Para ver los Inputs, ir a Edit > Project Settings > Input
	 */
	private void GetLocomotionInput()
	{
		var deadZone = 0.1f;//zona muerta del controlador

		//guardamos el valor del movevector.y ya que vamos a transformarlo a 0 despues, pero necesitamos el valor
		TPMotor.instance.verticalVelocity = TPMotor.instance.moveVector.y;

		//Lo igualamos a 0 para recalcularlo cada frame
		TPMotor.instance.moveVector = Vector3.zero;

		//Añadimos el movimiento del axis vertical si su movimiento es mayor que la zona muerta
		if (Input.GetAxis("Vertical") > deadZone || Input.GetAxis("Vertical") < -deadZone)
			TPMotor.instance.moveVector += new Vector3( 0, 0, Input.GetAxis("Vertical"));

		//Añadimos el movimiento del axis horizontal si su movimiento es mayor que la zona muerta
		if (Input.GetAxis("Horizontal") > deadZone || Input.GetAxis("Horizontal") < -deadZone)
			TPMotor.instance.moveVector += new Vector3(Input.GetAxis("Horizontal"), 0, 0);

		//Determinamos la direccion ahora que tenemos el vector de movimiento
		TPAnimator.instance.DetermineCurrentMoveDirection();
	}

	//Comprobamos si el jugador pulsa alguna tecla para realizar alguna acción, como saltar
	private void HandleActionInput()
	{
		//Si el jugador pulsa el boton de salto
		if (Input.GetButton("Jump"))
		{
			Jump();
		}

		if(Input.GetKey(KeyCode.LeftShift))
		{
			TPMotor.instance.running = true;
		}
		else
		{
			TPMotor.instance.running = false;
		}
	}

	//Aplicamos el salto
	private void Jump()
	{
		if(TPAnimator.instance.Jump()) //Ejecutamos la animación de salto
			TPMotor.instance.Jump(); //Ejecutamos las operaciones de salto
	}



	//Comprobamos si estamos en el suelo
	private bool IsOnGround()
	{		
		Debug.DrawRay(transform.position + new Vector3(0f, 0.05f, 0f),direction*distance,Color.green);

		return Physics.Raycast(transform.position + new Vector3(0f, 0.05f, 0f), direction, out hit, distance) || characterController.isGrounded;
	}

	public void SetTransportando(bool estado)
	{
		transportando = estado;
	}

	public bool GetTransportando()
	{
		return transportando;
	}
}
