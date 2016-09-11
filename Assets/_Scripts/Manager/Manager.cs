﻿using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text;

/*
 * 	Clase que se encarga de controlar diversos aspectos del juego
 */
public class Manager : MonoBehaviour {

	//Singleton pattern
	public static Manager Instance { get; private set; }

	//Indica el nombre de primera escena que se cargará
	//Contiene [SerializeField] para mostrar una variable privada en el editor de Unity
	[SerializeField]
	private string escenaInicial;

	private ManagerTiempo managerTiempo; //Controla el tiempo cronológico del juego

	private Dictionary<int, GameObject> interactuables; //grupos de npcs cargados en la escena actual (id_interactuable, gameobject)
	private List<GameObject> interactuablesCercanos; //lista con los interactuables cercanos al jugador
	private List<NavMeshAgent> navMeshAgentRutasActivas; //lista con los navmesh agent con rutas activas

	private Dictionary<int, List<GameObject>> transportes; //diccionario con listas de los transportes de la escena (num_escena, lista con gameobject)

	private List<ColaObjeto> ColaObjetos; //lista con los objetos por serializar

	private List<Grupo> GruposActivos; //lista grupos activos
	private List<int> GruposAcabados; //lista con ids de los grupos acabados

	private List<ObjetoReciente> objetosRecientes; //lista de objetos obtenidos recientemente

	public GameObject canvasGlobal; //referencia al canvas global del juego

	//Estados del Manager
	public enum EstadoJuego
	{
		Pausa, Activo
	}

	public EstadoJuego State {get; set; }
	public EstadoJuego PrevState {get; set; }

	public void SetState(EstadoJuego newState) {
		PrevState = State;
		State = newState;
	}

	private int escenaTransporte; //Variable usada para saber en que escena se encuentran los teletransportes (ya que el orden de ejecución de onloadlevel no es fiable)

	private string nombreJugador;

	//Lista de rutas
	public static string rutaDatosInteractuable;
	public static string rutaDatosInteractuableGuardados;
	public static string rutaTiempo;
	public static string rutaRutinas;
	public static string rutaAutoRutinas;
	public static string rutaAutoRutinasGuardadas;
	public static string rutaEventos;
	public static string rutaEventosGuardados;
	public static string rutaDatosAccion;
	public static string rutaDatosAccionGuardados;
	public static string rutaNPCDatos;
	public static string rutaNPCDatosGuardados;
	public static string rutaNPCDialogos;
	public static string rutaNPCDialogosGuardados;
	public static string rutaIntros;
	public static string rutaTemaMensajes;
	public static string rutaMensajes;
	public static string rutaGrupos;
	public static string rutaGruposModificados;
	public static string rutaGruposActivos;
	public static string rutaGruposAcabados;
	public static string rutaLanzadores;
	public static string rutaInventario;
	public static string rutaObjetoInventario;
	public static string rutaInventarioTienda;
	public static string rutaTransportes;

	public static string rutaDialogoVacio;

