using UnityEngine;
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
	public static Manager instance { get; private set; }

	//Indica el nombre de primera escena que se cargará
	//Contiene [SerializeField] para mostrar una variable privada en el editor de Unity
	[SerializeField]
	private string escenaInicial;

	private Dictionary<int, GameObject> interactuables; //grupos de npcs cargados en la escena actual (id_interactuable, gameobject)
	private List<GameObject> interactuablesCercanos; //lista con los interactuables cercanos al jugador
	private List<NavMeshAgent> navMeshAgentRutasActivas; //lista con los navmesh agent con rutas activas

	private Dictionary<int, List<GameObject>> transportes; //diccionario con listas de los transportes de la escena (num_escena, lista con gameobject)

	private List<ColaObjeto> colaObjetos; //lista con los objetos por serializar

	private List<Grupo> gruposActivos; //lista grupos activos
	private List<int> gruposAcabados; //lista con ids de los grupos acabados

	private List<ObjetoReciente> objetosRecientes; //lista de objetos obtenidos recientemente

	public GameObject canvasGlobal; //referencia al canvas global del juego

	private int escenaTransporte; //Variable usada para saber en que escena se encuentran los teletransportes (ya que el orden de ejecución de onloadlevel no es fiable)

	private string nombreJugador;

	//Lista de rutas
	public static string rutaDatosInteractuable;
	public static string rutaDatosInteractuableGuardados;
	public static string rutaTiempo;
	public static string rutaRutinas;
	public static string rutaAutorutinas;
	public static string rutaAutorutinasGuardadas;
	public static string rutaEventos;
	public static string rutaEventosGuardados;
	public static string rutaDatosAccion;
	public static string rutaDatosAccionGuardados;
	public static string rutaInterDatos;
	public static string rutaInterDatosGuardados;
	public static string rutaInterDialogos;
	public static string rutaInterDialogosGuardados;
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

	private void Awake()
	{
		// First we check if there are any other instances conflicting
		if(instance != null && instance != this)
		{
			// If that is the case, we destroy other instances
			Destroy(gameObject);
		}

		//Singleton pattern
		instance = this;

		DontDestroyOnLoad(gameObject); //Hacemos que el objeto no pueda ser destruido entre escenas

		Cursor.visible = false; //Oculta el cursor del ratón

		SetRutasArchivo();

		//Cargamos los gameObject estáticos
		CargarGameObjectsEstaticos();

		escenaTransporte = -1;
		nombreJugador = "Jugador"; //Nombre por defecto del jugador

		//Inicializa algunas variables
		interactuables = new Dictionary<int,GameObject>();
		interactuablesCercanos = new List<GameObject>();
		navMeshAgentRutasActivas = new List<NavMeshAgent>();
		transportes = new Dictionary<int, List<GameObject>>();
		colaObjetos = new List<ColaObjeto>();
		gruposActivos = new List<Grupo>();
		gruposAcabados = new List<int>();
		objetosRecientes = new List<ObjetoReciente>();

		//Comprobamos si los directorios necesarios existen y cargamos algunos ficheros
		ComprobarArchivosDirectorios();

		//Carga la lista de datos interactuables usados por las rutinas
		CargarDatosInteractuable();

		ComprobarEventosInicio(ManagerTiempo.instance.GetHoraActual());

		//Cargamos el escenario
		SceneManager.LoadScene(escenaInicial);
	}

	private void SetRutasArchivo()
	{
		//Cargamos las rutas
		rutaDatosInteractuable = Application.dataPath + "/StreamingAssets/DatosInteractuable/";
		rutaDatosInteractuableGuardados = Application.persistentDataPath + "/DatosInteractuable/";
		rutaTiempo = Application.persistentDataPath + "/Tiempo/";
		rutaRutinas = Application.dataPath + "/StreamingAssets/Rutinas/";
		rutaAutorutinas = Application.dataPath + "/StreamingAssets/Rutinas/AutoRutinas/";
		rutaAutorutinasGuardadas = Application.persistentDataPath + "/AutoRutinas/";
		rutaEventos = Application.dataPath + "/StreamingAssets/Eventos/";
		rutaEventosGuardados = Application.persistentDataPath + "/Eventos/";
		rutaDatosAccion = Application.dataPath + "/StreamingAssets/DatosAccion/";
		rutaDatosAccionGuardados = Application.persistentDataPath + "/DatosAccion/";
		rutaInterDatos = Application.dataPath + "/StreamingAssets/InterDatos/";
		rutaInterDatosGuardados = Application.persistentDataPath + "/InterDatosSaves/";
		rutaInterDialogos = Application.dataPath + "/StreamingAssets/InterDialogo/";
		rutaInterDialogosGuardados = Application.persistentDataPath + "/InterDialogoSaves/";
		rutaIntros = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLIntros/";
		rutaTemaMensajes = Application.dataPath + "/StreamingAssets/XMLDialogo/XMLMensajes/XMLTemaMensajes/";
		rutaMensajes = Application.dataPath + "/StreamingAssets/XMLDialogo/XMLMensajes/";
		rutaGrupos = Application.dataPath + "/StreamingAssets/XMLDialogo/XMLGrupos/";
		rutaGruposModificados = Application.persistentDataPath + "/GruposModificados/";
		rutaGruposActivos = Application.persistentDataPath + "/GruposActivos/";
		rutaGruposAcabados = Application.persistentDataPath + "/GruposAcabados/";
		rutaLanzadores = Application.dataPath + "/StreamingAssets/XMLDialogo/XMLGrupos/Lanzador/";
		rutaInventario = Application.persistentDataPath + "/Inventario/";
		rutaObjetoInventario = Application.dataPath + "/StreamingAssets/ObjetoInventario/";
		rutaInventarioTienda = Application.dataPath + "/StreamingAssets/Tiendas/";
		rutaTransportes = Application.dataPath + "/StreamingAssets/Transportes/";

		rutaDialogoVacio = rutaInterDialogos + "-1.xml";
	}

	private void CargarGameObjectsEstaticos()
	{
		canvasGlobal = (GameObject)Instantiate(Resources.Load("CanvasPrefab"));
		DontDestroyOnLoad(canvasGlobal); //Hacemos que el objeto no pueda ser destruido entre escenas

		GameObject objetoTemporalGO = (GameObject)Instantiate(Resources.Load("Text Box Manager"));
		DontDestroyOnLoad(objetoTemporalGO); //Hacemos que el objeto no pueda ser destruido entre escenas

		objetoTemporalGO = (GameObject)Instantiate(Resources.Load("PanelTiempoPrefab"));
		DontDestroyOnLoad(objetoTemporalGO); //Hacemos que el objeto no pueda ser destruido entre escenas
		objetoTemporalGO.transform.SetParent(canvasGlobal.transform, false);
		objetoTemporalGO.SetActive(false);

		objetoTemporalGO = (GameObject)Instantiate(Resources.Load("EventSystem"));
		DontDestroyOnLoad(objetoTemporalGO); //Hacemos que el objeto no pueda ser destruido entre escenas

		//CREAR CON PREFAB
		objetoTemporalGO = new GameObject("ManagerTiempo");
		objetoTemporalGO.transform.SetParent(gameObject.transform, false);
		objetoTemporalGO.AddComponent<ManagerTiempo>();

		//CREAR CON PREFAB
		objetoTemporalGO = new GameObject("ManagerRutinas");
		objetoTemporalGO.transform.SetParent(gameObject.transform, false);
		objetoTemporalGO.AddComponent<ManagerRutina>();

		objetoTemporalGO = (GameObject)Instantiate(Resources.Load("Ethan"));
		DontDestroyOnLoad(objetoTemporalGO); //Hacemos que el objeto no pueda ser destruido entre escenas
		objetoTemporalGO.transform.position = new Vector3(7.87f, 15.809f, -9.88f);
	}

	//Crea algunos directorios al inicio del juego si no están creados, así como algunos ficheros
	private void ComprobarArchivosDirectorios()
	{
		if (!System.IO.Directory.Exists(rutaDatosInteractuableGuardados))
		{
			System.IO.Directory.CreateDirectory(rutaDatosInteractuableGuardados);
		}

		if (!System.IO.Directory.Exists(rutaAutorutinasGuardadas))
		{
			System.IO.Directory.CreateDirectory(rutaAutorutinasGuardadas);
		}

		if (!System.IO.Directory.Exists(rutaEventosGuardados))
		{
			System.IO.Directory.CreateDirectory(rutaEventosGuardados);
		}

		if (!System.IO.Directory.Exists(rutaDatosAccionGuardados))
		{
			System.IO.Directory.CreateDirectory(rutaDatosAccionGuardados);
		}

		if (!System.IO.Directory.Exists(rutaInterDatosGuardados))
		{
			System.IO.Directory.CreateDirectory(rutaInterDatosGuardados);
		}

		//Creamos el directorio donde guardaremos los dialogos de los NPCs si no existe ya
		if (!System.IO.Directory.Exists(rutaInterDialogosGuardados))
		{
			System.IO.Directory.CreateDirectory(rutaInterDialogosGuardados);
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

	//Carga los datos de interactuables situados en un directorio y los añade a la rutina
	private void CargarDatosInteractuable()
	{
		DatosInteractuable datosInteractuable;

		DirectoryInfo directoryInfo = new DirectoryInfo(rutaDatosInteractuable);
		FileInfo[] fileInfo = directoryInfo.GetFiles().ToArray();

		for(var j = 0; j < fileInfo.Length; j++)
		{
			if(Path.GetExtension(fileInfo[j].Name) == ".xml")
			{
				datosInteractuable = null;

				if (System.IO.File.Exists(rutaDatosInteractuableGuardados + fileInfo[j].Name))
				{
					datosInteractuable = DeserializeData<DatosInteractuable>(rutaDatosInteractuableGuardados + fileInfo[j].Name);
				}
				else if(System.IO.File.Exists(rutaDatosInteractuable + fileInfo[j].Name))
				{
					datosInteractuable = DeserializeData<DatosInteractuable>(rutaDatosInteractuable + fileInfo[j].Name);
				}

				//Si el archivo DatosInteractuable con el ID existe, lo cargamos en el Manager
				if(datosInteractuable != null)
					ManagerRutina.instance.CargarInteractuable(datosInteractuable);
			}
		}
	}

	private void ComprobarEventosInicio(int horaActual)
	{
		ManagerRutina.instance.ComprobarEventosInicio(horaActual);
	}

	//Carga los interactuables al cargar una escena
	void OnLevelWasLoaded(int level)
	{
		//Cargamos los interactuables de la escena
		ManagerRutina.instance.CargarEscena(level);
	}

	//Crea un interactuable en la escena con las coordenadas y rotación especificadas
	public void CrearInteractuable(int IDInteractuable, int tipo, Vector3 coord, Quaternion rot)
	{
		GameObject interactuableGO;

		switch(tipo)
		{
		default:
		case 0: //Tipo NPC
			interactuableGO = (GameObject)Instantiate(Resources.Load("InteractuableNPC"));
			InteractuableNPC interactuableNPC = interactuableGO.gameObject.GetComponent<InteractuableNPC>();
			interactuableNPC.ID = IDInteractuable;
			break;
		case 1: //Tipo Objeto
			interactuableGO = (GameObject)Instantiate(Resources.Load("InteractuableObjeto"));
			InteractuableObjeto interactuableObjeto = interactuableGO.gameObject.GetComponent<InteractuableObjeto>();
			interactuableObjeto.ID = IDInteractuable;
			break;
		}

		interactuableGO.transform.position = coord;
		interactuableGO.transform.rotation = rot;
	}

	//Crea un interactuable en la escena en un transporte que conecta con la escena anterior
	public void MoverInteractuableDesdeOtraEscena(int IDInteractuable, int tipo, int IDEscenaAnterior, Vector3 coord, Quaternion rot)
	{
		GameObject interactuableGO;

		switch(tipo)
		{
		default:
		case 0: //Tipo NPC
			interactuableGO = (GameObject)Instantiate(Resources.Load("InteractuableNPC"));
			InteractuableNPC interactuableNPC = interactuableGO.gameObject.GetComponent<InteractuableNPC>();
			interactuableNPC.ID = IDInteractuable;

			//Si el interactuable es de tipo NPC, lo creamos en un transporte
			GameObject transporteGO = EncontrarTransporteEscena(IDEscenaAnterior);

			if(transporteGO != null)
			{
				interactuableGO.transform.position = transporteGO.transform.position;
				interactuableNPC.SetRuta(coord);

				GameObject rutaColliderGO = new GameObject("RutaCollider");
				rutaColliderGO.transform.position = coord;
				BoxCollider boxCollider = rutaColliderGO.AddComponent<BoxCollider>();
				boxCollider.size =  new Vector3(3.7f, 6.68f, 1f);
				boxCollider.isTrigger = true;

				RutaCollider rutaCollider = rutaColliderGO.AddComponent<RutaCollider>();
				rutaCollider.SetIDInteractuable(IDInteractuable);

				if(transporteGO.GetComponent<TransporteInter>().ComprobarSiEsTransporteObjeto())
				{
					transporteGO.transform.parent.GetComponent<InteractuableObjeto>().SetNavObstacle(false);

					GameObject transporteColliderGO = new GameObject("TransporteCollider");
					transporteColliderGO.transform.SetParent(transporteGO.transform, false);
					BoxCollider boxcollider = transporteColliderGO.AddComponent<BoxCollider>();
					boxcollider.size =  new Vector3(3.7f, 6.68f, 1f);
					boxcollider.isTrigger = true;

					SaliendoTransporteCollider saliendoTransporteCollider = transporteColliderGO.AddComponent<SaliendoTransporteCollider>();

					saliendoTransporteCollider.SetIDInteractuable(IDInteractuable);
					saliendoTransporteCollider.SetTransporte(transporteGO);
				}
			}
			else
			{
				interactuableGO.transform.position = coord;
				interactuableGO.transform.rotation = rot;
			}
			break;
		case 1: //Tipo Objeto
			//Si el interatuable es de tipo objeto, lo creamos directamente
			interactuableGO = (GameObject)Instantiate(Resources.Load("InteractuableObjeto"));
			InteractuableObjeto interactuableObjeto = interactuableGO.gameObject.GetComponent<InteractuableObjeto>();
			interactuableObjeto.ID = IDInteractuable;

			interactuableGO.transform.position = coord;
			interactuableGO.transform.rotation = rot;
			break;
		}
	}

	public void MoverInteractuableEnEscena(int tipo, int IDInteractuable, Vector3 coord, Quaternion rot)
	{
		GameObject interactuable = GetInteractuable(IDInteractuable);

		//Si el interactuable no es un NPC, lo movemos directamente
		if(tipo != 0)
		{
			interactuable.transform.position = coord;
			interactuable.transform.rotation = rot;
		}
		//Si es un NPC, establecemos la ruta que debe seguir, si el NPC no se encuentra ya en exactamente esa posición
		else if(interactuable.transform.position != coord)
		{
			InteractuableNPC interactuableNPC = interactuable.GetComponent<InteractuableNPC>();
			interactuableNPC.SetRuta(coord);
		}
	}

	public void MoverInteractuableHaciaOtraEscena(int tipo, int IDInteractuable, int IDEscena)
	{
		GameObject interactuableGO = GetInteractuable(IDInteractuable);

		if(tipo != 0)
		{
			Destroy(interactuableGO);
		}
		//Si es de tipo NPC, buscamos un transporte al que movernos
		else
		{
			GameObject transporteMasCercano = EncontrarTransporteInteractuable(interactuableGO, IDEscena);

			if(transporteMasCercano != null)
			{
				interactuableGO.GetComponent<InteractuableNPC>().SetRuta(transporteMasCercano.transform.position);

				GameObject transporteColliderGO = new GameObject("TransporteCollider");
				transporteColliderGO.transform.SetParent(transporteMasCercano.transform, false);
				BoxCollider boxCollider = transporteColliderGO.AddComponent<BoxCollider>();
				boxCollider.size =  new Vector3(3.7f, 6.68f, 1f);
				boxCollider.isTrigger = true;

				TransporteCollider transporteCollider = transporteColliderGO.AddComponent<TransporteCollider>();
				transporteCollider.SetIDInteractuable(IDInteractuable);

				if(transporteMasCercano.GetComponent<TransporteInter>().ComprobarSiEsTransporteObjeto())
				{
					transporteMasCercano.transform.parent.GetComponent<InteractuableObjeto>().SetNavObstacle(false);
					transporteCollider.SetTransporte(transporteMasCercano);
				}
			}
			//si no existe un transporte que nos lleve a la escena que queremos, destruimos el interactuable
			else
			{
				Destroy(interactuableGO);
			}
		}
	}

	/*
	 * 
	 * 
	 *  INTERACTUABLES
	 * 
	 * 
	 */

	public void AddToInteractuables(int IDInteractuable, GameObject gameobject)
	{
		interactuables[IDInteractuable] = gameobject;
	}

	public void RemoveFromInteractuables(int IDInteractuable)
	{
		interactuables.Remove(IDInteractuable);
	}

	//Devuelve el GameObject con el id especificado, sino existe, el gameobject devuelto es null
	public GameObject GetInteractuable(int IDInteractuable)
	{
		GameObject interactuableGO;

		interactuables.TryGetValue(IDInteractuable,out interactuableGO);

		return interactuableGO;
	}

	//Devuelve una lista compuesta por los elementos del diccionario interactuables
	public List<GameObject> GetAllInteractuables()
	{
		return interactuables.Select(d => d.Value).ToList();
	}

	//Recarga las acciones de los interactuables de la lista

	//CREAR OTRA FUNCIÓN DE ACTUALIZAR ACCIONES DONDE NO INTERVENGA EL INVENTARIO, PARA CUANDO SE ACTUALICEN LAS VARIABLES DE UN OBJETO

	public void ActualizarAcciones(Inventario inventario)
	{
		foreach(var entry in interactuables.Values)
		{
			// do something with entry.Value or entry.Key
			Interactuable interactuable = entry.GetComponent<Interactuable>() as Interactuable;
			interactuable.RecargarAcciones(inventario);
		}
	}

	/*
	 * 
	 * 
	 *  INTERACTUABLES CERCANOS
	 * 
	 * 
	 */

	public int DevuelveNumeroInteractuablesCercanos()
	{
		return interactuablesCercanos.Count;
	}

	public GameObject DevuelveInteractuableCercano(int num)
	{
		return interactuablesCercanos[num];
	}

	public void AddInteractuableCercano(GameObject gameobject)
	{
		interactuablesCercanos.Add(gameobject);
	}

	public void DeleteInteractuableCercano(GameObject gameobject)
	{
		interactuablesCercanos.Remove(gameobject);
	}

	/*
	 * 
	 * 
	 *  NAVMESH AGENT CON RUTAS ACTIVAS
	 * 
	 * 
	 */

	public void AddNavMeshAgent(NavMeshAgent navMeshAgent)
	{
		if(!navMeshAgentRutasActivas.Contains(navMeshAgent))
			navMeshAgentRutasActivas.Add(navMeshAgent);
	}

	public void DeleteNavhMeshAgent(NavMeshAgent navMeshAgent)
	{
		navMeshAgentRutasActivas.Remove(navMeshAgent);
	}

	//Pausa las navMesh de la lista de navMesh con rutas activas
	public void StopNavMeshAgents()
	{
		for(int i = 0; i < navMeshAgentRutasActivas.Count; i++)
		{
			navMeshAgentRutasActivas[i].velocity = Vector3.zero; //Detiene el navmesh totalmente, sin desaceleración
			navMeshAgentRutasActivas[i].Stop();
		}
	}

	//Reanuda las navMesh de la lista de navMesh con rutas activas
	public void ResumeNavMeshAgents()
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
	public void AnyadirTransporte(int numEscenaTransporte, GameObject transporteGO, List<int> escenas)
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
	
			listaTransportes.Add(transporteGO);
		}
	}

	//Busca el transporte más cercano del interactuable especificado hacia la IDEscena especificada
	//Devuelve un GameObject null si no ha encontrado ninguno
	private GameObject EncontrarTransporteInteractuable(GameObject interactuableGO, int IDEscena)
	{
		List<GameObject> listaTransportes;
		GameObject transporteMasCercano = null;

		if (transportes.TryGetValue(IDEscena, out listaTransportes))
		{
			float distanciaMasCercana = Mathf.Infinity;

			for(int i = 0; i < listaTransportes.Count; i++)
			{
				float distancia = Vector3.Distance(interactuableGO.transform.position, listaTransportes[i].transform.position);
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
			System.Random random = new System.Random();

			//Cogemos un transporte aleatorio de la lista
			transporteMasCercano = listaTransportes[random.Next(listaTransportes.Count-1)];
		}

		return transporteMasCercano;
	}

	public GameObject EncontrarTransporte(int IDTransporte)
	{
		GameObject transporteGO = null;

		foreach(var entry in transportes.Values)
		{
			for(int i = 0; i < entry.Count; i++)
			{
				TransporteInter transporteInter = entry[i].GetComponent<TransporteInter>();

				if(transporteInter.ID == IDTransporte)
				{
					transporteGO = entry[i];
				}
			}
		}

		return transporteGO;
	}

	/*
	 * 
	 * 
	 *  GRUPOS
	 * 
	 * 
	 */

	public void AddToGruposActivos(Grupo grupo)
	{
		gruposActivos.Add(grupo);
	}

	public bool GrupoActivoExiste(int IDGrupo)
	{
		return gruposActivos.Any(x => x.IDGrupo == IDGrupo);
	}
		
	public bool GrupoAcabadoExiste(int IDGrupo)
	{
		return gruposAcabados.IndexOf(IDGrupo) != -1;
	}

	//Devuelve un grupo activo con el id especificado, null si no existe en la lista
	public Grupo DevolverGrupoActivo(int IDGrupo)
	{
		return gruposActivos.Find (x => x.DevolverIDGrupo () == IDGrupo);
	}

	//Elimina el grupo indicado de la lista de grupos activos
	//y lo añade a la de grupos acabados, aunque no estuviera en los grupos activos
	public void RemoveFromGruposActivos(int IDGrupo)
	{
		//Buscamos el grupo en la lista de grupos activos
		Grupo grupo = DevolverGrupoActivo(IDGrupo);

		if (grupo != null)
		{
			gruposActivos.Remove (grupo); //lo borramos de la lista de grupos activos
		}

		//El grupo se añade a la lista de grupos acabados, estuviera o no en
		//la lista de grupos activos
		gruposAcabados.Add (IDGrupo); //Añadimos la id del grupo acabado

		//Buscamos si el grupo estaba en la lista de grupos modificados
		//y lo borramos de ahí también
		if(System.IO.File.Exists(rutaGruposModificados + IDGrupo.ToString () + ".xml"))
		{
			System.IO.File.Delete(rutaGruposModificados + IDGrupo.ToString () + ".xml");
		}
	}

	//Suma a una variable situada en la posicion num de la lista de variables de un grupo activo el valor
	public void AddVariablesGrupo(int IDGrupo, int num, int valor)
	{
		int indice = gruposActivos.FindIndex(x => x.IDGrupo == IDGrupo);
		gruposActivos[indice].variables[num] += valor;
	}

	//Establece a una variable situada en la posicion num de la lista de variables de un grupo activo el valor
	public void SetVariablesGrupo(int IDGrupo, int num, int valor)
	{
		int indice = gruposActivos.FindIndex(x => x.IDGrupo == IDGrupo);
		gruposActivos[indice].variables[num] = valor;
	}

	private void CargarGruposActivos()
	{
		gruposActivos = DeserializeData<List<Grupo>>(rutaGruposActivos + "GruposActivos.xml");
	}

	private void CargarGruposAcabados()
	{
		gruposAcabados = DeserializeData<List<int>>(rutaGruposAcabados + "GruposAcabados.xml");
	}

	public void GuardarGruposActivos()
	{
		//Antes de serializar los grupos activos, comprueba si
		//entre ellos hay algún grupo modificado para eliminar su fichero
		ComprobarGruposModificados();
		SerializeData(gruposActivos, rutaGruposActivos, "GruposActivos.xml");
	}

	//Borra los ficheros de grupos modificados que ahora son grupos activos
	public void ComprobarGruposModificados()
	{
		for(int i = 0; i < gruposActivos.Count; i++)
		{
			int ID = gruposActivos[i].DevolverIDGrupo();

			if(System.IO.File.Exists(rutaGruposModificados + ID.ToString () + ".xml"))
			{
				System.IO.File.Delete(rutaGruposModificados + ID.ToString () + ".xml");
			}
		}
	}

	public void GuardarGruposAcabados()
	{
		SerializeData(gruposAcabados, rutaGruposAcabados, "GruposAcabados.xml");
	}

	/*
	 * 
	 * 
	 *  OBJETOS RECIENTES
	 * 
	 * 
	 */

	public int DevuelveNumeroObjetosRecientes()
	{
		return objetosRecientes.Count;
	}

	//Devuelve un objeto reciente situado en la posición num de la lista de objetosRecientes
	public string DevuelveNombreObjetoReciente(int num)
	{
		return objetosRecientes[num].DevuelveObjeto().nombre;
	}

	public void AddObjetoReciente(ObjetoInventario objetoInventario, int cantidad)
	{
		objetosRecientes.Add(new ObjetoReciente(objetoInventario, cantidad));
	}

	public void VaciarObjetosRecientes()
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

	public void AddToColaObjetos(string path, ObjetoSerializable objetoSerializable)
	{
		//Comprobamos si ya existe el objeto indicado
		//Si ya existe, lo eliminamos
		if (ColaObjetoExiste(path))
			RemoveFromColaObjetos(path);

		ColaObjeto colaObjeto = new ColaObjeto(objetoSerializable, path);
		colaObjetos.Add(colaObjeto);
	}

	public bool ColaObjetoExiste(string path)
	{
		return colaObjetos.Any(x => x.GetRuta() == path);
	}

	public ColaObjeto GetColaObjetos(string path)
	{
		return colaObjetos.Find (x => x.GetRuta() == path);
	}

	//Devuelve una lista con objetosSerializables solo del tipo pasado
	public List<ObjetoSerializable> GetColaObjetosTipo(Type tipo)
	{
		List<ObjetoSerializable> listaObjetos = new List<ObjetoSerializable>();

		for(int i = 0; i < colaObjetos.Count; i++)
		{
			if(colaObjetos[i].GetObjeto().GetType() == tipo)
			{
				listaObjetos.Add(colaObjetos[i].GetObjeto());
			}
		}

		return listaObjetos;
	}

	public void RemoveFromColaObjetos(string path)
	{
		ColaObjeto cobj = GetColaObjetos(path);

		if (cobj != null)
		{
			colaObjetos.Remove(cobj);
		}
	}

	//Guardamos los objetos de la cola y la vaciamos
	public void SerializarCola()
	{
		for(var i = 0; i < colaObjetos.Count; i++)
		{
			string ruta = colaObjetos[i].GetRuta();
			ObjetoSerializable objetoSerializable = colaObjetos[i].GetObjeto();

			SerializeData(objetoSerializable, Path.GetDirectoryName(ruta), Path.GetFileName(ruta));

			//Si el inventario se ha actualizado, actualizamos el estado
			//de las acciones de los interactuables
			if(ruta == rutaInventario + "Inventario.xml")
			{
				Inventario inventario = objetoSerializable as Inventario;
				ActualizarAcciones(inventario);
			}
		}

		colaObjetos.Clear();
	}

	//Guarda los datos de la partida
	public void ActualizarDatos()
	{
		GuardarGruposActivos();
		GuardarGruposAcabados();
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