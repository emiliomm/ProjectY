using UnityEngine;
using System.Collections;

public class TP_Camera : MonoBehaviour
{
	public static TP_Camera Instance;
	public Transform TargetLookAt;

	public float Distance = 5f;
	public float DistanceMin = 3f;
	public float DistanceMax = 10f;
	public float DistanceSmooth = 0.05f; //Tiempo que tarda en variar la distancia al encontrar un obstaculo (mayor = mas lento)
	public float DistanceResumeSmooth = 0.05f; //Tiempo que tarda en variar la distancia al no encontrar ningún obstáculo(mayor = mas lento)
	public float X_MouseSensitivity = 3f;
	public float Y_MouseSensitivity = 3f;
	public float MouseWheelSentitivity = 15f;
	public float X_Smooth = 0.05f; //Tiempo que tarda en moverse la camara en su posicion en el ejeX
	public float Y_Smooth = 0.1f; //Tiempo que tarda en moverse la camara en su posicion en el ejeY
	public float Y_MinLimit = -30f; //Ángulo máximo inferior del ejeY en el que se mueve la cámara
	public float Y_MaxLimit = 40f; //Ángulo máximo superior del ejeY en el que se mueve la cámara
	public float OcclusionDistanceStep = 0.5f; //Distancia minima que se acerca la camara al encontrar un obstáculo
	public int MaxOcclusionChecks = 10; //numero maximo de comprobaciones antes de que la camara adopte la posicion directamente, sin pequeños incrementos

	private float mouseX = 0f; //Ängulo de giro del ratón en el ejeX
	private float mouseY = 0f; //Ängulo de giro del ratón en el ejeY
	private float velX = 0f;
	private float velY = 0f;
	private float velZ = 0f;
	private float velDistance = 0f;
	private float startDistance = 0f;
	private Vector3 position = Vector3.zero; //Vector de posición actual
	private Vector3 desiredPosition = Vector3.zero; //Vector de posición donde queremos estar
	private float desiredDistance = 0f;
	private float distanceSmooth = 0f;
	private float preOccludedDistance = 0f; //almacena la distancia actual de la camara hasta que el jugador cambie el zoom