	void Awake()
	{
		// First we check if there are any other instances conflicting
		if(Instance != null && Instance != this)
		{
			// If that is the case, we destroy other instances
			Destroy(gameObject);
		}

		//Singleton pattern
		Instance = this;

		DontDestroyOnLoad(gameObject); //Hacemos que el objeto no pueda ser destruido entre escenas

		SetState(EstadoJuego.Activo); //Estado Inicial

		Cursor.visible = false; //Oculta el cursor del ratón

		//Cargamos los gameObject estáticos
		CargarGameObjectsEstaticos();

		//Cargamos las rutas
		rutaDatosInteractuable = Application.dataPath + "/StreamingAssets/DatosInteractuable/";
		rutaDatosInteractuableGuardados = Application.persistentDataPath + "/DatosInteractuable/";
		rutaTiempo = Application.persistentDataPath + "/Tiempo/";
		rutaRutinas = Application.dataPath + "/StreamingAssets/Rutinas/";
		rutaAutoRutinas = Application.dataPath + "/StreamingAssets/Rutinas/AutoRutinas/";
		rutaAutoRutinasGuardadas = Application.persistentDataPath + "/AutoRutinas/";
		rutaEventos = Application.dataPath + "/StreamingAssets/Eventos/";
		rutaEventosGuardados = Application.persistentDataPath + "/Eventos/";
		rutaDatosAccion = Application.dataPath + "/StreamingAssets/DatosAccion/";
		rutaDatosAccionGuardados = Application.persistentDataPath + "/DatosAccion/";
		rutaNPCDatos = Application.dataPath + "/StreamingAssets/NPCDatos/";
		rutaNPCDatosGuardados = Application.persistentDataPath + "/NPC_Datos_Saves/";
		rutaNPCDialogos = Application.dataPath + "/StreamingAssets/NPCDialogue/";
		rutaNPCDialogosGuardados = Application.persistentDataPath + "/NPC_Dialogo_Saves/";
		rutaIntros = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLIntros/";
		rutaTemaMensajes = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLMensajes/XMLTemaMensajes/";
		rutaMensajes = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLMensajes/";
		rutaGrupos = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLGrupos/";
		rutaGruposModificados = Application.persistentDataPath + "/Grupos_Modificados/";
		rutaGruposActivos = Application.persistentDataPath + "/Grupos_Activos/";
		rutaGruposAcabados = Application.persistentDataPath + "/Grupos_Acabados/";
		rutaLanzadores = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLGrupos/Lanzador/";
		rutaInventario = Application.persistentDataPath + "/Inventario/";
		rutaObjetoInventario = Application.dataPath + "/StreamingAssets/ObjetoInventario/";
		rutaInventarioTienda = Application.dataPath + "/StreamingAssets/Tiendas/";
		rutaTransportes = Application.dataPath + "/StreamingAssets/Transportes/";

		rutaDialogoVacio = Application.dataPath + "/StreamingAssets/NPCDialogue/-1.xml";

		escenaTransporte = -1;
		nombreJugador = "Jugador"; //Nombre por defecto del jugador

		//Inicializa algunas variables
		managerTiempo = new ManagerTiempo();
		interactuables = new Dictionary<int,GameObject>();
		interactuablesCercanos = new List<GameObject>();
		navMeshAgentRutasActivas = new List<NavMeshAgent>();
		transportes = new Dictionary<int, List<GameObject>>();
		ColaObjetos = new List<ColaObjeto>();
		GruposActivos = new List<Grupo>();
		GruposAcabados = new List<int>();
		objetosRecientes = new List<ObjetoReciente>();

		//Comprobamos si los directorios necesarios existen y cargamos algunos ficheros
		ComprobarArchivosDirectorios();

		//Carga la lista de datos interactuables usados por las rutinas
		cargarDatosInteractuable();

		comprobarEventosInicio(getHoraActual());

		//Empieza a calcular el tiempo hasta la siguiente sección de las noticias
		StartCoroutine(SiguienteSeccion());

		//Cargamos el escenario
		SceneManager.LoadScene(escenaInicial);
	}

	private void CargarGameObjectsEstaticos()
	{
		canvasGlobal = (GameObject)Instantiate(Resources.Load("CanvasPrefab"));
		DontDestroyOnLoad(canvasGlobal); //Hacemos que el objeto no pueda ser destruido entre escenas

		GameObject Obj = (GameObject)Instantiate(Resources.Load("Text Box Manager"));
		DontDestroyOnLoad(Obj); //Hacemos que el objeto no pueda ser destruido entre escenas

		Obj = (GameObject)Instantiate(Resources.Load("EventSystem"));
		DontDestroyOnLoad(Obj); //Hacemos que el objeto no pueda ser destruido entre escenas

		Obj = new GameObject("ManagerRutinas");
		Obj.transform.SetParent(gameObject.transform, false);
		Obj.AddComponent<ManagerRutinas>();

		Obj = (GameObject)Instantiate(Resources.Load("Ethan"));
		DontDestroyOnLoad(Obj); //Hacemos que el objeto no pueda ser destruido entre escenas
		Obj.transform.position = new Vector3(7.87f, 15.809f, -9.88f);
	}

