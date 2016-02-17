using UnityEngine;
using System.Collections;

public class TP_Camera : MonoBehaviour
{
	public static TP_Camera Instance;

	public Transform TargetLookAt;
	public Transform TargetLookAtOffset;

	public float Distance = 3.5f;
	public float DistanceMin = 2f;
	public float DistanceMax = 3.5f;
	public float DistanceSmooth = 0.05f; //Intervalos a los que se debe mover el smooth de la Distance
	public float DistanceResumeSmooth = 0.4f; //Velocidad a la que la cámara vuelve a la posición original tras no detectar obstáculos

	public float LookAtMinY = 0f; //posicion y del lookAt cuando la distancia es alta
	public float LookAtMaxY = 1.5f; //posicion y del lookAt cuando la distancia es baja
	public float LookAtSmooth = 0.4f; //Intervalos a los que se debe mover el smooth de la velLookAt
	public float LookAtResumeSmooth = 2f; //Distancia a la que el LookAt se situa encima de la cabeza

	public float X_MouseSensitivity = 5f;
	public float Y_MouseSensitivity = 5f;
	public float X_Smooth = 0.05f;
	public float Y_Smooth = 0.1f;
	public float Y_MinLimit = -15f;
	public float Y_MaxLimit = 80f;
	public float MouseWheelSentitivity = 15f;

	public float OcclusionDistanceStep = 0.5f;
	public int MaxOcclusionChecks = 10; //numero maximo de comprobaciones antes de que la camara adopte la posicion directamente, sin pequeños incrementos

	private float mouseX = 0f;
	private float mouseY = 0f;
	private float velX = 0f;
	private float velY = 0f;
	private float velZ = 0f;
	private float velDistance = 0f; //almacena el valor temporal de la velocidad al cambiar la distancia con la rueda del raton para usar la funcion smooth
	private float velLookAt = 0f; //almacena el valor temporal de la velocidad al cambiar el lookat para usar la funcion smooth
	private float startDistance = 0f;
	private Vector3 position = Vector3.zero;
	private Vector3 desiredPosition = Vector3.zero;
	private float desiredDistance = 0f;
	private float distanceSmooth = 0f;
	private float preOccludedDistance = 0f; //almacena la distancia actual de la camara hasta que el jugador cambie el zoom

	private float offset = 1f;
	private float offset_value = 0f;

	// Use this when the object is created
	void Awake ()
	{
		//Inicializamos la variable instancia
		Instance = this;
	}

	// Use this for initialization
	void Start ()
	{
		//Validamos la distancia
		//Se asegura que la distancia esta entre los valores min y max
		Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
		startDistance = Distance;
		Reset();//asignamos valores predeterminados
	}

	void LateUpdate ()
	{
		//Si no miramos a ningun sitio, salimos
		if(TargetLookAt == null || !TP_Controller.Instance.canMove)
			return;

		HandlePlayerInput();

		var count = 0;

		//Calculamos la posicion de la camara hasta que no este obstruida
		do
		{
			CalculateDesiredPosition();
			count++;
		}while (CheckIfOccluded(count));

		//Actualizamos la posicion
		UpdatePosition();
	}

	//Al mover el raton, cambiamos las coordenadas de la camara
	void HandlePlayerInput()
	{
		var deadZone = 0.01f;

		//Movemos las coordenadas del raton segun el movimiento
		//Cogemos el Input X del raton multiplicada por la sensibilidad
		mouseX += Input.GetAxis ("Mouse X") * X_MouseSensitivity;

		//Cogemos el Input Y del raton multiplicada por la sensibilidad
		mouseY -= Input.GetAxis ("Mouse Y") * Y_MouseSensitivity;

		//Limitamos el valor de mouseY
		mouseY = Helper.ClampAngle(mouseY, Y_MinLimit, Y_MaxLimit);

		//Si el raton se encuentra fuera de la zona muerta, es decir, permitimos el movimiento
		if (Input.GetAxis("Mouse ScrollWheel") < -deadZone || Input.GetAxis("Mouse ScrollWheel") > deadZone)
		{
			//Movemos la distancia de la camara entre los valores min y max al usar la rueda del raton
			desiredDistance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel") * MouseWheelSentitivity, DistanceMin, DistanceMax);

			//Almacenamos la distancia de la camara al cambiar el zoom
			preOccludedDistance = desiredDistance;
			//Asignamos la fluidez habitual, ya que no vamos a cambiarla a DistanceResumeSmooth
			distanceSmooth = DistanceSmooth;
		}
	}

