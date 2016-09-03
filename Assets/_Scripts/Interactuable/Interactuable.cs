using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using System.Linq;

/*
 * 	Clase que almacena datos sobre un interactuable, que almacena objetos los cuales permiten al jugador realizar determinadas acciones
 */
public class Interactuable : MonoBehaviour {

	public int ID; //ID único que identifica al interactuable

	//Parámetros sobre la sensibilidad del ratón (PASAR AL MANAGER)
	public float X_MouseSensitivity = 0.02f;
	public float Y_MouseSensitivity = 0.02f;

	//Dos listas almacenan las acciones que puede ejecutar un interactuable. Los datos de las
	//acciones se guardan separados en la clase DatosAccion debido a la serialización
	private List<DatosAccion> Acciones; //Contiene todas las acciones, incluso las que no se han cargado
	private List<GameObject> AccionesGO; //Los acciones GameObject solo contienen las acciones cargadas

	//-1 = ninguna
	//x = lugar de la accion activa en la lista de DatosAccion
	//Indica que acción está siendo apuntada por el cursor
	private int accionActiva;

	private GameObject canvas; //Canvas propio del interactuable que contiene toda la UI
	private GameObject nombre; //Nombre del interactuable
	private GameObject Objeto; //GameObject que se usa como apariencia del interactuable
	private GameObject cursorUI; //Objeto que representa al cursor
	private Camera camara; //Referencia a la cámara del juego

	private float distance; //Distancia entre el jugador y el interactuable que es usada para aplicar transparencia a la UI según distancia
	private Vector3 moveVector; //vector de movimiento del cursorUI

	private bool cursorSobreAccion; //indica si el cursor está encima de alguna acción

	private RaycastHit hitInfo; //Rayo usado para averiguar si el interactuable es visible por la cámara
	public LayerMask layerMask; //Layermask usada en la colisión del rayo. Se usa la layer 10 (pared)

	//Estados de la clase
	public enum State { Desactivado, Accionable, Seleccionado, Accionando, Accionado }

	State _state = State.Desactivado;
	State _prevState;

	public State CurrentState {
		get { return _state; } 
	}

	public State PrevState {
		get { return _prevState; }
	}

	public void SetState(State newState) {
		_prevState = _state;
		_state = newState;
	}

	protected virtual void Start ()
	{
		//Añadimos el interactuable al diccionario para tenerlo disponible
		Manager.Instance.AddToInteractuables(ID, gameObject);

		//Carga la layerMask para que el rayo detecte todos las colisiones con objetos
		//con la layer 10 (Pared), es decir, las colisiones con el objeto, que contiene esta layer
		layerMask = 1 << 10;

		//Asignamos los objetos correspondientes
		Objeto = gameObject.transform.GetChild(0).gameObject;
		canvas = gameObject.transform.GetChild(1).gameObject;
		nombre = canvas.transform.GetChild(0).gameObject;
		cursorUI = canvas.transform.GetChild(1).gameObject;

		//Buscamos la cámara activa y se la asignamos al canvas
		camara = GameObject.FindWithTag("MainCamera").GetComponent<Camera> ();
		canvas.GetComponent<Canvas>().worldCamera = camara;

		//Asignamos la posicion inicial y el vector de movimientos
		moveVector = new Vector3(0f, 0f, 0f);
		reiniciarDistancia();

		//Asignamos el estado inicial
		SetState(State.Desactivado);

		//Ocultamos el canvas
		OcultaCanvas();

		//Otros valores por defecto
		setAccionActivaNull();

		//Cargamos las listas de acciones
		CargarAcciones();
	}

	//Al destruirse esta instancia, lo borramos de la lista del Manager que almacena los interactuables de la escena actual
	void OnDestroy()
	{
		//Borramos el valor del diccionario cuando el npc no existe
		Manager.Instance.RemoveFromInteractuables(ID);
	}