	//Crea algunos directorios al inicio del juego si no están creados, así como algunos ficheros
	private void ComprobarArchivosDirectorios()
	{
		if (!System.IO.Directory.Exists(rutaDatosInteractuableGuardados))
		{
			System.IO.Directory.CreateDirectory(rutaDatosInteractuableGuardados);
		}

		if (!System.IO.Directory.Exists(rutaTiempo))
		{
			System.IO.Directory.CreateDirectory(rutaTiempo);
		}
		else if(System.IO.File.Exists(rutaTiempo + "Tiempo.xml"))
		{
			CargarManagerTiempo();
		}

		if (!System.IO.Directory.Exists(rutaAutoRutinasGuardadas))
		{
			System.IO.Directory.CreateDirectory(rutaAutoRutinasGuardadas);
		}

		if (!System.IO.Directory.Exists(rutaEventosGuardados))
		{
			System.IO.Directory.CreateDirectory(rutaEventosGuardados);
		}

		if (!System.IO.Directory.Exists(rutaDatosAccionGuardados))
		{
			System.IO.Directory.CreateDirectory(rutaDatosAccionGuardados);
		}

		if (!System.IO.Directory.Exists(rutaNPCDatosGuardados))
		{
			System.IO.Directory.CreateDirectory(rutaNPCDatosGuardados);
		}

		//Creamos el directorio donde guardaremos los dialogos de los NPCs si no existe ya
		if (!System.IO.Directory.Exists(rutaNPCDialogosGuardados))
		{
			System.IO.Directory.CreateDirectory(rutaNPCDialogosGuardados);
		}

		if (!System.IO.Directory.Exists(rutaGruposModificados))
		{
			System.IO.Directory.CreateDirectory(rutaGruposModificados);
		}

		// Comprobamos si existe el directorio donde se guardan los grupos activos
		if(!System.IO.Directory.Exists(rutaGruposActivos))
		{    
			//if it doesn't, create it
			System.IO.Directory.CreateDirectory(rutaGruposActivos);
		}
		//Si ya existe, comprobamos si existe el fichero de gruposactivos
		else if(System.IO.File.Exists(rutaGruposActivos + "GruposActivos.xml"))
		{
			CargarGruposActivos();
		}

		if(!System.IO.Directory.Exists(rutaGruposAcabados))
		{    
			//if it doesn't, create it
			System.IO.Directory.CreateDirectory(rutaGruposAcabados);
		}
		//Si ya existe, comprobamos si existe el fichero de gruposactivos
		else if(System.IO.File.Exists(rutaGruposAcabados + "GruposAcabados.xml"))
		{
			CargarGruposAcabados();
		}

		if(!System.IO.Directory.Exists(rutaInventario))
		{    
			//if it doesn't, create it
			System.IO.Directory.CreateDirectory(rutaInventario);
		}
	}

	public string DevuelveNombreJugador()
	{
		return nombreJugador;
	}

	/*
	 * 
	 * 
	 *  RUTINAS Y TIEMPO
	 * 
	 * 
	 */

	//Carga los datos del managerTiempo de un fichero
	private void CargarManagerTiempo()
	{
		managerTiempo = ManagerTiempo.LoadManagerTiempo();
	}

	//Guarda datos del managerTiempo en un fichero
	private void GuardarTiempo()
	{
		managerTiempo.Serialize();
	}
		
	public int getHoraActual()
	{
		return managerTiempo.getHora();
	}

	//Carga los datos de interactuables situados en un directorio y los añade a la rutina
	private void cargarDatosInteractuable()
	{
		Datos_Interactuable dInter;

		var info = new DirectoryInfo(rutaDatosInteractuable);
		var fileInfo = info.GetFiles().ToArray();

		for(var j = 0; j < fileInfo.Length; j++)
		{
			if(Path.GetExtension(fileInfo[j].Name) == ".xml")
			{
				dInter = null;

				if (System.IO.File.Exists(rutaDatosInteractuableGuardados + fileInfo[j].Name))
				{
					dInter = DeserializeData<Datos_Interactuable>(rutaDatosInteractuableGuardados + fileInfo[j].Name);
				}
				else if(System.IO.File.Exists(rutaDatosInteractuable + fileInfo[j].Name))
				{
					dInter = DeserializeData<Datos_Interactuable>(rutaDatosInteractuable + fileInfo[j].Name);
				}

				//Si el archivo DatosInteractuable con el ID existe, lo cargamos en el Manager
				if(dInter != null)
					ManagerRutinas.Instance.cargarInteractuable(dInter);
			}
		}
	}

	private void comprobarEventosInicio(int hora_actual)
	{
		ManagerRutinas.Instance.comprobarEventosInicio(hora_actual);
	}

	//Carga los interactuables al cargar una escena
	void OnLevelWasLoaded(int level)
	{
		//Guardamos los datos del tiempo
		GuardarTiempo();

		//Cargamos los interactuables de la escena
		ManagerRutinas.Instance.CargarEscena(level);
	}

