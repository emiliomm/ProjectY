using UnityEngine;

/*
 * 	Clase que se encarga de controlar la cámara
 *  Autor clase original: Tutorial Cámara 3DBuzz (https://www.3dbuzz.com/training/view/3rd-person-character-system)
 * 	Modificada por mí
 */
public class TPCamera : MonoBehaviour
{
	//Patrón singleton
	public static TPCamera instance;

	//Variable que guarda la transformación de un objeto llamado LookAt, que será hacia donde mirará la cámara
	private Transform targetLookAt;

	//Valores editables de la cámara
	public float distance = 0f;
	public float distanceMin = 2.2f;
	public float distanceMax = 10f;
	public float distanceSmooth = 0.2f; //Tiempo que tarda en variar la distancia al encontrar un obstaculo (mayor = mas lento)
	public float distanceResumeSmooth = 0.6f; //Tiempo que tarda en variar la distancia al no encontrar ningún obstáculo(mayor = mas lento)
	public float XMouseSensitivity = 3f;
	public float YMouseSensitivity = 3f;
	public float mouseWheelSentitivity = 15f;
	public float XSmooth = 0.05f; //Tiempo que tarda en moverse la camara en su posicion en el ejeX
	public float YSmooth = 0.1f; //Tiempo que tarda en moverse la camara en su posicion en el ejeY
	public float YMinLimit = -30f; //Ángulo máximo inferior del ejeY en el que se mueve la cámara
	public float YMaxLimit = 40f; //Ángulo máximo superior del ejeY en el que se mueve la cámara
	public float occlusionDistanceStep = 0.05f; //Distancia minima que se acerca la camara al encontrar un obstáculo
	public int maxOcclusionChecks = 10; //numero maximo de comprobaciones antes de que la camara adopte la posicion directamente, sin pequeños incrementos

	//Valores del offset
	public float offset = 1f;
	public float offsetSmooth = 0.1f;
	public float offsetSmoothResume = 0.5f; //el offset al volver
	public float offsetMin = 0f;
	public float offsetMax = 1.7f;

	//Indica con que colisiona la cámara
	public LayerMask layerMask;

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
	private float distance_Smooth = 0f;
	public float preOccludedDistance = 0f; //almacena la distancia actual de la camara hasta que el jugador cambie el zoom

	public bool offsetActive = false; //Indica si el offset está activado o no
	private float offsetValue = 0f;

	//Indica el porcentaje de offser aplicado
	private float offsetPercent = 0f;
	private float percentValue = 0f;

	//Valores que se usan cuando la cámara ha chocado con algo
	private float blockOffset = -1f;

	// Use this when the object is created
	void Awake ()
	{
		//Inicializamos la variable instancia
		instance = this;

		DontDestroyOnLoad(gameObject);

		//Todas las layers seleccionadas excepto UI y Player
		layerMask =  ~((1 << 5) | (1 << 9)) ;
	}

	// Use this for initialization
	void Start ()
	{
		//Validamos la distancia
		//Se asegura que la distancia esta entre los valores min y max
		distance = Mathf.Clamp(distance, distanceMin, distanceMax);
		startDistance = distance;
		Reset();//asignamos valores predeterminados
	}

	//establece las variables a valores predeterminados
	public void Reset()
	{
		mouseX = 0;
		mouseY = 10;
		distance = startDistance;
		desiredDistance = startDistance;
		preOccludedDistance = startDistance;
	}

	//crea o encuentra una camara y la asigna a la clase
	public static void UseExistingOrCreateMainCamera()
	{
		GameObject tempCamera;
		GameObject targetLookAt;
		TPCamera myCamera;

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
		myCamera = tempCamera.GetComponent<TPCamera>();

		targetLookAt = GameObject.Find("targetLookAt") as GameObject;

		//si no hemos encontrado el gameobject targetLookAt (el objeto al que debemos mirar)
		if (targetLookAt == null)
		{
			//Lo creamos y lo posicionamos en 0,0,0
			targetLookAt = new GameObject("targetLookAt");
			targetLookAt.transform.position = Vector3.zero;
		}

		//ponemos la variable targetLookAt encontrada o creada en la clase
		myCamera.targetLookAt = targetLookAt.transform;
	}

