using UnityEngine;
using System.Collections;

public class LookAt_Controller : MonoBehaviour {

	public static LookAt_Controller Instance; //Instancia propia de la clase

	public Vector3 lookAtPosition;
	public Transform transformRightCamera;
	public Transform transformLookAt;
	public static CapsuleCollider collider; //Nos permite lidiar con colisiones sin RigidBody
	public Rigidbody rigid;
	public RaycastHit hit;
	public float collisionCheckDistance;

	public float offsetSmooth = 0.4f;

	public float offset = 0f;
	public float offset_value = 0f;
	public float max_offset = 1f;
	public float min_offset = 0f;

	public bool backToNormal;

	// Use this when the object is created
	void Awake ()
	{
		//Inicializamos la variable instancia
		Instance = this;
		backToNormal = true;
		collider = GetComponent("CapsuleCollider") as CapsuleCollider;
		rigid = GetComponent("Rigidbody") as Rigidbody; 

		offset = max_offset;
	}

	public void UpdatePosition(Vector3 lookatposition, Transform transformcamera, Transform b)
	{
		lookAtPosition = lookatposition;
		transformRightCamera = transformcamera;
		transformLookAt = b;
		transform.position = lookAtPosition + transformRightCamera.right * offset;
		transform.rotation = transformRightCamera.rotation;
	}

	void LateUpdate()
	{
		transform.position = lookAtPosition + transformRightCamera.right * offset;
		transform.rotation = transformRightCamera.rotation;

//		if (rigid.SweepTest (transformRightCamera.right, out hit, max_offset)) {
//			
//			if (hit.collider.tag != "player")
//			{
//				Debug.Log("HOla");
//				backToNormal = false;
//				var distanceToCollision = hit.distance;
//
//				if (distanceToCollision > 2)
//				{
//					backToNormal = true;
//				}
//			}
//		}

		Debug.DrawLine(transformLookAt.position, transform.position, Color.blue);
		if(Physics.Linecast(transformLookAt.position, transform.position, out hit))
		{
			backToNormal = false;
		}
		else
		{
			Debug.Log ("Hola");
			backToNormal = true;
		}

//		if (backToNormal)
//		{
//			offset = Mathf.SmoothDamp(offset, max_offset, ref offset_value, offsetSmooth);
//		}
//		else
//		{
//			offset = Mathf.SmoothDamp(offset, min_offset, ref offset_value, offsetSmooth);
//		}
	}

//	void OnTriggerEnter(Collider other)
//	{
//		if (other.tag != "Player")
//		{
//			print("Collision detected with trigger object " + other.name);
//			backToNormal = false;
//		}
//	}
//
//	void OnTriggerExit(Collider other)
//	{
//		if (other.tag != "Player")
//		{
//			print(gameObject.name + " and trigger object " + other.name + " are no longer colliding");
//		}
//	}
//
//	void OnTriggerStay(Collider other)
//	{
//		if (other.tag != "Player")
//		{
//			print("Still colliding with trigger object " + other.name);
//			backToNormal = false;
//		}
//	}
}