	//Crea un interactuable en la escena con las coordenadas y rotación especificadas
	public void crearInteractuable(int IDInter, int tipo, Vector3 coord, Quaternion rot)
	{
		GameObject interactuable;

		switch(tipo)
		{
		default:
		case 0: //Tipo NPC
			interactuable = (GameObject)Instantiate(Resources.Load("InteractuableNPC"));
			InteractuableNPC iNPC = interactuable.gameObject.GetComponent<InteractuableNPC>();
			iNPC.ID = IDInter;
			break;
		case 1: //Tipo Objeto
			interactuable = (GameObject)Instantiate(Resources.Load("InteractuableObjeto"));
			InteractuableObjeto iObj = interactuable.gameObject.GetComponent<InteractuableObjeto>();
			iObj.ID = IDInter;
			break;
		}

		interactuable.transform.position = coord;
		interactuable.transform.rotation = rot;
	}

	//Crea un interactuable en la escena en un transporte que conecta con la escena anterior
	public void moverInteractuableDesdeOtraEscena(int IDInter, int tipo, int IDEscenaAnterior, Vector3 coord, Quaternion rot)
	{
		GameObject interactuable;

		switch(tipo)
		{
		default:
		case 0: //Tipo NPC
			interactuable = (GameObject)Instantiate(Resources.Load("InteractuableNPC"));
			InteractuableNPC iNPC = interactuable.gameObject.GetComponent<InteractuableNPC>();
			iNPC.ID = IDInter;

			//Si el interactuable es de tipo NPC, lo creamos en un transporte
			GameObject transporte = EncontrarTransporteEscena(IDEscenaAnterior);

			if(transporte != null)
			{
				interactuable.transform.position = transporte.transform.position;
				iNPC.setRuta(coord);

				GameObject rutaColliderGO = new GameObject("RutaCollider");
				rutaColliderGO.transform.position = coord;
				BoxCollider collider = rutaColliderGO.AddComponent<BoxCollider>();
				collider.size =  new Vector3(3.7f, 6.68f, 1f);
				collider.isTrigger = true;

				RutaCollider rc = rutaColliderGO.AddComponent<RutaCollider>();
				rc.setIDInteractuable(IDInter);

				if(transporte.GetComponent<TransporteInter>().comprobarSiEsTransporteObjeto())
				{
					transporte.transform.parent.GetComponent<InteractuableObjeto>().setNavObstacle(false);

					GameObject saliendoColliderGO = new GameObject("TransporteCollider");
					saliendoColliderGO.transform.SetParent(transporte.transform, false);
					BoxCollider colliderSaliendo = saliendoColliderGO.AddComponent<BoxCollider>();
					colliderSaliendo.size =  new Vector3(3.7f, 6.68f, 1f);
					colliderSaliendo.isTrigger = true;

					SaliendoTransporteCollider saliendoCollider = saliendoColliderGO.AddComponent<SaliendoTransporteCollider>();

					saliendoCollider.setIDInteractuable(IDInter);
					saliendoCollider.setTransporte(transporte);
				}
			}
			else
			{
				interactuable.transform.position = coord;
				interactuable.transform.rotation = rot;
			}
			break;
		case 1: //Tipo Objeto
			//Si el interatuable es de tipo objeto, lo creamos directamente
			interactuable = (GameObject)Instantiate(Resources.Load("InteractuableObjeto"));
			InteractuableObjeto iObj = interactuable.gameObject.GetComponent<InteractuableObjeto>();
			iObj.ID = IDInter;

			interactuable.transform.position = coord;
			interactuable.transform.rotation = rot;
			break;
		}
	}

	public void moverInteractuableEnEscena(int tipo, int IDInter, Vector3 coord, Quaternion rot)
	{
		GameObject Inter = GetInteractuable(IDInter);

		//Si el interactuable no es un NPC, lo movemos directamente
		if(tipo != 0)
		{
			Inter.transform.position = coord;
			Inter.transform.rotation = rot;
		}
		//Si es un NPC, establecemos la ruta que debe seguir, si el NPC no se encuentra ya en exactamente esa posición
		else if(Inter.transform.position != coord)
		{
			InteractuableNPC intNPC = Inter.GetComponent<InteractuableNPC>();
			intNPC.setRuta(coord);
		}
	}

