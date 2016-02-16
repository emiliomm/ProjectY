using UnityEngine;
using System.Collections;

public class LookAtOffset : MonoBehaviour {

	public static LookAtOffset Instance;
	public static CharacterController OffsetControler; //Nos permite lidiar con colisiones sin RigidBody

	private Vector3 distancia;

	// Use this when the object is created
	void Awake ()
	{
		OffsetControler = GetComponent("CharacterController") as CharacterController; 
		
		//Inicializamos la variable instancia
		Instance = this;
	}

	public bool checkCollision(Vector3 check)
	{
		distancia = check;
	}

	void onCollisionEnter(Collision other)
	{
		
	}

	void Update()
	{
		transform.position = distancia;
	}
}