	void LateUpdate ()
	{
		switch(TPController.instance.CurrentState)
		{
		//Si el personaje está en el estado normal, calculamos posición de la camara
		case TPController.State.Normal:
			//Si no miramos a ningun sitio, salimos
			if(targetLookAt == null)
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
		case TPController.State.Interactuables:
		case TPController.State.Dialogo:
			break;
		}
	}

	//Al mover el raton, cambiamos las coordenadas de la camara
	private void HandlePlayerInput()
	{
		var deadZone = 0.01f;

		//Movemos las coordenadas del raton segun el movimiento
		//Cogemos el eje X del Input del raton multiplicada por la sensibilidad
		mouseX += Input.GetAxis ("Mouse X") * XMouseSensitivity;

		//Cogemos el eje Y del Input del raton multiplicada por la sensibilidad
		mouseY -= Input.GetAxis ("Mouse Y") * YMouseSensitivity;

		//Limitamos el valor de mouseY
		mouseY = Helper.ClampAngle(mouseY, YMinLimit, YMaxLimit);

		//Si el movimiento de la rueda del ratón se encuentra fuera de la zona muerta, cambiamos el zoom
		if (Input.GetAxis("Mouse ScrollWheel") < -deadZone || Input.GetAxis("Mouse ScrollWheel") > deadZone)
		{
			//Movemos la distancia de la camara entre los valores min y max al usar la rueda del raton
			desiredDistance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * mouseWheelSentitivity, distanceMin, distanceMax);

			//Almacenamos la distancia de la camara al cambiar el zoom
			preOccludedDistance = desiredDistance;
			//Asignamos la fluidez habitual, ya que no vamos a cambiarla a ResumeSmooth
			distance_Smooth = distanceSmooth;
		}
	}

	private void CalculateDesiredPosition()
	{
		//Comprobamos si ya no hay obstáculos
		ResetDesiredDistance();

		distance = Mathf.SmoothDamp(distance, desiredDistance, ref velDistance, distance_Smooth);

		//Calculamos la posicion deseada
		desiredPosition = CalculatePosition(mouseY, mouseX, distance);
	}

	//Calcula la posición de la cámara según su distancia y la rotación desde el lookat
	Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
	{
		Vector3 direction = new Vector3(0, 0, -distance);
		Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
		return targetLookAt.position + rotation * direction;
	}

	//Comprueba si se interpone algo entre la cámara y el lookat
	//count = numero de veces que hemos comprobado hasta el momento
	private bool CheckIfOccluded(int count)
	{
		var isOccluded = false;

		//Comprueba la distancia con el objeto con el que hemos colisionado más cercano
		//-1 si no hemos chocado con ninguno
		var nearestDistance = CheckCameraPoints(targetLookAt.position, desiredPosition);

		//Si le hemos dado a algo
		if (nearestDistance != -1)
		{
			//Desactivamos el offset
			offsetActive = false;

			//Si no lo hemos hecho antes, guardamos determinadas variables
			if(blockOffset == -1f)
			{
				blockOffset = offset;
			}

			//Si aun no nos hemos pasado con el tope de comprobaciones, acercamos la cámara hacia el personaje
			if (count < maxOcclusionChecks)
			{
				isOccluded = true;
				distance -= occlusionDistanceStep; //reducimos la distancia poco a poco
			}
			//Si hemos sobrepasado el limite, movemos la camara directamente hacia el lugar "vacio", sin pequeños incrementos
			else
			{
				distance = nearestDistance - Camera.main.nearClipPlane;
			}

			//A partir de 0.25, la camara empieza a actuar raro (el numero no es fijo, varia segun el personaje, la escena, etc)
			if (distance < 0.25f)
			{
				distance = 0.25f;
			}

			desiredDistance = distance; //moveremos la camara hacia el punto indicado
			distance_Smooth = distanceResumeSmooth; //La camara ya no esta bloqueada por ningun objeto, asignamos la fluidez de salida
		}
			
		return isOccluded;
	}

	//Calculamos los puntos que la camara necesita para moverse si un objeto tapa la vista
	//Devuelve la distancia mas cercana de lo que hemos chocado, si hemos chocado con algo
	//sino, devuelve negativo
	/*
	 * Para que un objeto no afecte a la cámara y lo pueda atravesar podemos modificar
	 * la layerMask
	 * 
	*/
	private float CheckCameraPoints(Vector3 from, Vector3 to)
	{
		var nearestDistance = -1f;

		RaycastHit hitInfo; //aqui almacenaremos info sobre lo que hemos chocado

		Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlaneAtNear(to); //cogemos el rectangulo de vision

		//Dibujamos 4 lineas que van desde el lookAt hasta ĺos 4 puntos de la cámara
		Debug.DrawLine(from, to + transform.forward * -Camera.main.nearClipPlane, Color.red); //linea situada detras de la camara desde el centro
		Debug.DrawLine(from, clipPlanePoints.upperLeft);
		Debug.DrawLine(from, clipPlanePoints.lowerLeft);
		Debug.DrawLine(from, clipPlanePoints.upperRight);
		Debug.DrawLine(from, clipPlanePoints.lowerRight);

		//Dibujamos el rectángulo de visión de la cámara
		Debug.DrawLine(clipPlanePoints.upperLeft, clipPlanePoints.upperRight);
		Debug.DrawLine(clipPlanePoints.upperRight, clipPlanePoints.lowerRight);
		Debug.DrawLine(clipPlanePoints.lowerRight, clipPlanePoints.lowerLeft);
		Debug.DrawLine(clipPlanePoints.lowerLeft, clipPlanePoints.upperLeft);

		//Comprobamos que hemos dado a algo que no es la layermask
		if (Physics.Linecast(from, clipPlanePoints.upperLeft, out hitInfo, layerMask))
			nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(from, clipPlanePoints.lowerLeft, out hitInfo, layerMask))
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(from, clipPlanePoints.upperRight, out hitInfo, layerMask))
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(from, clipPlanePoints.lowerRight, out hitInfo, layerMask))
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(from, to + transform.forward * -Camera.main.nearClipPlane, out hitInfo, layerMask))
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

		return nearestDistance;
	}

	//Realiza la misma función que CheckCameraPoints, solo que utilizando el reactángulo de visión de la cámara en los dos extremos
	private float CheckCameraPoints2(Vector3 from, Vector3 to)
	{
		var nearestDistance = -1f;

		RaycastHit hitInfo; //aqui almacenaremos info sobre lo que hemos chocado

		Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlaneAtNear(to);
		Helper.ClipPlanePoints clipPlanePoints2 = Helper.ClipPlaneAtNear(from); //cogemos el rectangulo de vision

		//Dibujamos 4 lineas que van desde el lookAt hasta ĺos 4 puntos de la cámara
		Debug.DrawLine(clipPlanePoints2.upperLeft, clipPlanePoints.upperLeft, Color.red);
		Debug.DrawLine(clipPlanePoints2.lowerLeft, clipPlanePoints.lowerLeft, Color.red);
		Debug.DrawLine(clipPlanePoints2.upperRight, clipPlanePoints.upperRight, Color.red);
		Debug.DrawLine(clipPlanePoints2.lowerRight, clipPlanePoints.lowerRight, Color.red);

		//Dibujamos el rectángulo de visión de la cámara
		Debug.DrawLine(clipPlanePoints.upperLeft, clipPlanePoints.upperRight, Color.red);
		Debug.DrawLine(clipPlanePoints.upperRight, clipPlanePoints.lowerRight, Color.red);
		Debug.DrawLine(clipPlanePoints.lowerRight, clipPlanePoints.lowerLeft, Color.red);
		Debug.DrawLine(clipPlanePoints.lowerLeft, clipPlanePoints.upperLeft, Color.red);

		//Dibujamos el rectángulo de visión 2 de la cámara
		Debug.DrawLine(clipPlanePoints2.upperLeft, clipPlanePoints2.upperRight, Color.red);
		Debug.DrawLine(clipPlanePoints2.upperRight, clipPlanePoints2.lowerRight, Color.red);
		Debug.DrawLine(clipPlanePoints2.lowerRight, clipPlanePoints2.lowerLeft, Color.red);
		Debug.DrawLine(clipPlanePoints2.lowerLeft, clipPlanePoints2.upperLeft, Color.red);

		//Comprobamos que hemos dado a algo que no es la layermask
		if (Physics.Linecast(clipPlanePoints2.upperLeft, clipPlanePoints.upperLeft, out hitInfo, layerMask))
			nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(clipPlanePoints2.lowerLeft, clipPlanePoints.lowerLeft, out hitInfo, layerMask))
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(clipPlanePoints2.upperRight, clipPlanePoints.upperRight, out hitInfo, layerMask))
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(clipPlanePoints2.lowerRight, clipPlanePoints.lowerRight, out hitInfo, layerMask))
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(from + transform.forward * -Camera.main.nearClipPlane, to + transform.forward * -Camera.main.nearClipPlane, out hitInfo, layerMask))
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

		return nearestDistance;
	}

	private void ResetDesiredDistance()
	{
		//Si la distancia a la que estaba la cámara era mayor que la que nos estamos moviendo, comprobamos
		//si nos podemos alejar
		if (desiredDistance < preOccludedDistance)
		{
			//Si nos hemos chocado con algo, usamos las variables block
			var offset_used = offset;

			if(blockOffset != -1f)
			{
				offset_used = blockOffset;
			}

			var pos = CalculatePosition(mouseY, mouseX, preOccludedDistance);
			var nearestDistance = CheckCameraPoints2(targetLookAt.position, pos);

			transform.LookAt(targetLookAt.position+transform.right*offset_used*offsetPercent);

			var posOffset = CalculatePosition(mouseY, mouseX, preOccludedDistance);
			var nearestDistanceOffset = CheckCameraPoints2(targetLookAt.position, posOffset);

			//No se han detectado nuevas colisiones o la distancia del choque es mayor que la actual
			//Movemos la camara hacia atras todo lo que podemos
			if ((nearestDistanceOffset == -1 && nearestDistance == -1) || nearestDistanceOffset > preOccludedDistance)
			{
				desiredDistance = preOccludedDistance;
			}

			//Si no nos hemos chocado con nada, aplicamos valores por defecto a las variables block
			if(nearestDistanceOffset == -1)
			{
				blockOffset = -1;
			}

			transform.LookAt(targetLookAt.position+transform.right*offset*offsetPercent);
		}

		//Si vamos a estar en la posición que queremos antes del choque, activamos el offset
		if(desiredDistance == preOccludedDistance)
		{
			offsetActive = true;
		}
	}
		
	//Movemos la cámara a la posición deseada
	private void UpdatePosition()
	{
		//posiciones con los valores suavizados
		var posX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, XSmooth);
		var posY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, YSmooth);
		var posZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, XSmooth);
		position = new Vector3(posX, posY, posZ);

		//Asignamos la posicion actual con la posicion suavizada
		transform.position = position;

		//Asignamos el lookAt
		transform.LookAt(targetLookAt);

		//Aplicamos el offset
		//if (offset_active && !CheckIfOccludedOffset())
		if (offsetActive)
		{
			offset = Mathf.SmoothDamp(offset, offsetMax, ref offsetValue, offsetSmoothResume);
		}
		else
		{
			//offset = Mathf.SmoothDamp(offset, offset_min, ref offset_value, offset_smooth);
			var offsetM = (1f/2.2f)*distance;
			offset = Mathf.SmoothDamp(offset, offsetM, ref offsetValue, offsetSmooth);
		}

		//Calculamos el porcentaje de offset que aplicamos según distancia si la diferencia es mayor que 0.001
		if (Mathf.Abs(offsetPercent - distance/preOccludedDistance) > 0.001f )
			offsetPercent = Mathf.SmoothDamp(offsetPercent, distance/preOccludedDistance, ref percentValue, offsetSmooth);

		//Aplicamos el lookat de nuevo, pero con valores del offset aplicados
		var targetPos = targetLookAt.position+transform.right*offset*offsetPercent;

		transform.LookAt(targetPos);
	}

	//Establecemos el modo de visión normal de las cámaras
	public void SetNormalMode()
	{
		Camera.main.cullingMask = 1 << 9; //Jugador
		Camera.main.clearFlags = CameraClearFlags.Nothing;
		Camera.main.backgroundColor = new Color32(49, 77, 121, 5);
	}

	//Cuando vamos a dialogar, desactivamos la cámara UI de dos cámaras para no ver la UI
	public void ToDialogMode()
	{
		Camera camara = Camera.main.transform.GetChild(0).gameObject.GetComponent<Camera>();
		camara.cullingMask = ~(1 << 5); //Desactiva la capa UI

		camara = camara.transform.GetChild(0).gameObject.GetComponent<Camera>();
		camara.cullingMask = ~(1 << 5); //Desactiva la capa UI
	}

	//Cuando acabamos el diálogo, activamos todas las capas de la cámara principal
	public void FromDialogMode()
	{
		Camera camara = Camera.main.transform.GetChild(0).gameObject.GetComponent<Camera>();
		camara.cullingMask = -1; //Activa todas las capas

		camara = camara.transform.GetChild(0).gameObject.GetComponent<Camera>();
		camara.cullingMask = ~(1 << 5); //Desactiva la capa UI
	}

	//Establecemos el modo de visión objeto, cuando estamos manipulando el objeto de alguna acción
	public void SetObjectMode()
	{
		Camera.main.cullingMask = 1 << 8; //UIObjeto
		Camera.main.clearFlags = CameraClearFlags.SolidColor;
		Camera.main.backgroundColor = new Color32(73, 67, 67, 0);
	}

	//Calcula la posición de la cámara durante el diálogo según el que está hablando actualmente y
	//lo que debemos mover la cámara respecto a la dirección de este
	public void PosicionDialogo(PosicionCamara posicionCamara, GameObject interactuableGO)
	{
		//La cámara mira al jugador
		if (posicionCamara.lookAt == -1)
		{
			//- 1.769/2 (mitad cuerpo) temporal
			Vector3 translado = targetLookAt.TransformDirection(new Vector3(posicionCamara.coordX, posicionCamara.coordY - 1.769f/2, posicionCamara.coordZ));

			Vector3 pos = targetLookAt.position + translado;

			//Asignamos la posicion actual con la posicion suavizada
			transform.position = pos;

			//Asignamos el lookAt
			//- 1.769/2 (mitad cuerpo) temporal
			transform.LookAt(targetLookAt.position - new Vector3(0f,1.769f/2,0f));
		}
		//La cámara mira al Interactuable
		else
		{
			Vector3 translado = interactuableGO.transform.TransformDirection(new Vector3(posicionCamara.coordX, posicionCamara.coordY, posicionCamara.coordZ));

			Vector3 pos = interactuableGO.transform.position + translado;

			transform.position = pos;

			transform.LookAt(interactuableGO.transform);
		}
	}
}