	public void moverInteractuableHaciaOtraEscena(int tipo, int IDInter, int IDEscena)
	{
		GameObject Inter = GetInteractuable(IDInter);

		if(tipo != 0)
		{
			Destroy(Inter);
		}
		//Si es de tipo NPC, buscamos un transporte al que movernos
		else
		{
			GameObject transporteMasCercano = encontrarTransporteInteractuable(Inter, IDEscena);

			if(transporteMasCercano != null)
			{
				Inter.GetComponent<InteractuableNPC>().setRuta(transporteMasCercano.transform.position);

				GameObject transporteColliderGO = new GameObject("TransporteCollider");
				transporteColliderGO.transform.SetParent(transporteMasCercano.transform, false);
				BoxCollider collider = transporteColliderGO.AddComponent<BoxCollider>();
				collider.size =  new Vector3(3.7f, 6.68f, 1f);
				collider.isTrigger = true;

				TransporteCollider tc = transporteColliderGO.AddComponent<TransporteCollider>();
				tc.setIDInteractuable(IDInter);

				if(transporteMasCercano.GetComponent<TransporteInter>().comprobarSiEsTransporteObjeto())
				{
					transporteMasCercano.transform.parent.GetComponent<InteractuableObjeto>().setNavObstacle(false);
					tc.setTransporte(transporteMasCercano);
				}
			}
			//si no existe un transporte que nos lleve a la escena que queremos, destruimos el interactuable
			else
			{
				Destroy(Inter);
			}
		}
	}

	//Pasa a la siguiente sección de las rutinas
	public IEnumerator SiguienteSeccion()
	{
		while(true)
		{
			yield return new WaitForSeconds (1f);

			switch(State)
			{
			case EstadoJuego.Activo:
				managerTiempo.avanzaMinutos();

				//Si pasa una hora
				if(managerTiempo.continuaHora())
				{
					//Aumentamos la hora
					managerTiempo.avanzaHora();

					//Comprobamos que rutinas avanzamos
					ComprobarRutinas();
				}
				break;
			case EstadoJuego.Pausa:
				break;
			}
		}
	}

	//Comprueba las rutinas en el ManagerRutinas
	private void ComprobarRutinas()
	{
		ManagerRutinas.Instance.ComprobarRutinas(managerTiempo.getHora());
	}

	public void cambiarRutina(int IDRutina)
	{
		ManagerRutinas.Instance.cargarRutina(IDRutina, false, false);
	}

	//MIRAR SI SE PUEDE ESTANDARIZAR, AÑADIR COSAS QUE SE LLAMAN AL USAR ESTA FUNCIÓN
	//Establece el estado de pausa
	public void setPausa(bool pausa)
	{
		if(pausa)
			SetState(EstadoJuego.Pausa);
		else
			SetState(EstadoJuego.Activo);
	}

	/*
	 * 
	 * 
	 *  INTERACTUABLES
	 * 
	 * 
	 */

	public void AddToInteractuables(int id, GameObject gobj)
	{
		interactuables[id] = gobj;
	}

	public void RemoveFromInteractuables(int id)
	{
		interactuables.Remove(id);
	}

	//Devuelve el GameObject con el id especificado, sino existe, el gameobject devuelto es null
	public GameObject GetInteractuable(int id)
	{
		GameObject npc;

		interactuables.TryGetValue(id,out npc);

		return npc;
	}

	//Devuelve una lista compuesta por los elementos del diccionario interactuables
	public List<GameObject> GetAllInteractuables()
	{
		return interactuables.Select(d => d.Value).ToList();
	}

	//Recarga las acciones de los interactuables de la lista

	//CREAR OTRA FUNCIÓN DE ACTUALIZAR ACCIONES DONDE NO INTERVENGA EL INVENTARIO, PARA CUANDO SE ACTUALICEN LAS VARIABLES DE UN OBJETO

	public void actualizarAcciones(Inventario inventario)
	{
		foreach(var entry in interactuables.Values)
		{
			// do something with entry.Value or entry.Key
			Interactuable inter = entry.GetComponent<Interactuable>() as Interactuable;
			inter.RecargarAcciones(inventario);
		}
	}

	/*
	 * 
	 * 
	 *  INTERACTUABLES CERCANOS
	 * 
	 * 
	 */

	public int devuelveNumeroInteractuablesCercanos()
	{
		return interactuablesCercanos.Count;
	}

	public GameObject devuelveInteractuableCercano(int num)
	{
		return interactuablesCercanos[num];
	}