	//Rellena las listas de acciones
	private void CargarAcciones()
	{
		AccionesGO = new List<GameObject>();

		//Si existe un fichero guardado, cargamos ese fichero, sino cargamos el fichero por defecto
		if (System.IO.File.Exists(Manager.rutaDatosAccionGuardados + ID.ToString()  + ".xml"))
		{
			Acciones = LoadDatosAccion(Manager.rutaDatosAccionGuardados + ID.ToString()  + ".xml");
		}
		else
		{
			Acciones = LoadDatosAccion(Manager.rutaDatosAccion + ID.ToString()  + ".xml");
		}

		//Cargamos el inventario, necesario para comprobar si ciertas acciones se muestran
		Inventario inventario;

		//Cargamos el inventario si existe, sino lo creamos
		if(System.IO.File.Exists(Manager.rutaInventario + "Inventario.xml"))
		{
			inventario = Inventario.LoadInventario(Manager.rutaInventario + "Inventario.xml");
		}
		else
		{
			inventario = new Inventario();
		}

		//Creamos los gameObject que representan las acciones
		for(int i = 0; i < Acciones.Count; i++)
		{
			//Comprobamos si los requisitos para mostrar la accion son correctos. Si lo son, creamos la accion
			if(mostrarAccion(Acciones[i], inventario))
			{
				GameObject AccionGO = new GameObject("Accion" + i.ToString());
				AccionObjeto aobj = AccionGO.AddComponent<AccionObjeto>();

				aobj.Inicializar(ID, i);

				//Si la acción es de tipo Dialogo, realizamos acciones específicas
				if(Acciones[i].GetType() == typeof(DatosAccionDialogo))
				{
					DatosAccionDialogo d = Acciones[i] as DatosAccionDialogo;

					//Si el dialogo es a distancia creamos el box collider
					if(d.aDistancia)
					{
						GameObject dialogoDistancia = new GameObject("Dialogo Distancia");
						dialogoDistancia.transform.position = Objeto.transform.position;
						dialogoDistancia.transform.SetParent(gameObject.transform, true);

						DialogoDistancia dd = dialogoDistancia.AddComponent<DialogoDistancia>();
						dd.cargarDialogo(this, d);

						BoxCollider col = dialogoDistancia.AddComponent<BoxCollider>();
						col.isTrigger = true;
						col.size = new Vector3(d.tamX, d.tamY, d.tamZ);
					}
				}

				//Carga la acción en la lista de acciones GameObjects
				CargaAccionGO(AccionGO, i);
			}

			//Carga el dialogo si la accion es de tipo dialogo aunque no se muestre
			if(Acciones[i].GetType() == typeof(DatosAccionDialogo))
			{
				DatosAccionDialogo d = Acciones[i] as DatosAccionDialogo;
				d.CargaDialogo();
			}
		}

		//Cargamos la UI de las acciones actuales
		CargarAccionesUI();

		//Ocultamos el texto de las acciones
		DesactivarTextoAcciones();
	}

	//Elimina o añade acciones que dependen de los objetos disponibles en el inventario
	public void RecargarAcciones(Inventario inventario)
	{
		for(int i = Acciones.Count - 1; i >= 0; i--)
		{
			//Comprobamos si los requisitos para mostrar la accion
			//son correctos. Si lo son, creamos la accion, solo si no existía anteriormente
			if(mostrarAccion(Acciones[i], inventario))
			{
				//Comprobamos si la acción que queremos añadir ya está actualmente en la lista
				GameObject accionGO =  AccionesGO.Where(x => x.name == "Accion" + i.ToString()).SingleOrDefault();

				//Si la accion no existe, la añadimos
				if(accionGO == null)
				{
					accionGO = new GameObject("Accion" + i.ToString());
					AccionObjeto aobj = accionGO.AddComponent<AccionObjeto>();
					aobj.Inicializar(ID, i);

					CargaAccionGO(accionGO, i);
				}
			}
			//comprobamos si la acción ya existía para eliminarla
			else
			{
				GameObject accionGO =  AccionesGO.Where(x => x.name == "Accion" + i.ToString()).SingleOrDefault();

				//Si la accion existe, la eliminamos
				if(accionGO != null)
				{
					AccionesGO.Remove(accionGO);
					Destroy(accionGO);
				}
			}
		}

		//Cargamos la UI de las acciones actuales
		CargarAccionesUI();

		//Ocultamos el texto de las acciones
		DesactivarTextoAcciones();
	}

