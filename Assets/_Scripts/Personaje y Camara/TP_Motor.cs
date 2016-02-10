﻿using UnityEngine;
using System.Collections;

public class TP_Motor : MonoBehaviour
{
	public static TP_Motor Instance;

	public float ForwardSpeed = 10f; //Velocidad de movimiento hacia delante
	public float BackwardSpeed = 2f; //Velocidad de movimiento hacia atras
	public float StrafingSpeed = 5f; //Velocidad de movimiento lateral
	public float SlideSpeed = 10f; //Velocidad de movimiento resbaladiza

	public float JumpSpeed = 10f;
	public float Gravity = 21f;
	public float TerminalVelocity = 20f;
	public float SlideThreshold = 0.6f;//Limite para resbalar
	public float MaxControllableSlideMagnitude = 0.4f;//El personaje puede controlarse al resbalar si no rebasa este parametro

	private Vector3 slideDirection;

	public Vector3 MoveVector {get; set; } //Vector de movimiento
	public float VerticalVelocity {get; set; } //Velocidad vertical
	
	// Use this when the object is created
	void Awake()
	{
		//Inicializamos la variable instancia
		Instance = this;
	}

	public void UpdateMotor()
	{
		SnapAlignCharacterWithCamera();
		ProcessMotion();
	}

	//Procesa el movimiento
	void ProcessMotion()
	{
		//Transformamos MoveVector al espacio del mundo
		MoveVector = transform.TransformDirection(MoveVector);

		//Normalizamos MoveVector si la magnitud es > 1
		if (MoveVector.magnitude > 1)
			MoveVector = Vector3.Normalize(MoveVector);

		//Comprobamos si resbalamos
		ApplySlide();

		//Multiplamos MoveVector por MoveSpeed
		MoveVector *= MoveSpeed();

		//Reaplicamos la VelocidadTerminal a MoveVector.y
		MoveVector = new Vector3(MoveVector.x, VerticalVelocity, MoveVector.z);

		//Aplicamos la gravedad
		ApplyGravity();

		//Movemos el personaje en el espacio del mundo
		//Convertimos el MoveVector(units*frame) a (units*second). Para ello, multiplicamos el MoveVector por DeltaTime
		TP_Controller.CharacterController.Move(MoveVector * Time.deltaTime);
	}

	//Aplicamos la gravedad
	void ApplyGravity()
	{
		//Si no superamos la velocidad terminal, aplicamos la gravedad
		if (MoveVector.y > -TerminalVelocity)
			//Restamos a y la gravedad en m/s
			MoveVector = new Vector3(MoveVector.x, MoveVector.y - Gravity * Time.deltaTime, MoveVector.z);

		//Si el personaje esta tocando tierra, reseteamos la y a -1 para que no supere valores negativos mas bajos
		if (TP_Controller.CharacterController.isGrounded && MoveVector.y < -1)
			MoveVector = new Vector3(MoveVector.x, -1, MoveVector.z);
	}

	//Miramos si el personaje resbale si la superficie es muy pronunciada
	void ApplySlide()
	{
		//Si estamos en el aire, no hacemos nada
		if(!TP_Controller.CharacterController.isGrounded)
			return;

		slideDirection = Vector3.zero;

		RaycastHit hitInfo; //Aqui guardaremos la informacion del rayo lanzado hacia el suelo

		//Si le damos a algo situado debajo del personaje (tierra)
		//El rayo esta situado 1 unidad encima de los pies del personaje apuntando hacia abajo
		if(Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hitInfo))
		{
			if (hitInfo.normal.y < SlideThreshold) //si la normal a la que le hemos dado es menor que nuestro limite, resbalamos
				slideDirection = new Vector3(hitInfo.normal.x, -hitInfo.normal.y, hitInfo.normal.z); //Aplicamos la direccion del terreno que hemos tocado, invirtiendo la y, ya que nos movemos hacia abajo al resbalar
		}

		//Comprobamos la magnitud de SlideDirection para ver si nos podemos mover al resbalar
		//Si es menor que nuestro valor maximo, podemos controlar al personaje al caer
		if (slideDirection.magnitude < MaxControllableSlideMagnitude)
			MoveVector += slideDirection;
		//Si es mayor o igual, resbalamos, sin poder controlar al personaje
		else
		{
			MoveVector = slideDirection;
		}
	}

	//Al saltar
	public void Jump()
	{
		//Comprobamos si estamos tocamos tierra y aplicamos la velocidad de salto
		if (TP_Controller.CharacterController.isGrounded)
			VerticalVelocity = JumpSpeed;
	}

	//Mira si el personaje se mueve, si nos movemos alinea el personaje con la camara
	void SnapAlignCharacterWithCamera()
	{
		//Si no nos movemos
		if (MoveVector.x != 0 || MoveVector.z != 0)
		{
			//Deja las coord x y z como estan y asigna la coord y del personaje a la de la camara
			transform.rotation = Quaternion.Euler(transform.eulerAngles.x,Camera.main.transform.eulerAngles.y,transform.eulerAngles.z);
		}
	}

	//Calculamos la velocidad de movimiento
	float MoveSpeed()
	{
		var moveSpeed = 0f;

		switch(TP_Animator.Instance.MoveDirection)
		{
		//quieto
		case TP_Animator.Direction.Stationary:
			moveSpeed = 0;
			break;
		case TP_Animator.Direction.Forward:
			moveSpeed = ForwardSpeed;
			break;
		case TP_Animator.Direction.Backward:
			moveSpeed = BackwardSpeed;
			break;
		case TP_Animator.Direction.Left:
			moveSpeed = StrafingSpeed;
			break;
		case TP_Animator.Direction.Right:
			moveSpeed = StrafingSpeed;
			break;
		case TP_Animator.Direction.LeftForward:
			moveSpeed = ForwardSpeed;
			break;
		case TP_Animator.Direction.RightForward:
			moveSpeed = ForwardSpeed;
			break;
		case TP_Animator.Direction.LeftBackward:
			moveSpeed = BackwardSpeed;
			break;
		case TP_Animator.Direction.RightBackward:
			moveSpeed = BackwardSpeed;
			break;
		}

		//Comprobamos si estamos resbalando
		if (slideDirection.magnitude > 0)
			moveSpeed = SlideSpeed;

		return moveSpeed;
	}
}