	public void addInteractuableCercano(GameObject gObj)
	{
		interactuablesCercanos.Add(gObj);
	}

	public void deleteInteractuableCercano(GameObject gObj)
	{
		interactuablesCercanos.Remove(gObj);
	}

	/*
	 * 
	 * 
	 *  NAVMESH AGENT CON RUTAS ACTIVAS
	 * 
	 * 
	 */

	public void addNavMeshAgent(NavMeshAgent nav)
	{
		if(!navMeshAgentRutasActivas.Contains(nav))
			navMeshAgentRutasActivas.Add(nav);
	}

	public void deleteNavhMeshAgent(NavMeshAgent nav)
	{
		navMeshAgentRutasActivas.Remove(nav);
	}

	//Pausa las navMesh de la lista de navMesh con rutas activas
	public void stopNavMeshAgents()
	{
		for(int i = 0; i < navMeshAgentRutasActivas.Count; i++)
		{
			navMeshAgentRutasActivas[i].velocity = Vector3.zero; //Detiene el navmesh totalmente, sin desaceleración
			navMeshAgentRutasActivas[i].Stop();
		}
	}

	//Reanuda las navMesh de la lista de navMesh con rutas activas
	public void resumeNavMeshAgents()
	{
		for(int i = 0; i < navMeshAgentRutasActivas.Count; i++)
		{
			navMeshAgentRutasActivas[i].Resume();
		}
	}

	/*
	 * 
	 * 
	 *  TRANSPORTES
	 * 
	 * 
	 */

	//Añadimos el transporte al diccionario de transportes
	public void anyadirTransporte(int numEscenaTransporte, GameObject transporte, List<int> escenas)
	{
		//Si la escena del transporte no se corresponde a la escena del Manager, significa que hemos cambiado
		//de escena, vaciamos el diccionario de transportes
		if(escenaTransporte != numEscenaTransporte)
		{
			transportes.Clear();
			escenaTransporte = numEscenaTransporte;
		}

		//Añadimos el número de las escenas conectadas al transporte en el diccionario
		for(int i = 0; i < escenas.Count; i++)
		{
			List<GameObject> listaTransportes;

			if (!transportes.TryGetValue(escenas[i], out listaTransportes))
			{
				listaTransportes = new List<GameObject>();
				transportes.Add(escenas[i], listaTransportes);
			}
	
			listaTransportes.Add(transporte);
		}
	}

	//Busca el transporte más cercano del interactuable especificado hacia la IDEscena especificada
	//Devuelve un GameObject null si no ha encontrado ninguno
	private GameObject encontrarTransporteInteractuable(GameObject interactuable, int IDEscena)
	{
		List<GameObject> listaTransportes;
		GameObject transporteMasCercano = null;

		if (transportes.TryGetValue(IDEscena, out listaTransportes))
		{
			float distanciaMasCercana = Mathf.Infinity;

			for(int i = 0; i < listaTransportes.Count; i++)
			{
				float distancia = Vector3.Distance(interactuable.transform.position, listaTransportes[i].transform.position);
				if (distancia < distanciaMasCercana)
				{
					distanciaMasCercana = distancia;
					transporteMasCercano = listaTransportes[i];
				}
			}
		}

		return transporteMasCercano;
	}
		
	//Devuelve la distancia entre una recta con un punto
	private float DistanceToLine(Ray ray, Vector3 point)
	{
		return Vector3.Cross(ray.direction, point - ray.origin).sqrMagnitude;
	}

	//Busca el transporte que conecte con la escena indicada
	//Devuelve un GameObject null si no ha encontrado ninguno
	private GameObject EncontrarTransporteEscena(int IDEscena)
	{
		List<GameObject> listaTransportes;
		GameObject transporteMasCercano = null;

		if (transportes.TryGetValue(IDEscena, out listaTransportes))
		{
			System.Random rnd = new System.Random();

			//Cogemos un transporte aleatorio de la lista
			transporteMasCercano = listaTransportes[rnd.Next(listaTransportes.Count-1)];
		}

		return transporteMasCercano;
	}

	public GameObject EncontrarTransporte(int IDTransporte)
	{
		GameObject transporte = null;

		foreach(var entry in transportes.Values)
		{
			for(int i = 0; i < entry.Count; i++)
			{
				TransporteInter trans = entry[i].GetComponent<TransporteInter>();

				if(trans.ID == IDTransporte)
				{
					transporte = entry[i];
				}
			}
		}

		return transporte;
	}