	//Comprueba si los requisitos para que la acción se muestre. Si no se cumplen, devuelve falso
	protected virtual bool mostrarAccion(DatosAccion dAcc, Inventario inventario)
	{
		bool mostrarAccion = true;

		for(int i = 0; i < dAcc.objetos.Count; i++)
		{
			//Si equipado es true
			//Si no tenemos el objeto la acción no se muestra
			if(dAcc.objetos[i].equipado)
			{
				if(!inventario.ObjetoInventarioExiste(dAcc.objetos[i].IDObjeto))
				{
					mostrarAccion = false;
				}
			}
			//Si equipado es false
			//Si tenemos el objeto la acción no se muestra
			else
			{
				if(inventario.ObjetoInventarioExiste(dAcc.objetos[i].IDObjeto))
				{
					mostrarAccion = false;
				}
			}
		}

		return mostrarAccion;
	}

	//Establece la posición del gameObject Acción, así como el texto, tamaño, etc.
	private void CargaAccionGO(GameObject AccionGO, int i)
	{
		AccionGO.transform.SetParent(canvas.transform, false);
		AccionGO.transform.localPosition = new Vector3(0f, 0f, 0f);
		AccionGO.layer = 5; //UI
		AccionGO.tag = "AccionUI";

		BoxCollider collider = AccionGO.AddComponent<BoxCollider>();
		collider.size =  new Vector2(430f, 140f);

		Text myText = AccionGO.AddComponent<Text>();
		myText.text = Acciones[i].DevolverNombre();
		myText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		myText.fontSize = 80;
		myText.rectTransform.sizeDelta = new Vector2(430f, 140f);
		myText.material = Resources.Load("UI") as Material;

		AccionesGO.Add(AccionGO);
	}

	//Distribuye las acciones en la lista de acciones GameObject en un circulo en la UI
	//donde la distancia en grados entre cada acción es la misma
	private void CargarAccionesUI()
	{
		float ang = 0;
		float radio = 600;

		for(int i = 0; i < AccionesGO.Count; i++)
		{
			Vector3 vec = new Vector3();

			vec.x = radio*Mathf.Cos(ang);
			vec.y = radio*Mathf.Sin(ang);
			vec.z = 0f;

			GameObject AccionGO = AccionesGO[i];

			AccionGO.transform.localPosition = new Vector3(0f, 0f, 0f);
			AccionGO.transform.localPosition += vec;

			ang += (360/AccionesGO.Count)*Mathf.Deg2Rad;
		}

		cursorUI.transform.SetAsLastSibling(); //Mueve el cursor al final de la jerarquía, mostrándolo encima de los demás GameObjects
	}

	//Devuelve el número de acciones creadas, o lo que es lo mismo, el número de acciones en la lista
	//de accionesGO
	public int DevolverAccionesCreadas()
	{
		return AccionesGO.Count;
	}

	//Devuelve un objeto NPC_Dialogo con la ID pasada (null si no lo ha encontrado)
	public NPC_Dialogo DevolverDialogo(int ID_Dialogo)
	{
		NPC_Dialogo diag = null;

		for(int i = 0; i < Acciones.Count; i++)
		{
			DatosAccion dac = Acciones[i];

			if(dac.GetType() == typeof(DatosAccionDialogo))
			{
				DatosAccionDialogo dacdial = dac as DatosAccionDialogo;
				if(ID_Dialogo == dacdial.DevuelveIDDiag())
				{
					diag = dacdial.DevuelveDialogo();
				}
			}
		}

		return diag;
	}

	//Devuelve una lista con los objetos NPC_Dialogo del interactuable, vacía si no tiene ninguno
	public List<NPC_Dialogo> DevolverDialogos()
	{
		List<NPC_Dialogo> dialogos = new List<NPC_Dialogo>();

		for(int i = 0; i < Acciones.Count; i++)
		{
			DatosAccion dac = Acciones[i];

			if(dac.GetType() == typeof(DatosAccionDialogo))
			{
				DatosAccionDialogo dacdial = dac as DatosAccionDialogo;
				dialogos.Add(dacdial.DevuelveDialogo());
			}
		}

		return dialogos;
	}