	//Valores del offset
	public float offset = 1f;
	public float offset_smooth = 0.5f;
	public float offset_smooth_resume = 0.5f; //el offset al volver
	public float offset_min = 0f;
	public float offset_max = 2f;
	public bool offset_active = false;
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
		switch(TP_Controller.Instance.CurrentState)
		{
		//Si el personaje está en el estado normal, calculamos posición de la camara
		case TP_Controller.State.Normal:
			//Si no miramos a ningun sitio, salimos
			if(TargetLookAt == null)
				return;

			//Maneja los controles de la cámara
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
			break;
		//Si se encuentra en otro estado, no hacemos nada
		case TP_Controller.State.Interactuables:
		case TP_Controller.State.Dialogo:
			break;
		}
	}

	//Al mover el raton, cambiamos las coordenadas de la camara
	void HandlePlayerInput()
	{
		var deadZone = 0.01f;

		//Movemos las coordenadas del raton segun el movimiento
		//Cogemos el eje X del Input del raton multiplicada por la sensibilidad
		mouseX += Input.GetAxis ("Mouse X") * X_MouseSensitivity;

		//Cogemos el eje Y del Input del raton multiplicada por la sensibilidad
		mouseY -= Input.GetAxis ("Mouse Y") * Y_MouseSensitivity;

		//Limitamos el valor de mouseY
		mouseY = Helper.ClampAngle(mouseY, Y_MinLimit, Y_MaxLimit);

		//Si el movimiento de la rueda del ratón se encuentra fuera de la zona muerta, cambiamos el zoom
		if (Input.GetAxis("Mouse ScrollWheel") < -deadZone || Input.GetAxis("Mouse ScrollWheel") > deadZone)
		{
			//Movemos la distancia de la camara entre los valores min y max al usar la rueda del raton
			desiredDistance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel") * MouseWheelSentitivity, DistanceMin, DistanceMax);

			//Almacenamos la distancia de la camara al cambiar el zoom
			preOccludedDistance = desiredDistance;
			//Asignamos la fluidez habitual, ya que no vamos a cambiarla a ResumeSmooth
			distanceSmooth = DistanceSmooth;
		}
	}

	void CalculateDesiredPosition()
	{
		//Comprobamos si ya no hay obstáculos
		ResetDesiredDistance();

		Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velDistance, distanceSmooth);

		if(desiredDistance == preOccludedDistance)
		{
			if(Distance >= desiredDistance - 1f)
				offset_active = true;
		}

		//Calculamos la posicion deseada
		desiredPosition = CalculatePosition(mouseY, mouseX, Distance);
	}
		
	Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
	{
		Vector3 direction = new Vector3(0, 0, -distance);
		Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
		return TargetLookAt.position + rotation * direction;
	}

	//count = numero de veces que hemos comprobado hasta el momento
	bool CheckIfOccluded(int count)
	{
		var isOccluded = false;

		//Comprueba la distancia con el objeto con el que hemos colisionado más cercano
		//-1 si no hemos chocado con ninguno
		var nearestDistance = CheckCameraPoints(TargetLookAt.position, desiredPosition);

		Debug.Log("nearestDistance occluded: " + nearestDistance);

		//Si le hemos dado a algo
		if (nearestDistance != -1)
		{
			//Desactivamos el offset
			offset_active = false;

			//Si aun no nos hemos pasado con el tope de comprobaciones, acercamos la cámara hacia el personaje
			if (count < MaxOcclusionChecks)
			{
				isOccluded = true;
				Distance -= OcclusionDistanceStep; //reducimos la distancia poco a poco
			}
			//Si hemos sobrepasado el limite, movemos la camara directamente hacia el lugar "vacio", sin pequeños incrementos
			else
			{
				Distance = nearestDistance - Camera.main.nearClipPlane;
			}

			//A partir de 0.25, la camara empieza a actuar raro (el numero no es fijo, varia segun el personaje, la escena, etc)
			if (Distance < 0.25f)
			{
				Distance = 0.25f;
				offset = offset_min;
			}

			desiredDistance = Distance; //moveremos la camara hacia el punto indicado
			distanceSmooth = DistanceResumeSmooth; //La camara ya no esta bloqueada por ningun objeto, asignamos la fluidez de salida

			if (count < MaxOcclusionChecks)
			Debug.Log("desiredDistance final count: " + desiredDistance);
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

		//Dibujamos 4 lineas que van desde el lookAt hasta ĺos 4 puntos de la cámara
		Debug.DrawLine(from, to + transform.forward * -Camera.main.nearClipPlane, Color.red); //linea situada detras de la camara desde el centro
		Debug.DrawLine(from, clipPlanePoints.UpperLeft);
		Debug.DrawLine(from, clipPlanePoints.LowerLeft);
		Debug.DrawLine(from, clipPlanePoints.UpperRight);
		Debug.DrawLine(from, clipPlanePoints.LowerRight);

		//Dibujamos el rectángulo de visión de la cámara
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
		//La distancia en la que la cámara estaba obstruida ahora es mayor, podemos moverla hacia
		//afuera
		Debug.Log("preOclluded: " + preOccludedDistance);
		Debug.Log("desireddi: " + desiredDistance);
		if (desiredDistance < preOccludedDistance)
		{
			//Calculamos la nueva posicion y distancia ahora que el objeto ya no la obstruye
			var pos = CalculatePosition(mouseY, mouseX, preOccludedDistance);
//			var nearestDistance = CheckCameraPoints(TargetLookAt.position, pos);
			var nearestDistance = CheckCameraPoints2(CalculatePosition(mouseY, mouseX, desiredDistance), pos);


			Debug.Log("nearestDistamce reset: " + nearestDistance);

			//No se han detectado nuevas colisiones y la distancia anterior es mayor que la actual
			//Movemos la camara hacia atras todo lo que podemos
			if (nearestDistance == -1 || nearestDistance > preOccludedDistance)
			{
				desiredDistance = preOccludedDistance;
			}
		}
	}

	float CheckCameraPoints2(Vector3 from, Vector3 to)
	{
		var nearestDistance = -1f;

		RaycastHit hitInfo; //aqui almacenaremos info sobre lo que hemos chocado

		Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlaneAtNear(to); //cogemos el rectangulo de vision
		Helper.ClipPlanePoints clipPlanePoints2 = Helper.ClipPlaneAtNear(from); //cogemos el rectangulo de vision

		//Dibujamos 4 lineas que van desde el lookAt hasta ĺos 4 puntos de la cámara
//		Debug.DrawLine(from, to + transform.forward * -Camera.main.nearClipPlane, Color.red); //linea situada detras de la camara desde el centro
		Debug.DrawLine(clipPlanePoints2.UpperLeft, clipPlanePoints.UpperLeft, Color.red);
		Debug.DrawLine(clipPlanePoints2.LowerLeft, clipPlanePoints.LowerLeft, Color.red);
		Debug.DrawLine(clipPlanePoints2.UpperRight, clipPlanePoints.UpperRight, Color.red);
		Debug.DrawLine(clipPlanePoints2.LowerRight, clipPlanePoints.LowerRight, Color.red);

		//Dibujamos el rectángulo de visión de la cámara
		Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight, Color.red);
		Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.LowerRight, Color.red);
		Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft, Color.red);
		Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft, Color.red);

		//Dibujamos el rectángulo de visión de la cámara
		Debug.DrawLine(clipPlanePoints2.UpperLeft, clipPlanePoints2.UpperRight, Color.red);
		Debug.DrawLine(clipPlanePoints2.UpperRight, clipPlanePoints2.LowerRight, Color.red);
		Debug.DrawLine(clipPlanePoints2.LowerRight, clipPlanePoints2.LowerLeft, Color.red);
		Debug.DrawLine(clipPlanePoints2.LowerLeft, clipPlanePoints2.UpperLeft, Color.red);

		//Comprobamos que hemos dado a algo que no es el jugador
		//Necesitamos que el objeto que controla al jugador tenga el tag Player
		if (Physics.Linecast(clipPlanePoints2.UpperLeft, clipPlanePoints.UpperLeft, out hitInfo) && hitInfo.collider.tag != "Player")
			nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(clipPlanePoints2.LowerLeft, clipPlanePoints.LowerLeft, out hitInfo) && hitInfo.collider.tag != "Player")
		if (hitInfo.distance < nearestDistance || nearestDistance == -1)
			nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(clipPlanePoints2.UpperRight, clipPlanePoints.UpperRight, out hitInfo) && hitInfo.collider.tag != "Player")
		if (hitInfo.distance < nearestDistance || nearestDistance == -1)
			nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(clipPlanePoints2.LowerRight, clipPlanePoints.LowerRight, out hitInfo) && hitInfo.collider.tag != "Player")
		if (hitInfo.distance < nearestDistance || nearestDistance == -1)
			nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(from + transform.forward * -Camera.main.nearClipPlane, to + transform.forward * -Camera.main.nearClipPlane, out hitInfo) && hitInfo.collider.tag != "Player")
		if (hitInfo.distance < nearestDistance || nearestDistance == -1)
			nearestDistance = hitInfo.distance;

		return nearestDistance;
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

		//Asignamos el lookAt
		transform.LookAt(TargetLookAt);

		//Aplicamos el offset
		if (offset_active && !CheckIfOccludedOffset())
			offset = Mathf.SmoothDamp(offset, offset_max, ref offset_value, offset_smooth_resume);
		else
			offset = Mathf.SmoothDamp(offset, offset_min, ref offset_value, offset_smooth);

