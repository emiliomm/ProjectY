using UnityEngine;
using System.Collections;


public class CS_Camera : MonoBehaviour {

	public Transform target;

	[System.Serializable]
	public class PositionSettings
	{
		public Vector3 targetPosOffset = new Vector3(0, 3.4f, 0);
		public float distanceFromTarget = -8;
		public float zoomSmooth = 70;
		public float zoomStep = 2;
		public float maxZoom = -2;
		public float minZoom = -15;
		public bool smoothFollow = true;
		public float smooth = 0.05f;

		[HideInInspector]
		public float adjustmentDistance = -8;
	}

	[System.Serializable]
	public class OrbitSettings
	{
		public float xRotation = -20;
		public float yRotation = -180;
		public float maxXRotation = 25;
		public float minXRotation = -85;
		public float vOrbitSmooth = 150;
		public float hOrbitSmooth = 150;
		public float yOrbitSmooth = 0.5f;
		public float xOrbitSmooth = 0.5f;

		//holding our current x and y rotation for our camera
		//bool for allowing orbit
		public bool allowOrbit = true;
	}

	[System.Serializable]
	public class InputSettings
	{
		public string MOUSE_ORBIT = "MouseOrbit";
		public string MOUSE_ORBIT_VERTICAL = "MouseOrbitVertical";
		public string ORBIT_HORIZONTAL_SNAP = "OrbitHorizontalSnap";
		public string ORBIT_HORIZONTAL = "OrbitHorizontal";
		public string ORBIT_VERTICAL = "OrbitVertical";
		public string ZOOM = "Mouse ScrollWheel";
	}

	[System.Serializable]
	public class DebugSettings
	{
		public bool drawDesiredCollisionLines = true;
		public bool drawAdjustedCollisionLines = true;
	}

	public PositionSettings position = new PositionSettings();
	public OrbitSettings orbit = new OrbitSettings();
	public InputSettings input = new InputSettings();
	public DebugSettings debug = new DebugSettings();
	public CollisionHandler collision = new CollisionHandler();

	private Vector3 targetPos = Vector3.zero;
	private Vector3 destination = Vector3.zero;
	private Vector3 adjustedDestination = Vector3.zero;
	private Vector3 camVel = Vector3.zero;
	private CharacterController charController;
	float vOrbitInput, hOrbitInput, zoomInput, hOrbitSnapInput, mouseOrbitInput, vMouseOrbitInput;
	private Vector3 previousMousePos = Vector3.zero;
	private Vector3 currentMousePos = Vector3.zero;

	// Use this for initialization
	void Start () {
		SetCameraTarget(target);

		vOrbitInput = hOrbitInput = zoomInput = hOrbitSnapInput = mouseOrbitInput = vMouseOrbitInput = 0;

		MoveToTarget();

		collision.Initialize(Camera.main);
		collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
		collision.UpdateCameraClipPoints(destination, transform.rotation, ref collision.desiredCameraClipPoints);

		previousMousePos = currentMousePos = Input.mousePosition;
	}

	void SetCameraTarget(Transform t)
	{
		target = t;

		if(target != null)
		{
			if(target.GetComponent<CharacterController>())
			{
				charController = target.GetComponent<CharacterController>();
			}
			else
				Debug.LogError("The camera's target needs a character controller");
		}
		else
			Debug.LogError("The camera needs a target");
	}

	void GetInput()
	{
		vOrbitInput = Input.GetAxisRaw(input.ORBIT_VERTICAL);
		hOrbitInput = Input.GetAxisRaw(input.ORBIT_HORIZONTAL);
		hOrbitSnapInput = Input.GetAxisRaw(input.ORBIT_HORIZONTAL_SNAP);
		zoomInput = Input.GetAxisRaw(input.ZOOM);
		mouseOrbitInput = Input.GetAxisRaw(input.MOUSE_ORBIT);
		vMouseOrbitInput = Input.GetAxisRaw(input.MOUSE_ORBIT_VERTICAL);
	}

	void Update()
	{
		GetInput();
		ZoomInOnTarget();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//moving
		MoveToTarget();
		//rotating
		LookAtTarget();
		//player input orbit
		OrbitTarget();
		MouseOrbitTarget();

		collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
		collision.UpdateCameraClipPoints(destination, transform.rotation, ref collision.desiredCameraClipPoints);

		//draw debug lines
		for(int i = 0; i < 5; i++)
		{
			if(debug.drawDesiredCollisionLines)
			{
				Debug.DrawLine(targetPos, collision.desiredCameraClipPoints[i], Color.white);
			}
			if(debug.drawAdjustedCollisionLines)
			{
				Debug.DrawLine(targetPos, collision.adjustedCameraClipPoints[i], Color.green);
			}
		}

		collision.CheckColliding(targetPos); //using raycast here
		position.adjustmentDistance = collision.GetAdjustedDistanceWithRayFrom(targetPos);
	}