	void CalculateDesiredPosition()
	{
		//Calculamos la distancia necesaria para llegar hasta el punto que queremos con la velocidad y fluidez deseadas
		ResetDesiredDistance();
		Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velDistance, distanceSmooth);

		//Calculamos la posicion deseada
		desiredPosition = CalculatePosition(mouseY, mouseX, Distance);
	}

	void UpdateTargetLookAt()
	{
		//Si la distancia es menor que la establecida (está muy cerca)
		//El lookAt se mueve hacia arriba en el eje y
		if(Distance < LookAtResumeSmooth)
		{
			//Vector3 localPos = TargetLookAt.localPosition;
			//localPos.y = Mathf.SmoothDamp(localPos.y, LookAtMaxY, ref velLookAt, LookAtSmooth);
			//TargetLookAt.localPosition = localPos;

			offset = Mathf.SmoothDamp(offset, 0f, ref offset_value, LookAtSmooth);


		}
		else
		{
			//Vector3 localPos = TargetLookAt.localPosition;
			//localPos.y = Mathf.SmoothDamp(localPos.y, LookAtMinY, ref velLookAt, LookAtSmooth);
			//TargetLookAt.localPosition = localPos;

			offset = Mathf.SmoothDamp(offset, 2f, ref offset_value, LookAtSmooth);
		}
	}

	Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
	{
		Vector3 direction = new Vector3(0, 0, -distance);
		Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);

		return TargetLookAt.position + rotation * direction + transform.right * LookAt_Controller.Instance.offset;
//		return TargetLookAtOffset.position + rotation * direction;
//		return TargetLookAt.position + rotation * direction;
//		return LookAt_Controller.Instance.transform.position + rotation*direction;
	}

	//count = numero de veces que hemos comprobado hasta el momento
	bool CheckIfOccluded(int count)
	{
		var isOccluded = false;

		var nearestDistance = CheckCameraPoints(TargetLookAt.position, desiredPosition);
//		var nearestDistance = CheckCameraPoints(TargetLookAtOffset.position, desiredPosition);

		//Si le hemos dado a algo
		if (nearestDistance != -1)
		{
			//Si aun no nos hemos pasado con el tope de comprobaciones, movemos la camara hacia delante
			if (count < MaxOcclusionChecks)
			{
				isOccluded = true;
				Distance -= OcclusionDistanceStep; //reducimos la distancia poco a poco

				//A partir de 0.25, la camara empieza a actuar raro (el numero no es fijo, varia segun el personaje, la escena, etc)
				if (Distance < 0.25f)
					Distance = 0.25f;
			}
			//Si hemos sobrepasado el limite, movemos la camara directamente hacia el lugar "vacio", sin pequeños incrementos
			else
			{
				Distance = nearestDistance - Camera.main.nearClipPlane;
			}

			desiredDistance = Distance; //moveremos la camara hacia el punto indicado
			distanceSmooth = DistanceResumeSmooth; //La camara ya no esta bloqueada por ningun objeto, asignamos la fluidez de salida

//			UpdateTargetLookAt();
		}
			
		return isOccluded;
	}

	//calculamos los puntos que la camara necesita para moverse si un objeto tapa la vista
	//devuelve la distancia mas cercana de lo que hemos chocado, si hemos chocado con algo
	//sino, devuelve negativo

	/*
	 * Para que un objeto no afecte a la cámara y lo pueda atravesar podemos poner
	 * en el Inspector > Layer > Ignore Raycast
	 * 
	*/
	float CheckCameraPoints(Vector3 from, Vector3 to)
	{
		var nearestDistance = -1f;

		RaycastHit hitInfo; //aqui almacenaremos info sobre lo que hemos chocado

		Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlaneAtNear(to); //cogemos el rectangulo de vision

		//Dibujamos lineas en el editor para que sea mas facil visualizarlo
		Debug.DrawLine(from, to + transform.forward * -Camera.main.nearClipPlane, Color.red); //linea situada detras de la camara desde el centro
		Debug.DrawLine(from, clipPlanePoints.UpperLeft);
		Debug.DrawLine(from, clipPlanePoints.LowerLeft);
		Debug.DrawLine(from, clipPlanePoints.UpperRight);
		Debug.DrawLine(from, clipPlanePoints.LowerRight);

		Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight);
		Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.LowerRight);
		Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft);
		Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft);

		//Comprobamos que hemos dado a algo que no es el jugador
		//Necesitamos que el objeto que controla al jugador tenga el tag Player
		if (Physics.Linecast(from, clipPlanePoints.UpperLeft, out hitInfo) && hitInfo.collider.tag != "Player")
			nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(from, clipPlanePoints.LowerLeft, out hitInfo) && hitInfo.collider.tag != "Player")
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(from, clipPlanePoints.UpperRight, out hitInfo) && hitInfo.collider.tag != "Player")
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(from, clipPlanePoints.LowerRight, out hitInfo) && hitInfo.collider.tag != "Player")
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(from, to + transform.forward * -Camera.main.nearClipPlane, out hitInfo) && hitInfo.collider.tag != "Player")
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

		return nearestDistance;
	}

	void ResetDesiredDistance()
	{
		//la camara ha variado su zoom ya que un objeto la obstruia
		if (desiredDistance < preOccludedDistance)
		{
			//Calculamos la nueva posicion y distancia ahora que el objeto ya no la obstruye
			var pos = CalculatePosition(mouseY, mouseX, preOccludedDistance);
			var nearestDistance = CheckCameraPoints(TargetLookAt.position, pos);
//			var nearestDistance = CheckCameraPoints(TargetLookAtOffset.position, pos);

			//No se han detectado nuevas colisiones y la distancia anterior es mayor que la actual
			//Movemos la camara hacia atras todo lo que podemos
			if (nearestDistance == -1 || nearestDistance > preOccludedDistance)
			{
				desiredDistance = preOccludedDistance;
			}
		}
	}

	void UpdatePosition()
	{
		//posiciones con los valores suavizados
		var posX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, X_Smooth);
		var posY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, Y_Smooth);
		var posZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, X_Smooth);

		position = new Vector3(posX, posY, posZ);

		//Asignamos la posicion actual con la posicion suavizada
		transform.position = position;

		//var lookatoffset = TargetLookAt.position + transform.right * LookAt_Controller.Instance.offset;
		LookAt_Controller.Instance.UpdatePosition(TargetLookAt.position, transform, TargetLookAt.transform);