//		var offsetv = 0f;
//
//		if(Distance != 0)
//			offsetv = 0.5816f*Mathf.Log(Distance)+0.9427f;
//		else
//			offsetv = 0f;
//
//		offset = Mathf.SmoothDamp(offset, offsetv, ref offset_value, offset_smooth);

		transform.LookAt(TargetLookAt.position+transform.right*offset);
	}

	bool CheckIfOccludedOffset()
	{
		var isOccluded = false;

		//Comprueba la distancia con el objeto con el que hemos colisionado más cercano
		//-1 si no hemos chocado con ninguno
		var nearestDistance = CheckCameraPoints3(TargetLookAt.position+transform.right*offset_max, desiredPosition);

		//Si le hemos dado a algo
		if (nearestDistance != -1)
		{
			isOccluded = true;
		}

		return isOccluded;
	}

	float CheckCameraPoints3(Vector3 from, Vector3 to)
	{
		var nearestDistance = -1f;

		RaycastHit hitInfo; //aqui almacenaremos info sobre lo que hemos chocado

		Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlaneAtNear(to); //cogemos el rectangulo de vision

		//Dibujamos 4 lineas que van desde el lookAt hasta ĺos 4 puntos de la cámara
		Debug.DrawLine(from, to + transform.forward * -Camera.main.nearClipPlane, Color.red); //linea situada detras de la camara desde el centro
		Debug.DrawLine(from, clipPlanePoints.UpperLeft, Color.blue);
		Debug.DrawLine(from, clipPlanePoints.LowerLeft, Color.blue);
		Debug.DrawLine(from, clipPlanePoints.UpperRight, Color.blue);
		Debug.DrawLine(from, clipPlanePoints.LowerRight, Color.blue);

		//Dibujamos el rectángulo de visión de la cámara
		Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight, Color.blue);
		Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.LowerRight, Color.blue);
		Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft, Color.blue);
		Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft, Color.blue);

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
			//Cargamos el prefab de Resources
			tempCamera = (GameObject)Instantiate(Resources.Load("CameraPrefab"));
		}

		//Guardamos el componente TP_Camera(el script) en myCamera
		myCamera = tempCamera.GetComponent("TP_Camera") as TP_Camera;

		targetLookAt = GameObject.Find("targetLookAt") as GameObject;

		//si no hemos encontrado el gameobject targetLookAt (el objeto al que debemos mirar)
		if (targetLookAt == null)
		{
			//Lo creamos y lo posicionamos en 0,0,0
			targetLookAt = new GameObject("targetLookAt");
			targetLookAt.transform.position = Vector3.zero;
		}

		//ponemos la variable targetLookAt encontrada o creada en la clase
		myCamera.TargetLookAt = targetLookAt.transform;
	}

	//USAR UNA ESFERA QUE ALMACENA UNA LISTA DE OBJETOS COLISIONADOS CON ONTRIGGER
	public void GetNearestTaggedObject()
	{
		var nearestDistanceSqr = Mathf.Infinity;
		var taggedGameObjects = GameObject.FindGameObjectsWithTag("ObjectoUI"); //Añadir al Manager en un dictionary
		Transform nearestObj = null;

		//Creamos un rayo que va desde la cámara hacia adelante
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

		//Dubujamos el rayo
		Debug.DrawRay(ray.origin, ray.direction*100, Color.blue);

		//Recorremos todos los objetos, guardando el más cercano
		//Poniéndolos en estado alejado
		foreach (var obj in taggedGameObjects)
		{
			Interactuable inter = obj.gameObject.GetComponent<Interactuable> ();

			inter.SetState (Interactuable.State.Alejado);
			inter.DesactivarTextoAcciones();

			Vector3 objectPos = obj.transform.position;
			float distanceSqr, distancePlayerNPC;

			distanceSqr = DistanceToLine (ray, objectPos);
			distancePlayerNPC = (objectPos - TP_Controller.Instance.transform.position).sqrMagnitude;

			//AUMENTAR DISTANCEPLAYER O CAUSA PROBLEMAS, QUITAR DIRECTAMENTE ¿?
			if (distanceSqr < nearestDistanceSqr && distancePlayerNPC < 16.0f && inter.isRendered())
			{
				nearestObj = obj.transform;
				nearestDistanceSqr = distanceSqr;
			}
		}

		//Si existe el más cercano, le cambiamos el estado a accionable
		//Dándole foco
		if(nearestObj != null)
		{
			Interactuable inter = nearestObj.gameObject.GetComponent<Interactuable>();
			inter.SetState(Interactuable.State.Accionable);
			inter.ActivarTextoAcciones();
		}
	}

	private float DistanceToLine(Ray ray, Vector3 point)
	{
		return Vector3.Cross(ray.direction, point - ray.origin).sqrMagnitude;
	}
}