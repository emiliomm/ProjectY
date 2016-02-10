﻿using UnityEngine;
using System.Collections;

public class TP_Controller : MonoBehaviour
{
	public static CharacterController CharacterController; //Nos permite lidiar con colisiones sin RigidBody
	public static TP_Controller Instance; //Instancia propia de la clase

	public bool canMove; //Indica si el jugador se puede mover

	// Use this when the object is created
	void Awake ()
	{
		//Inicializamos el componente CharacterController
		//Inicializamos la variable instancia
		CharacterController = GetComponent("CharacterController") as CharacterController; 
		Instance = this;

		//creamos o buscamos una camara
		TP_Camera.UseExistingOrCreateMainCamera();
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Si no hay camara, no nos movemos
		//Si el bool canMove es false, no nos movemos
		if (Camera.main == null || !canMove)
			return;

		GetLocomotionInput();//Asignamos el movimiento del input

		HandleActionInput();

		TP_Motor.Instance.UpdateMotor();//lo pasamos a coord del mundo, normalizando, etc...
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

	//Comprobamos si el jugador pulsa alguna tecla
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
	}
}