	//Devuelve el nombre del interactuable que aparece en el dialogo
	//La función es sustituida por las clases hijas
	//(no es el mismo para la clase derivada interactuableobjeto, sí que lo es para el interactuableNPC)
	public virtual string DevuelveNombreDialogo()
	{
		return "";
	}

	//Establece el nombre del interactuable que se muestra en la UI, no el del dialogo
	//(no es el mismo para la clase derivada interactuableobjeto, sí que lo es para el interactuableNPC)
	public void SetNombre(string n)
	{
		nombre.GetComponent<Text>().text = n;
	}

	//Establece la acción activa y, como el cursor está sobre esta acción, el booleano que lo indica pasa a true
	public void AsignarAccionActiva(int num)
	{
		accionActiva = num;
		setCursorSobreAccion(true);
	}

	//Establece la acción activa a ninguna(-1) y, como el cursor no está sobre ninguna acción, el booleano que lo indica pasa a false
	public void setAccionActivaNull()
	{
		accionActiva = -1;
		setCursorSobreAccion(false);
	}

	private void setCursorSobreAccion(bool estado)
	{
		cursorSobreAccion = estado;
	}
		
	void LateUpdate()
	{
		switch(CurrentState)
		{
		case State.Desactivado: //El interactuable no realiza ninguna acción
			break;
		case State.Accionable: //La UI del interactuable se muestra, pero solo su nombre
			CalcularDistancia();
			ShowCanvas();
			MoverCanvas();
			break;
		case State.Seleccionado://La UI del interactuable cambia, ahora se muestran las acciones asociadas
			MuestraCanvasSinTransparencia();
			MoverCanvas();

			//Si pulsamos click izquierdo, significa que estamos interactuando con la UI
			if (Input.GetMouseButton(0) && TP_Controller.Instance.CurrentState == TP_Controller.State.Normal)
			{
				TP_Controller.Instance.SetState(TP_Controller.State.Interactuables);
				SetState(State.Accionando);
				ChangeCursorUI(Resources.Load<Sprite>("cursor")); //MOVER LA REFERENCIA DEL RESOURCE AL MANAGER PARA NO TENER QUE ESTAR CARGÁNDOLA CONTINUAMENTE
			}
			break;
		case State.Accionando://El cursor se está moviendo
			if (Input.GetMouseButton(0))
			{
				Manager.Instance.stopNavMeshAgents();
				MoviendoCursorUI();
			}
			else if (!cursorSobreAccion)
			{
				Manager.Instance.resumeNavMeshAgents();
				DefaultCursorUI();
				TP_Controller.Instance.SetState(TP_Controller.State.Normal);
				SetState(State.Seleccionado);
			}
			else
			{
				SetState(State.Accionado);
			}
			break;
		case State.Accionado://El cursor ha dejado de moverse sobre una acción, activamos esa acción
			EjecutarAccion();
			break;
		}
	}

	//Calcula la distancia entre el jugador y el interactuable
	private void CalcularDistancia()
	{
		distance = Vector3.Distance(TP_Controller.characterController.transform.position, transform.position);
	}

	private void MuestraCanvasSinTransparencia()
	{
		canvas.GetComponent<CanvasGroup>().alpha = 1;
	}

	public void OcultaCanvas()
	{
		canvas.GetComponent<CanvasGroup>().alpha = 0;
	}

	//Asigna a la distancia su valor inicial
	public void reiniciarDistancia()
	{
		distance = 1;
	}

	//Regula la transparencia del canvas según la distancia
	private void ShowCanvas() {
		//Dist max - distance/2
		float alpha = 3.1f - distance / 2.0f;
		canvas.GetComponent<CanvasGroup>().alpha = alpha;
	}