	/*
	 * 
	 * 
	 *  GRUPOS
	 * 
	 * 
	 */

	public void AddToGruposActivos(Grupo g)
	{
		GruposActivos.Add(g);
	}

	public bool GrupoActivoExiste(int id)
	{
		return GruposActivos.Any(x => x.IDGrupo == id);
	}
		
	public bool GrupoAcabadoExiste(int id)
	{
		return GruposAcabados.IndexOf(id) != -1;
	}

	//Devuelve un grupo activo con el id especificado, null si no existe en la lista
	public Grupo DevolverGrupoActivo(int id)
	{
		return GruposActivos.Find (x => x.DevolverIDGrupo () == id);
	}

	//Elimina el grupo indicado de la lista de grupos activos
	//y lo añade a la de grupos acabados, aunque no estuviera en los grupos activos
	public void RemoveFromGruposActivos(int id)
	{
		//Buscamos el grupo en la lista de grupos activos
		Grupo g = DevolverGrupoActivo(id);

		if (g != null)
		{
			GruposActivos.Remove (g); //lo borramos de la lista de grupos activos
		}

		//El grupo se añade a la lista de grupos acabados, estuviera o no en
		//la lista de grupos activos
		GruposAcabados.Add (id); //Añadimos la id del grupo acabado

		//Buscamos si el grupo estaba en la lista de grupos modificados
		//y lo borramos de ahí también
		if(System.IO.File.Exists(rutaGruposModificados + id.ToString () + ".xml"))
		{
			System.IO.File.Delete(rutaGruposModificados + id.ToString () + ".xml");
		}
	}

	//Suma a una variable situada en la posicion num de la lista de variables de un grupo activo el valor
	public void AddVariablesGrupo(int id, int num, int valor)
	{
		int indice = GruposActivos.FindIndex(x => x.IDGrupo == id);
		GruposActivos[indice].variables[num] += valor;
	}

	//Establece a una variable situada en la posicion num de la lista de variables de un grupo activo el valor
	public void SetVariablesGrupo(int id, int num, int valor)
	{
		int indice = GruposActivos.FindIndex(x => x.IDGrupo == id);
		GruposActivos[indice].variables[num] = valor;
	}

	private void CargarGruposActivos()
	{
		GruposActivos = DeserializeData<List<Grupo>>(rutaGruposActivos + "GruposActivos.xml");
	}

	private void CargarGruposAcabados()
	{
		GruposAcabados = DeserializeData<List<int>>(rutaGruposAcabados + "GruposAcabados.xml");
	}

	public void GuardarGruposActivos()
	{
		//Antes de serializar los grupos activos, comprueba si
		//entre ellos hay algún grupo modificado para eliminar su fichero
		ComprobarGruposModificados();
		SerializeData(GruposActivos, rutaGruposActivos, "GruposActivos.xml");
	}

	//Borra los ficheros de grupos modificados que ahora son grupos activos
	public void ComprobarGruposModificados()
	{
		for(int i = 0; i < GruposActivos.Count; i++)
		{
			int ID = GruposActivos[i].DevolverIDGrupo();

			if(System.IO.File.Exists(rutaGruposModificados + ID.ToString () + ".xml"))
			{
				System.IO.File.Delete(rutaGruposModificados + ID.ToString () + ".xml");
			}
		}
	}

	public void GuardarGruposAcabados()
	{
		SerializeData(GruposAcabados, rutaGruposAcabados, "GruposAcabados.xml");
	}

	/*
	 * 
	 * 
	 *  OBJETOS RECIENTES
	 * 
	 * 
	 */

	public int devuelveNumeroObjetosRecientes()
	{
		return objetosRecientes.Count;
	}

	//Devuelve un objeto reciente situado en la posición num de la lista de objetosRecientes
	public string devuelveNombreObjetoReciente(int num)
	{
		return objetosRecientes[num].devuelveObjeto().nombre;
	}

	public void addObjetoReciente(ObjetoInventario obj, int cantidad)
	{
		objetosRecientes.Add(new ObjetoReciente(obj, cantidad));
	}

	public void vaciarObjetosRecientes()
	{
		objetosRecientes.Clear();
	}

	/*
	 * 
	 * 
	 *  COLAOBJETOS
	 * 
	 * 
	 */