//		transform.LookAt(lookatoffset);

//		TargetLookAtOffset.transform.position = lookatoffset;
//		TargetLookAtOffset.transform.rotation = transform.rotation;

//		TargetLookAtOffset.transform.position = LookAt_Controller.Instance.transform.position;
//		TargetLookAtOffset.transform.rotation = transform.rotation;

//		transform.LookAt(TargetLookAt);
//		transform.LookAt(TargetLookAtOffset);
		transform.LookAt(LookAt_Controller.Instance.transform);
	}

	//establece las variables a valores predeterminados
	public void Reset()
	{
		mouseX = 0;
		mouseY = 10;
		Distance = startDistance;
		desiredDistance = startDistance;
		preOccludedDistance = startDistance;
	}

	//crea o encuentra una camara y la asigna a la clase
	public static void UseExistingOrCreateMainCamera()
	{
		GameObject tempCamera;
		GameObject targetLookAt;
		TP_Camera myCamera;

		//Si la camara existe
		if(Camera.main != null)
		{
			tempCamera = Camera.main.gameObject;
		}
		else
		{
			//Creamos la camara, le añadimos el componente camera y el tag main camera
			tempCamera = new GameObject("Main Camera");
			tempCamera.AddComponent<Camera>();
			tempCamera.tag = "MainCamera";
		}

		//Añadimos el componente TP_Camera(el script) y lo guardamos en myCamera
		tempCamera.AddComponent<TP_Camera>();
		myCamera = tempCamera.GetComponent("TP_Camera") as TP_Camera;

		targetLookAt = GameObject.Find("targetLookAt") as GameObject;

		//si no hemos encontrado el gameobject targetLookAt (el objeto al que debemos mirar), lo creamos
		if (targetLookAt == null)
		{
			//Lo creamos y lo posicionamos en 0,0,0
			targetLookAt = new GameObject("targetLookAt");
			targetLookAt.transform.position = Vector3.zero;
		}

		//ponemos la variable targetLookAt encontrada o creada en la clase
		myCamera.TargetLookAt = targetLookAt.transform;

		targetLookAt = GameObject.Find("targetLookAtOffset") as GameObject;

		//si no hemos encontrado el gameobject targetLookAt (el objeto al que debemos mirar), lo creamos
		if (targetLookAt == null)
		{
			//Lo creamos y lo posicionamos en 0,0,0
			targetLookAt = new GameObject("targetLookAtOffset");
			targetLookAt.transform.position = Vector3.zero;
		}

		//ponemos la variable targetLookAt encontrada o creada en la clase
		myCamera.TargetLookAtOffset = targetLookAt.transform;
	}
}
