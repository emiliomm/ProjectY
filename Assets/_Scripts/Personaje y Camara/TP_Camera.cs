using UnityEngine;

/*
 * 	Clase que se encarga de controlar la cámara
 *  Autor clase original: Tutorial Cámara 3DBuzz (https://www.3dbuzz.com/training/view/3rd-person-character-system)
 * 	Modificada por mí
 */
public class TP_Camera : MonoBehaviour
{
	//Patrón singleton
	public static TP_Camera Instance;

	//Variable que guarda la transformación de un objeto llamado LookAt, que será hacia donde mirará la cámara
	public Transform TargetLookAt;

	//Valores editables de la cámara
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
	public float OcclusionDistanceStep = 0.05f; //Distancia minima que se acerca la camara al encontrar un obstáculo
	public int MaxOcclusionChecks = 10; //numero maximo de comprobaciones antes de que la camara adopte la posicion directamente, sin pequeños incrementos

	//Valores del offset
	public float offset = 1f;
	public float offset_smooth = 0.5f;
	public float offset_smooth_resume = 0.5f; //el offset al volver
	public float offset_min = 0f;
	public float offset_max = 2f;

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
	private float distanceSmooth = 0f;
	private float preOccludedDistance = 0f; //almacena la distancia actual de la camara hasta que el jugador cambie el zoom

	public bool offset_active = false; //Indica si el offset está activado o no
	private float offset_value = 0f;

	//Indica el porcentaje de offser aplicado
	private float offsetPercent = 0f;
	private float percent_value = 0f;

	//Valores que se usan cuando la cámara ha chocado con algo
	private float block_offset = -1f;
	private float block_percent = -1f;
	private Vector3 block_right = Vector3.zero;

	// Use this when the object is created
	void Awake ()
	{
		//Inicializamos la variable instancia
		Instance = this;

		DontDestroyOnLoad(gameObject);
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
	private void HandlePlayerInput()
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

	private void CalculateDesiredPosition()
	{
		//Comprobamos si ya no hay obstáculos
		ResetDesiredDistance();

		Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velDistance, distanceSmooth);

		//Calculamos la posicion deseada
		desiredPosition = CalculatePosition(mouseY, mouseX, Distance);
	}

	//Calcula la posición de la cámara según su distancia y la rotación desde el lookat
	Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
	{
		Vector3 direction = new Vector3(0, 0, -distance);
		Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
		return TargetLookAt.position + rotation * direction;
	}

