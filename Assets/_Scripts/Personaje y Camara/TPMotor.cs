using UnityEngine;

/*
 * 	Clase que se encarga de detectar y ajustar el movimiento del jugador
 *  Autor clase original: Tutorial Cámara 3DBuzz (https://www.3dbuzz.com/training/view/3rd-person-character-system)
 * 	Modificada por mí
 */
public class TPMotor : MonoBehaviour
{
	//patrón singleton
	public static TPMotor instance;

	public float forwardSpeed = 3.2f; //Velocidad de movimiento hacia delante
	public float backwardSpeed = 2f; //Velocidad de movimiento hacia atras
	public float strafingSpeed = 2f; //Velocidad de movimiento lateral
	public float slideSpeed = 3f; //Velocidad de movimiento resbaladiza

	public float jumpSpeed = 10f;
	public float gravity = 21f;
	public float terminalVelocity = 10f;
	public float slideThreshold = 0.6f;//Limite para resbalar
	public float maxControllableSlideMagnitude = 0.4f;//El personaje puede controlarse al resbalar si no rebasa este parametro

	private Vector3 slideDirection;

	public Vector3 moveVector {get; set; } //Vector de movimiento
	public float verticalVelocity {get; set; } //Velocidad vertical
	
	// Use this when the object is created
	void Awake()
	{
		//Inicializamos la variable instancia
		instance = this;
	}

	public void UpdateMotor()
	{
		SnapAlignCharacterWithCamera();
		ProcessMotion();
	}

	//Procesa el movimiento
	private void ProcessMotion()
	{
		//Transformamos MoveVector al espacio del mundo
		moveVector = transform.TransformDirection(moveVector);

		//Normalizamos MoveVector si la magnitud es > 1
		if (moveVector.magnitude > 1)
			moveVector = Vector3.Normalize(moveVector);

		//Comprobamos si resbalamos
		ApplySlide();

		//Multiplamos MoveVector por MoveSpeed
		moveVector *= MoveSpeed();

		//Reaplicamos la VelocidadTerminal a MoveVector.y
		moveVector = new Vector3(moveVector.x, verticalVelocity, moveVector.z);

		//Aplicamos la gravedad
		ApplyGravity();

		//Movemos el personaje en el espacio del mundo
		//Convertimos el MoveVector(units*frame) a (units*second). Para ello, multiplicamos el MoveVector por DeltaTime
		TPController.characterController.Move(moveVector * Time.deltaTime);
	}

	//Aplicamos la gravedad
	public void ApplyGravity()
	{
		//Si no superamos la velocidad terminal, aplicamos la gravedad
		if (moveVector.y > -terminalVelocity)
			//Restamos a y la gravedad en m/s
			moveVector = new Vector3(moveVector.x, moveVector.y - gravity * Time.deltaTime, moveVector.z);

		//Si el personaje esta tocando tierra, reseteamos la y a -1 para que no supere valores negativos mas bajos
		//Unity necesita que siempre se añada gravedad al charactercontroller
		if (TPController.instance.onGround && moveVector.y < -1)
			moveVector = new Vector3(moveVector.x, -1, moveVector.z);
	}

	//Miramos si el personaje resbale si la superficie es muy pronunciada
	private void ApplySlide()
	{
		//Si estamos en el aire, no hacemos nada
		if(!TPController.instance.onGround)
			return;

		slideDirection = Vector3.zero;

		RaycastHit hitInfo; //Aqui guardaremos la informacion del rayo lanzado hacia el suelo

		//Si le damos a algo situado debajo del personaje (tierra)
		//El rayo esta apuntando hacia abajo
		if(Physics.Raycast(transform.position, Vector3.down, out hitInfo))
		{
			if (hitInfo.normal.y < slideThreshold) //si la normal a la que le hemos dado es menor que nuestro limite, resbalamos
				slideDirection = new Vector3(hitInfo.normal.x, -hitInfo.normal.y, hitInfo.normal.z); //Aplicamos la direccion del terreno que hemos tocado, invirtiendo la y, ya que nos movemos hacia abajo al resbalar
		}

		//Comprobamos la magnitud de SlideDirection para ver si nos podemos mover al resbalar
		//Si es menor que nuestro valor maximo, podemos controlar al personaje al caer
		if (slideDirection.magnitude < maxControllableSlideMagnitude)
			moveVector += slideDirection;
		//Si es mayor o igual, resbalamos, sin poder controlar al personaje
		else
		{
			moveVector = slideDirection;
		}
	}

	//Al saltar
	public void Jump()
	{
		//Comprobamos si estamos tocamos tierra y aplicamos la velocidad de salto
		if (TPController.instance.onGround)
			verticalVelocity = jumpSpeed;
	}

	//CORREGIR LA DIRECCIÓN RESPECTO AL OFFSET DE LA CÁMARA
	//Mira si el personaje se mueve, si nos movemos alinea el personaje con la camara
	private void SnapAlignCharacterWithCamera()
	{
		if(TPCamera.instance.distance == TPCamera.instance.preOccludedDistance)
		{
			if(moveVector.x != 0 || moveVector.z != 0)
			{
				//Deja las coord x y z como estan y asigna la coord y del personaje a la de la camara
				transform.rotation = Quaternion.Euler(transform.eulerAngles.x,Camera.main.transform.eulerAngles.y,transform.eulerAngles.z);
			}
		}
		else
		{
			if((Input.GetAxis ("Mouse X") != 0 || Input.GetAxis ("Mouse Y") != 0) && (moveVector.x != 0 || moveVector.z != 0))
			{
				//Deja las coord x y z como estan y asigna la coord y del personaje a la de la camara
				transform.rotation = Quaternion.Euler(transform.eulerAngles.x,Camera.main.transform.eulerAngles.y,transform.eulerAngles.z);
			}
			else if((moveVector.x != 0 || moveVector.z != 0) && TPAnimator.instance.prevMoveDirection == TPAnimator.Direction.Stationary)
			{
				//Deja las coord x y z como estan y asigna la coord y del personaje a la de la camara
				transform.rotation = Quaternion.Euler(transform.eulerAngles.x,Camera.main.transform.eulerAngles.y,transform.eulerAngles.z);
			}
		}

	}

	//Calculamos la velocidad de movimiento
	private float MoveSpeed()
	{
		var moveSpeed = 0f;

		switch(TPAnimator.instance.moveDirection)
		{
		//quieto
		case TPAnimator.Direction.Stationary:
			moveSpeed = 0;
			break;
		case TPAnimator.Direction.Forward:
			moveSpeed = forwardSpeed;
			break;
		case TPAnimator.Direction.Backward:
			moveSpeed = backwardSpeed;
			break;
		case TPAnimator.Direction.Left:
			moveSpeed = strafingSpeed;
			break;
		case TPAnimator.Direction.Right:
			moveSpeed = strafingSpeed;
			break;
		case TPAnimator.Direction.LeftForward:
			moveSpeed = forwardSpeed;
			break;
		case TPAnimator.Direction.RightForward:
			moveSpeed = forwardSpeed;
			break;
		case TPAnimator.Direction.LeftBackward:
			moveSpeed = backwardSpeed;
			break;
		case TPAnimator.Direction.RightBackward:
			moveSpeed = backwardSpeed;
			break;
		}

		//Comprobamos si estamos resbalando
		if (slideDirection.magnitude > 0)
			moveSpeed = slideSpeed;

		return moveSpeed;
	}
}