	public void AddToColaObjetos(string path, ObjetoSerializable obj)
	{
		//Comprobamos si ya existe el objeto indicado
		//Si ya existe, lo eliminamos
		if (ColaObjetoExiste(path))
			RemoveFromColaObjetos(path);

		ColaObjeto item = new ColaObjeto(obj, path);
		ColaObjetos.Add(item);
	}

	public bool ColaObjetoExiste(string path)
	{
		return ColaObjetos.Any(x => x.GetRuta() == path);
	}

	public ColaObjeto GetColaObjetos(string path)
	{
		return ColaObjetos.Find (x => x.GetRuta() == path);
	}

	//Devuelve una lista con objetosSerializables solo del tipo pasado
	public List<ObjetoSerializable> GetColaObjetosTipo(Type tip)
	{
		List<ObjetoSerializable> listaObjetos = new List<ObjetoSerializable>();

		for(int i = 0; i < ColaObjetos.Count; i++)
		{
			if(ColaObjetos[i].GetObjeto().GetType() == tip)
			{
				listaObjetos.Add(ColaObjetos[i].GetObjeto());
			}
		}

		return listaObjetos;
	}

	public void RemoveFromColaObjetos(string path)
	{
		ColaObjeto cobj = GetColaObjetos(path);

		if (cobj != null)
		{
			ColaObjetos.Remove(cobj);
		}
	}

	//Guardamos los objetos de la cola y la vaciamos
	public void SerializarCola()
	{
		for(var i = 0; i < ColaObjetos.Count; i++)
		{
			string ruta = ColaObjetos[i].GetRuta();
			ObjetoSerializable obj = ColaObjetos[i].GetObjeto();

			SerializeData(obj, Path.GetDirectoryName(ruta), Path.GetFileName(ruta));

			//Si el inventario se ha actualizado, actualizamos el estado
			//de las acciones de los interactuables
			if(ruta == rutaInventario + "Inventario.xml")
			{
				Inventario inventario = obj as Inventario;
				actualizarAcciones(inventario);
			}
		}

		ColaObjetos.Clear();
	}

	//Guarda los datos de la partida
	public void ActualizarDatos()
	{
		GuardarGruposActivos();
		GuardarGruposAcabados();
		GuardarTiempo();
		SerializarCola();
	}
		
	/*
	 * 
	 * 
	 *  SERIALIZACION Y DESERIALIZACION
	 * 
	 * 
	 */

	public T DeserializeData<T>(string rutaArchivo)
	{
		XmlSerializer deserz = new XmlSerializer(typeof(T));
		StreamReader reader = new StreamReader(rutaArchivo);

		T pObject = (T)deserz.Deserialize(reader);

		reader.Close();

		return pObject;
	}

	//NombreDirectorio acaba en /
	public void SerializeData<T>(T pObject, string nombreDirectorio, string nombreArchivo)
	{
		string _data = SerializeObject(pObject);
		// This is the final resulting XML from the serialization process
		CreateXML(_data, nombreDirectorio, nombreArchivo);
	}

	private string SerializeObject<T>(T pObject) 
	{ 
		string XmlizedString = null; 
		MemoryStream memoryStream = new MemoryStream(); 
		XmlSerializer xs = new XmlSerializer(typeof(T));

		XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8); 
		xs.Serialize(xmlTextWriter, pObject); 
		memoryStream = (MemoryStream)xmlTextWriter.BaseStream; 
		XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
		return XmlizedString; 
	}

	private string UTF8ByteArrayToString(byte[] characters) 
	{      
		UTF8Encoding encoding = new UTF8Encoding(); 
		string constructedString = encoding.GetString(characters); 
		return (constructedString); 
	} 

	private void CreateXML(string _data, string nombreDirectorio, string nombreArchivo) 
	{
		StreamWriter writer; 
		FileInfo t;

		if(!Directory.Exists(nombreDirectorio))
		{    
			//if it doesn't, create it
			Directory.CreateDirectory(nombreDirectorio);
		}

		t = new FileInfo(nombreDirectorio + "/" + nombreArchivo);
			

		if(!t.Exists) 
		{ 
			writer = t.CreateText(); 
		} 
		else 
		{ 
			t.Delete(); 
			writer = t.CreateText(); 
		} 
		writer.Write(_data); 
		writer.Close(); 

		Debug.Log("File written."); 
	}
}