	//Mueve el canvas según la posición de la cámara, mirando a esta
	private void MoverCanvas()
	{
		canvas.transform.LookAt(canvas.transform.position + camara.transform.rotation * Vector3.forward, camara.transform.rotation * Vector3.up);
	}

	//Cambia la imagen del objeto cursor
	public void ChangeCursorUI(Sprite sprite)
	{
		Image imagen = cursorUI.GetComponent<Image>();
		imagen.sprite = sprite;
	}

	//Controla cuanto se mueve el cursorUI cuando se interactúa con él moviendo el ratón
	private void MoviendoCursorUI()
	{
		//Movemos las coordenadas del raton segun el movimiento
		//Cogemos el eje X del Input del raton multiplicada por la sensibilidad
		moveVector.x += Input.GetAxis ("Mouse X") * X_MouseSensitivity;

		//Cogemos el eje Y del Input del raton multiplicada por la sensibilidad
		moveVector.y += Input.GetAxis ("Mouse Y") * Y_MouseSensitivity;

		//Limitamos el módulo del vector convirtiéndolo en unitario
		//y haciendo que el rango de movimiento esté limitado a un círculo de radio 1
		moveVector = Vector3.ClampMagnitude(moveVector, 1.0f);

		//Transforma los movimientos del ratón según el punto de vista de la cámara
		Vector3 delta = new Vector3(moveVector.x,moveVector.y,0);
		delta = Camera.main.transform.TransformDirection(delta);

		//Asignamos la posición al objeto que hace de cursor
		cursorUI.transform.position = canvas.transform.position + delta;
	}

	//Mueve el cursorUI a su posición inicial
	private void DefaultCursorUI()
	{
		moveVector = new Vector3(0f, 0f, 0f); //Reseteamos el vector de movimiento
		cursorUI.transform.position = canvas.transform.position; //Asignamos la posición inicial al objeto
		ChangeCursorUI(Resources.Load<Sprite>("mouse")); //MOVER LA REFERENCIA DEL RESOURCE AL MANAGER PARA NO TENER QUE ESTAR CARGÁNDOLA CONTINUAMENTE
	}

	//Ejecuta la acción de la lista de acciones indicado en la variable accionActiva
	public void EjecutarAccion()
	{
		DefaultCursorUI();
		SetState(State.Accionable);
		Acciones[accionActiva].EjecutarAccion();
	}

	//Devuelve un booleano indicando si el interactuable es visible desde la cámara (el gameobject llamado Objeto)
	//Comprobando la propiedad isVisible del render y haciendo un rayline hacia la cámara
	public bool isVisible()
	{
		bool visible = false;

		Debug.DrawLine(transform.position, Camera.main.transform.position);

		if(Objeto.GetComponent<Renderer>().isVisible && !Physics.Linecast(transform.position, Camera.main.transform.position, out hitInfo, layerMask))
		{
			visible = true;
		}

		return visible;
	}

	//Muestra el texto de las acciones, el cursorUI y oculta el nombre del interactuable
	public void ActivarTextoAcciones()
	{
		nombre.SetActive(false);
		cursorUI.SetActive(true);
		for(int i = 0; i < AccionesGO.Count; i++)
		{
			AccionesGO[i].SetActive(true);
		}
	}

	//Oculta el texto de las acciones, el cursorUI y muestra el nombre del interactuable
	public void DesactivarTextoAcciones()
	{
		nombre.SetActive(true);
		cursorUI.SetActive(false);
		for(int i = 0; i < AccionesGO.Count; i++)
		{
			AccionesGO[i].SetActive(false);
		}
	}

	//Devuelve la lista de DatosAccion de un xml indicado en la ruta
	public static List<DatosAccion> LoadDatosAccion(string path)
	{
		List<DatosAccion> datosAccion = Manager.Instance.DeserializeData<List<DatosAccion>>(path);

		return datosAccion;
	}

	//Guarda la lista de acciones en un fichero xml
	public void GuardarAcciones()
	{
		Manager.Instance.SerializeData(Acciones, Manager.rutaDatosAccionGuardados, ID.ToString()  + ".xml");
	}
}
