using UnityEngine;

/*
 * 	Clase que se encarga de controlar el movimiento del jugador
 *  Autor clase original: Tutorial Cámara 3DBuzz (https://www.3dbuzz.com/training/view/3rd-person-character-system)
 * 	Modificada por mí
 */
public class TP_Controller : MonoBehaviour
{
	//Radio cápsula de al menos 0.4
	public static CharacterController characterController; //Nos permite lidiar con colisiones sin RigidBody
	public static TP_Controller Instance; //Instancia propia de la clase

	public enum State { Normal, Dialogo, Interactuables }

	private State _state;
	private State _prevState;

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
	public bool onGround { get { return isOnGround(); } }

	//Indica si el jugador está utilizando un transporte entre escenas
	private bool transportando;

	// Use this when the object is created
	private void Awake ()
	{
		transportando = false;

		//Inicializamos el componente CharacterController
		//Inicializamos la variable instancia
		characterController = GetComponent<CharacterController>(); 
		Instance = this;
		SetState(State.Normal);

		//creamos o buscamos una camara
		TP_Camera.UseExistingOrCreateMainCamera();
	}

	private void Update ()
	{
		//Dependiendo del estado, hacemos unas cosas u otras
		switch(_state)
		{
		case State.Normal:
			//Si no hay camara, no nos movemos
			if (Camera.main == null)
				return;

			//Si el menú no se ha activado, continuamos
			if(!MenuInput())
			{
				GetLocomotionInput();//Asignamos el movimiento del input

				HandleActionInput();

				TP_Motor.Instance.UpdateMotor();//lo pasamos a coord del mundo, normalizando, etc...
				InteractuableCollider.Instance.EncontrarInteractuablesCercanos();
			}
			break;
		case State.Dialogo: //También se usa en la pantalla de Inventario, de momento
		case State.Interactuables:
			//guardamos el valor del movevector.y ya que vamos a transformarlo a 0 despues, pero necesitamos el valor
			TP_Motor.Instance.VerticalVelocity = TP_Motor.Instance.MoveVector.y;

			//Lo igualamos a 0 para recalcularlo cada frame
			TP_Motor.Instance.MoveVector = new Vector3(0f, -1f, 0f); //Unity necesita tener gravedad siempre activa para el CharacterController
			TP_Animator.Instance.SetMoveDirection(TP_Animator.Direction.Stationary);

			TP_Motor.Instance.UpdateMotor();//lo pasamos a coord del mundo, normalizando, etc...
			break;
		}
	}

	//Controla el input de los menus
	//Devuelve true si se ha activado un menu
	private bool MenuInput()
	{
		bool activado = false;

		if (Input.GetKeyDown((KeyCode.I)))
		{
			activado = true;

			GameObject InventarioManager = (GameObject)Instantiate(Resources.Load("PanelInventarioPrefab"));
			InventarioManager.transform.SetParent(Manager.Instance.canvasGlobal.transform, false);

			SetState(State.Dialogo);
			Manager.Instance.setPausa(true);
			Manager.Instance.stopNavMeshAgents();
			Cursor.visible = true; //Muestra el cursor del ratón
			Camera.main.GetComponent<TP_Camera>().setObjectMode();
		}

		return activado;
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
	private void HandleActionInput()
	{
		//Si el jugador pulsa el boton de salto
		if (Input.GetButton("Jump"))
		{
			Jump();
		}
	}

	//Aplicamos el salto
	private void Jump()
	{
		if(TP_Animator.Instance.Jump()) //Ejecutamos la animación de salto
			TP_Motor.Instance.Jump(); //Ejecutamos las operaciones de salto
	}

	//Comprobamos si estamos en el suelo
	private bool isOnGround()
	{
		return characterController.isGrounded;
	}

	public void setTransportando(bool estado)
	{
		transportando = estado;
	}

	public bool getTransportando()
	{
		return transportando;
	}
}
