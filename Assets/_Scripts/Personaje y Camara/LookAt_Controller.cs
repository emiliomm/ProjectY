using UnityEngine;
using System.Collections;

public class LookAt_Controller : MonoBehaviour {

	public static LookAt_Controller Instance; //Instancia propia de la clase

	public Vector3 lookAtPosition;
	public Transform transformRightCamera;
	public RaycastHit hit_blue;
	public RaycastHit hit_red;

	public float offsetSmooth = 0.4f;
	public float offsetSmoothEmergency = 0.1f;

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

		offset = max_offset;
	}

	void Update()
	{
		offset = Mathf.Clamp(offset, min_offset, max_offset);

		lookAtPosition = TP_Camera.Instance.TargetLookAt.transform.position;
		transformRightCamera = TP_Camera.Instance.transform;

		transform.position = lookAtPosition + transformRightCamera.right * offset;

		Debug.DrawLine(lookAtPosition, lookAtPosition + transformRightCamera.right * max_offset, Color.blue);
		Debug.DrawLine(lookAtPosition, lookAtPosition - transformRightCamera.right * max_offset, Color.red);

//		if(Physics.Linecast(lookAtPosition, lookAtPosition + transformRightCamera.right * max_offset, out hit_blue))
//		{
//			backToNormal = false;
//		}
//		else if(Physics.Linecast(lookAtPosition, lookAtPosition - transformRightCamera.right * max_offset, out hit_red))
//		{
//			backToNormal = false;
//		}
//		else
//		{
//			if (TP_Camera.Instance.Distance > 0.5f)
//				backToNormal = true;
//		}
//

//		Debug.Log(hit_red.distance);
//		Debug.Log("--------------------------");
//
//		if (backToNormal)
//		{
//			offset = Mathf.SmoothDamp(offset, max_offset, ref offset_value, offsetSmooth);
//		}
//		else
//		{
//			
//			if (offset < 0.001)
//			{
//				offset = min_offset;
//			}
//			else
//				offset = Mathf.SmoothDamp(offset, min_offset, ref offset_value, offsetSmooth);
//		}


//		if(Physics.Linecast(lookAtPosition, lookAtPosition + transformRightCamera.right * max_offset, out hit_blue))
//		{
//			if (offset > hit_blue.distance - 0.4f)
//			{
//				offset = hit_blue.distance - 0.4f;
//			}
//			if (offset <= 0.03f)
//				offset = min_offset;
//		}
//		else{
//			if(TP_Camera.Instance.Distance > 0.25f)
//				offset = Mathf.SmoothDamp(offset, max_offset, ref offset_value, offsetSmooth);
//		}
//
//		if(TP_Camera.Instance.Distance < 0.50f)
//		{
//			do{
//				offset = Mathf.SmoothDamp(offset, min_offset, ref offset_value, offsetSmooth);
//				if (offset < 0)
//					offset = 0;
//			}while(TP_Camera.Instance.Distance >= 0.50f);
//		}

	}
}
