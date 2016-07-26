using UnityEngine;
using System.Collections;

public class ObjetoDato : MonoBehaviour {

	RaycastHit hitInfo = new RaycastHit();
	public LayerMask layerMask;
	Material m;
	Vector3 forward;

	// Use this for initialization
	void Start () {
		SpriteRenderer s = GetComponent<SpriteRenderer>();
		m = s.material;
	}
	
	// Update is called once per frame
	void Update () {
		Ray rayToCameraPos = new Ray(transform.position, Camera.main.transform.position-transform.position);
		Debug.DrawRay(rayToCameraPos.origin, rayToCameraPos.direction*100, Color.blue);

		if(Physics.Raycast(rayToCameraPos, out hitInfo, 1000))
		{
			transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
			SpriteRenderer s = GetComponent<SpriteRenderer>();
			s.material = m;
		}
		else
		{
			transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
			SpriteRenderer s = GetComponent<SpriteRenderer>();
			s.material = Resources.Load("UI") as Material;
		}
	}
}
