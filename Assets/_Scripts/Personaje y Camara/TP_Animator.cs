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

	public static TP_Animator Instance;

	//propiedad que guarda nuestra direccion
	public Direction MoveDirection {get; set; }

	void Awake()
	{
		Instance = this;
	}

	// Update is called once per frame
	void Update ()
	{
	
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

}