	void MoveToTarget()
	{
		targetPos = target.position + Vector3.up * position.targetPosOffset.y + Vector3.forward * position.targetPosOffset.z + transform.TransformDirection(Vector3.right * position.targetPosOffset.x);
		destination = Quaternion.Euler(orbit.xRotation, orbit.yRotation + target.eulerAngles.y, 0) * -Vector3.forward * position.distanceFromTarget;
		destination += targetPos;

		if(collision.colliding)
		{
			adjustedDestination = Quaternion.Euler(orbit.xRotation, orbit.yRotation + target.eulerAngles.y, 0) * Vector3.forward * position.adjustmentDistance;
			adjustedDestination += targetPos;

			if(position.smoothFollow)
			{
				//use smooth damp function
				transform.position = Vector3.SmoothDamp(transform.position, adjustedDestination, ref camVel, position.smooth);
			}
			else
				transform.position = adjustedDestination;
		}
		else
		{
			if(position.smoothFollow)
			{
				//use smooth damp function
				transform.position = Vector3.SmoothDamp(transform.position, destination, ref camVel, position.smooth);
			}
			else
				transform.position = destination;
		}
	}

	void LookAtTarget()
	{
		Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100 * Time.deltaTime);
	}

	void OrbitTarget()
	{
		if (hOrbitSnapInput > 0)
		{
			orbit.yRotation = -180;
		}

		orbit.xRotation += -vOrbitInput * orbit.vOrbitSmooth;
		orbit.yRotation += -hOrbitInput * orbit.hOrbitSmooth;

		if(orbit.xRotation > orbit.maxXRotation)
		{
			orbit.xRotation = orbit.maxXRotation;
		}
		if(orbit.xRotation < orbit.minXRotation)
		{
			orbit.xRotation = orbit.minXRotation;
		}
	}

	void MouseOrbitTarget()
	{
		//getting the camera to orbit around our character
		previousMousePos = currentMousePos;
		currentMousePos = Input.mousePosition;

		if(mouseOrbitInput > 0)
		{
			orbit.yRotation += (currentMousePos.x - previousMousePos.x) * orbit.yOrbitSmooth;

		}

		if(vMouseOrbitInput > 0)
		{
			orbit.xRotation += (currentMousePos.y - previousMousePos.y) * orbit.xOrbitSmooth;
		}

	}

	void ZoomInOnTarget()
	{
		position.distanceFromTarget += zoomInput * position.zoomSmooth * Time.deltaTime;

		if(position.distanceFromTarget > position.maxZoom)
		{
			position.distanceFromTarget = position.maxZoom;
		}
		if(position.distanceFromTarget < position.minZoom)
		{
			position.distanceFromTarget = position.minZoom;
		}
	}

	[System.Serializable]
	public class CollisionHandler
	{
		public LayerMask collisionLayer;

		[HideInInspector]
		public bool colliding = false;
		[HideInInspector]
		public Vector3[] adjustedCameraClipPoints;
		[HideInInspector]
		public Vector3[] desiredCameraClipPoints;

		Camera camera;


		public void Initialize(Camera cam)
		{
			camera = cam;
			adjustedCameraClipPoints = new Vector3[5];
			desiredCameraClipPoints = new Vector3[5];
		}

		public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
		{
			if(!camera)
				return;

			//clear the contents of intoArray
			intoArray = new Vector3[5];

			float z = camera.nearClipPlane;
			float x = Mathf.Tan(camera.fieldOfView / 3.41f) * z;
			float y = x / camera.aspect;

			//top left
			intoArray[0] = (atRotation * new Vector3(-x, y, z)) + cameraPosition; //added and rotated the point relative to camera
			//top right
			intoArray[1] = (atRotation * new Vector3(x, y, z)) + cameraPosition; //added and rotated the point relative to camera
			// bottom left
			intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition; //added and rotated the point relative to camera
			//bottom right
			intoArray[3] = (atRotation * new Vector3(x, -y, z)) + cameraPosition; //added and rotated the point relative to camera
			//camera's position
			intoArray[4] = cameraPosition - camera.transform.forward;
		}

		bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 fromPosition)
		{
			for(int i = 0; i < clipPoints.Length; i++)
			{
				Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
				float distance = Vector3.Distance(clipPoints[i], fromPosition);
				if(Physics.Raycast(ray, distance, collisionLayer))
				{
					return true;
				}
			}

			return false;
		}

		public float GetAdjustedDistanceWithRayFrom(Vector3 from)
		{
			float distance = -1;

			for(int i = 0; i < desiredCameraClipPoints.Length; i++)
			{
				Ray ray = new Ray(from, desiredCameraClipPoints[i] - from);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit))
				{
					if(distance == -1)
						distance = hit.distance;
					else
					{
						if(hit.distance < distance)
							distance = hit.distance;
					}
				}
			}

			if(distance == -1)
				return 0;
			else
				return distance;
		}

		public void CheckColliding(Vector3 targetPosition)
		{
			if(CollisionDetectedAtClipPoints(desiredCameraClipPoints, targetPosition))
			{
				colliding = true;
			}
			else
			{
				colliding = false;
			}
		}
	}
}