	//Comprueba si se interpone algo entre la cámara y el lookat
	//count = numero de veces que hemos comprobado hasta el momento
	private bool CheckIfOccluded(int count)
	{
		var isOccluded = false;

		//Comprueba la distancia con el objeto con el que hemos colisionado más cercano
		//-1 si no hemos chocado con ninguno
		var nearestDistance = CheckCameraPoints(TargetLookAt.position, desiredPosition);

		//Si le hemos dado a algo
		if (nearestDistance != -1)
		{
			//Desactivamos el offset
			offset_active = false;

			//Si no lo hemos hecho antes, guardamos determinadas variables
			if(block_offset == -1f)
			{
				block_offset = offset;
				block_percent = offsetPercent;
				block_right = transform.right;
			}

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
			}

			desiredDistance = Distance; //moveremos la camara hacia el punto indicado
			distanceSmooth = DistanceResumeSmooth; //La camara ya no esta bloqueada por ningun objeto, asignamos la fluidez de salida
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
		Debug.DrawLine(from, clipPlanePoints.UpperLeft);
		Debug.DrawLine(from, clipPlanePoints.LowerLeft);
		Debug.DrawLine(from, clipPlanePoints.UpperRight);
		Debug.DrawLine(from, clipPlanePoints.LowerRight);

		//Dibujamos el rectángulo de visión de la cámara
		Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight);
		Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.LowerRight);
		Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft);
		Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft);

		//Comprobamos que hemos dado a algo que no es la layermask
		if (Physics.Linecast(from, clipPlanePoints.UpperLeft, out hitInfo, layerMask))
			nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(from, clipPlanePoints.LowerLeft, out hitInfo, layerMask))
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(from, clipPlanePoints.UpperRight, out hitInfo, layerMask))
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(from, clipPlanePoints.LowerRight, out hitInfo, layerMask))
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
		Debug.DrawLine(clipPlanePoints2.UpperLeft, clipPlanePoints.UpperLeft, Color.red);
		Debug.DrawLine(clipPlanePoints2.LowerLeft, clipPlanePoints.LowerLeft, Color.red);
		Debug.DrawLine(clipPlanePoints2.UpperRight, clipPlanePoints.UpperRight, Color.red);
		Debug.DrawLine(clipPlanePoints2.LowerRight, clipPlanePoints.LowerRight, Color.red);

		//Dibujamos el rectángulo de visión de la cámara
		Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight, Color.red);
		Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.LowerRight, Color.red);
		Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft, Color.red);
		Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft, Color.red);

		//Dibujamos el rectángulo de visión 2 de la cámara
		Debug.DrawLine(clipPlanePoints2.UpperLeft, clipPlanePoints2.UpperRight, Color.red);
		Debug.DrawLine(clipPlanePoints2.UpperRight, clipPlanePoints2.LowerRight, Color.red);
		Debug.DrawLine(clipPlanePoints2.LowerRight, clipPlanePoints2.LowerLeft, Color.red);
		Debug.DrawLine(clipPlanePoints2.LowerLeft, clipPlanePoints2.UpperLeft, Color.red);

		//Comprobamos que hemos dado a algo que no es la layermask
		if (Physics.Linecast(clipPlanePoints2.UpperLeft, clipPlanePoints.UpperLeft, out hitInfo, layerMask))
			nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(clipPlanePoints2.LowerLeft, clipPlanePoints.LowerLeft, out hitInfo, layerMask))
		if (hitInfo.distance < nearestDistance || nearestDistance == -1)
			nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(clipPlanePoints2.UpperRight, clipPlanePoints.UpperRight, out hitInfo, layerMask))
		if (hitInfo.distance < nearestDistance || nearestDistance == -1)
			nearestDistance = hitInfo.distance;

		//Si le hemos dado a algo mas cercano, cambiamos la distancia mas cercana
		if (Physics.Linecast(clipPlanePoints2.LowerRight, clipPlanePoints.LowerRight, out hitInfo, layerMask))
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
			var pos = CalculatePosition(mouseY, mouseX, preOccludedDistance);

			//Si nos hemos chocado con algo, usamos las variables block
			var offset_used = offset;
			var percent_used = offsetPercent;
			var right_used = transform.right;

			if(block_offset != -1f)
			{
				offset_used = block_offset;
				percent_used = block_percent;
				right_used = block_right;
			}

			//Comprobamos si nos hemos chocado con algo con el lookat con el offset aplicado
			transform.LookAt(TargetLookAt.position+right_used*offset_used*percent_used);
			var nearestDistance = CheckCameraPoints2(TargetLookAt.position, pos);
			transform.LookAt(TargetLookAt.position+transform.right*offset*offsetPercent);

			//No se han detectado nuevas colisiones o la distancia del choque es mayor que la actual
			//Movemos la camara hacia atras todo lo que podemos
			if (nearestDistance == -1 || nearestDistance > preOccludedDistance)
			{
				desiredDistance = preOccludedDistance;
			}

			//Si no nos hemos chocado con nada, aplicamos valores por defecto a las variables block
			if(nearestDistance == -1)
			{
				block_offset = -1;
				block_percent = -1;
				block_right = Vector3.zero;
			}
		}

		//Si vamos a estar en la posición que queremos antes del choque, activamos el offset
		if(desiredDistance == preOccludedDistance)
		{
			offset_active = true;
		}
	}
		
	//Movemos la cámara a la posición deseada
	private void UpdatePosition()
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
		//if (offset_active && !CheckIfOccludedOffset())
		if (offset_active)
		{
			offset = Mathf.SmoothDamp(offset, offset_max, ref offset_value, offset_smooth_resume);
		}
		else
		{
			offset = Mathf.SmoothDamp(offset, offset_min, ref offset_value, offset_smooth);
		}

		//Calculamos el porcentaje de offset que aplicamos según distancia si la diferencia es mayor que 0.001
		if (Mathf.Abs(offsetPercent - Distance/preOccludedDistance) > 0.001f )
			offsetPercent = Mathf.SmoothDamp(offsetPercent, Distance/preOccludedDistance, ref percent_value, offset_smooth);

		//Aplicamos el lookat de nuevo, pero con valores del offset aplicados
		var targetPos = TargetLookAt.position+transform.right*offset*offsetPercent;

		transform.LookAt(targetPos);
	}

	//Establecemos el modo de visión normal de las cámaras
	public void setNormalMode()
	{
		Camera.main.cullingMask = 1 << 9; //Jugador
		Camera.main.clearFlags = CameraClearFlags.Nothing;
		Camera.main.backgroundColor = new Color32(49, 77, 121, 5);
	}

	//Cuando vamos a dialogar, desactivamos la cámara UI de dos cámaras para no ver la UI
	public void toDialogMode()
	{
		Camera camara = Camera.main.transform.GetChild(0).gameObject.GetComponent<Camera>();
		camara.cullingMask = ~(1 << 5); //Desactiva la capa UI

		camara = camara.transform.GetChild(0).gameObject.GetComponent<Camera>();
		camara.cullingMask = ~(1 << 5); //Desactiva la capa UI
	}

	//Cuando acabamos el diálogo, activamos todas las capas de la cámara principal
	public void fromDialogMode()
	{
		Camera camara = Camera.main.transform.GetChild(0).gameObject.GetComponent<Camera>();
		camara.cullingMask = -1; //Activa todas las capas

		camara = camara.transform.GetChild(0).gameObject.GetComponent<Camera>();
		camara.cullingMask = ~(1 << 5); //Desactiva la capa UI
	}

	//Establecemos el modo de visión objeto, cuando estamos manipulando el objeto de alguna acción
	public void setObjectMode()
	{
		Camera.main.cullingMask = 1 << 8; //UIObjeto
		Camera.main.clearFlags = CameraClearFlags.SolidColor;
		Camera.main.backgroundColor = new Color32(73, 67, 67, 0);
	}

	//Calcula la posición de la cámara durante el diálogo según el que está hablando actualmente y
	//lo que debemos mover la cámara respecto a la dirección de este
	public void PosicionDialogo(PosicionCamara posC, GameObject inter)
	{
		//La cámara mira al jugador
		if (posC.lookAt == -1)
		{
			//- 1.769/2 (mitad cuerpo) temporal
			Vector3 translado = TargetLookAt.TransformDirection(new Vector3(posC.coordX, posC.coordY - 1.769f/2, posC.coordZ));

			Vector3 pos = TargetLookAt.position + translado;

			//Asignamos la posicion actual con la posicion suavizada
			transform.position = pos;

			//Asignamos el lookAt
			//- 1.769/2 (mitad cuerpo) temporal
			transform.LookAt(TargetLookAt.position - new Vector3(0f,1.769f/2,0f));
		}
		//La cámara mira al Interactuable
		else
		{
			Vector3 translado = inter.transform.TransformDirection(new Vector3(posC.coordX, posC.coordY, posC.coordZ));

			Vector3 pos = inter.transform.position + translado;

			transform.position = pos;

			transform.LookAt(inter.transform);
		}
	}
}