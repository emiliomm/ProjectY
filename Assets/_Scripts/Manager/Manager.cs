﻿using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text;

public class Manager : MonoBehaviour {

	public static Manager Instance { get; private set; } //singleton

	public Dictionary<int, List<Rutina>> rutinas; //Diccionario con el lugar actual y una lista con la rutina de los npcs

	public Dictionary<int,GameObject> interactuables; //grupos de npcs cargados en la escena actual (id, gameobject)

	public List<ColaObjeto> ColaObjetos; //cola con los objetos por serializar ¿convertir a cola?

	public List<Grupo> GruposActivos; //grupos activos
	private List<int> GruposAcabados; //ids de los grupos acabados

	private int SegundosEntreSeccion = 2;
	private int Hora = 0;

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

	private string nombreJugador;

	//Lista de rutas
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
	public static string rutaRutinas;

	void Awake()
	{
		// First we check if there are any other instances conflicting
		if(Instance != null && Instance != this)
		{
			// If that is the case, we destroy other instances
			Destroy(gameObject);
		}

		Instance = this;

		DontDestroyOnLoad(gameObject); //Hacemos que el objeto no pueda ser destruido entre escenas

		SetState(EstadoJuego.Activo); //Estado Inicial

		Cursor.visible = false; //Oculta el cursor del ratón

		nombreJugador = "Jugador"; //Nombre por defecto del jugador

		rutinas = new Dictionary<int,List<Rutina>>();
		interactuables = new Dictionary<int,GameObject>();

		ColaObjetos = new List<ColaObjeto>();

		GruposActivos = new List<Grupo>();
		GruposAcabados = new List<int>();

		//Cargamos las rutas
		rutaDatosAccion = Application.dataPath + "/StreamingAssets/DatosAccion/";
		rutaDatosAccionGuardados = Application.persistentDataPath + "/DatosAccion/";
		rutaNPCDatos = Application.dataPath + "/StreamingAssets/NPCDatos/";
		rutaNPCDatosGuardados = Application.persistentDataPath + "/NPC_Datos_Saves/";
		rutaNPCDialogos = Application.dataPath + "/StreamingAssets/NPCDialogue/";
		rutaNPCDialogosGuardados = Application.persistentDataPath + "/NPC_Dialogo_Saves/";
		rutaIntros = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLIntros/";
		rutaTemaMensajes = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLMensajes/XMLMensajeTemas/";
		rutaMensajes = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLMensajes/";
		rutaGrupos = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLGrupos/";
		rutaGruposModificados = Application.persistentDataPath + "/Grupos_Modificados/";
		rutaGruposActivos = Application.persistentDataPath + "/Grupos_Activos/";
		rutaGruposAcabados = Application.persistentDataPath + "/Grupos_Acabados/";
		rutaLanzadores = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLGrupos/Lanzador/";
		rutaRutinas = Application.dataPath + "/StreamingAssets/Rutinas/";

		//Comprobamos si los directorios necesarios existen y cargamos algunos ficheros
		ComprobarArchivosDirectorios();

		//Cargamos las rutinas de los interactuables
		cargarRutinas();

		//Empieza a calcular el tiempo hasta la siguiente sección de las noticias
		StartCoroutine(SiguienteSeccion());

		//Cargamos el escenario
		SceneManager.LoadScene("Demo");
	}

	private void ComprobarArchivosDirectorios()
	{
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
	}

	public string DevuelveNombreJugador()
	{
		return nombreJugador;
	}

	/*
	 * 
	 * 
	 *  RUTINAS
	 * 
	 * 
	 */

	private void cargarRutinas()
	{
		var info = new DirectoryInfo(Manager.rutaRutinas);
		var fileInfo = info.GetFiles().ToArray();

		for(var j = 0; j < fileInfo.Length; j++)
		{
			if(Path.GetExtension(fileInfo[j].Name) == ".xml")
			{
				//Cogemos el nombre del archivo
				string nombre = Path.GetFileNameWithoutExtension(fileInfo[j].Name);

				//Deserializamos la lista de rutinas de la zona
				List<Rutina> rutina = new List<Rutina>();

				DeserializeData(ref rutina, Manager.rutaRutinas + nombre + ".xml");

				//La añadimos al diccionario
				rutinas.Add(Int32.Parse(nombre), rutina);
			}
		}
	}
		
	//Carga los interactuables al cargar una escena
	void OnLevelWasLoaded(int level)
	{
		List<Rutina> rutinaNiveles = new List<Rutina>();

		//Creamos los interactuables de la escena actual (si existen)
		if(rutinas.TryGetValue(level, out rutinaNiveles))
		{
			for(int i = 0; i < rutinaNiveles.Count; i++)
			{
				int ID_Interactuable = rutinaNiveles[i].ID_Inter;
				int tipo_Inter = rutinaNiveles[i].tipo_Inter;

				GameObject interactuable;

				switch(tipo_Inter)
				{
				default:
				case 0: //Tipo NPC
					interactuable = (GameObject)Instantiate(Resources.Load("InteractuableNPC"));
					InteractuableNPC iNPC = interactuable.gameObject.GetComponent<InteractuableNPC>();
					iNPC.ID = ID_Interactuable;
					break;
				case 1: //Tipo Objeto
					interactuable = (GameObject)Instantiate(Resources.Load("InteractuableObjeto"));
					InteractuableObjeto iObj = interactuable.gameObject.GetComponent<InteractuableObjeto>();
					iObj.ID = ID_Interactuable;
					break;
				}

				interactuable.transform.position = new Vector3(rutinaNiveles[i].lugares[0].coordX, rutinaNiveles[i].lugares[0].coordY, rutinaNiveles[i].lugares[0].coordZ);
			}
		}
	}

	//Pasa a la siguiente sección de las rutinas
	public IEnumerator SiguienteSeccion()
	{
		int TiempoActualEntreSecciones = SegundosEntreSeccion;

		while(true)
		{
			yield return new WaitForSeconds (1f);

			switch(State)
			{
			case EstadoJuego.Activo:
				TiempoActualEntreSecciones--;

				if(TiempoActualEntreSecciones == 0)
				{
					//Comprobamos si tenemos que cambiar la sección horaria de las rutinas


					//Aumentamos la hora
					Hora++;
					Hora = Hora % 24;

					TiempoActualEntreSecciones = SegundosEntreSeccion;
				}
				break;
			case EstadoJuego.Pausa:
				break;
			}
		}
	}

	public void setPausa(bool pausa)
	{
		if(pausa)
			SetState(EstadoJuego.Activo);
		else
			SetState(EstadoJuego.Pausa);
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
		interactuables.Add(id, gobj);
	}

	public void RemoveFromInteractuables(int id)
	{
		interactuables.Remove(id);
	}

	public GameObject GetInteractuables(int id)
	{
		GameObject npc;

		//Coge el GameObject mediante referencia, sino existe, el gameobject es null
		interactuables.TryGetValue(id,out npc);

		return npc;
	}

	//Devuelve una lista de los valores del diccionario
	public List<GameObject> GetAllInteractuables()
	{
		return interactuables.Select(d => d.Value).ToList();
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
		return GruposActivos.Any(x => x.idGrupo == id);
	}
		
	public bool GrupoAcabadoExiste(int id)
	{
		bool existe = GruposAcabados.IndexOf(id) != -1;

		return existe;
	}

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

	public void AddVariablesGrupo(int id, int num, int valor)
	{
		int indice = GruposActivos.FindIndex(x => x.idGrupo == id);
		GruposActivos[indice].variables[num] += valor;
	}

	public void SetVariablesGrupo(int id, int num, int valor)
	{
		int indice = GruposActivos.FindIndex(x => x.idGrupo == id);
		GruposActivos[indice].variables[num] = valor;
	}

	private void CargarGruposActivos()
	{
		DeserializeData(ref GruposActivos, rutaGruposActivos + "GruposActivos.xml");
	}

	private void CargarGruposAcabados()
	{
		DeserializeData(ref GruposAcabados, rutaGruposAcabados + "GruposAcabados.xml");
	}

	public void GuardarGruposActivos()
	{
		//Antes de serializar los grupos activos, comprueba si
		//entre ellos hay algún grupo modificado para eliminar su fichero
		ComprobarGruposModificados();
		SerializeData(GruposActivos, rutaGruposActivos, rutaGruposActivos + "GruposActivos.xml");
	}
		
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
		SerializeData(GruposAcabados, rutaGruposAcabados, rutaGruposAcabados + "GruposAcabados.xml");
	}

	/*
	 * 
	 * 
	 *  COLAOBJETOS
	 * 
	 * 
	 */

	public void AddToColaObjetos(string path, ObjetoSer obj)
	{
		//Comprobamos si ya existe el objeto indicado
		//en la cola para eliminarlo
		if (ColaObjetoExiste(path))
			RemoveFromColaObjetos(path);

		ColaObjeto item = new ColaObjeto(obj, path);
		ColaObjetos.Add(item);
	}

	public bool ColaObjetoExiste(string path)
	{
		bool existe = ColaObjetos.Any(x => x.GetRuta() == path);

		return existe;
	}

	public ColaObjeto GetColaObjetos(string path)
	{
		return ColaObjetos.Find (x => x.GetRuta() == path);
	}

	public List<ObjetoSer> GetColaObjetosTipo(Type tip)
	{
		List<ObjetoSer> listaObjetos = new List<ObjetoSer>();

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

	public void SerializarCola()
	{
		for(var i = 0; i < ColaObjetos.Count; i++)
		{
			string ruta = ColaObjetos[i].GetRuta();
			ObjetoSer obj = ColaObjetos[i].GetObjeto();

			SerializeData(obj, Path.GetDirectoryName(ruta), ruta);
		}

		ColaObjetos.Clear();
	}

	public void ActualizarDatos()
	{
		Manager.Instance.SerializarCola ();
		Manager.Instance.GuardarGruposActivos();
		Manager.Instance.GuardarGruposAcabados();
	}

	/*
	 * 
	 * 
	 *  SERIALIZACION Y DESERIALIZACION
	 * 
	 * 
	 */

	public void DeserializeData<T>(ref T pObject, string rutaArchivo)
	{
		XmlSerializer deserz = new XmlSerializer(typeof(T));
		StreamReader reader = new StreamReader(rutaArchivo);

		pObject = (T)deserz.Deserialize(reader);

		reader.Close();
	}

	public T DeserializeDataWithReturn<T>(string rutaArchivo)
	{
		XmlSerializer deserz = new XmlSerializer(typeof(T));
		StreamReader reader = new StreamReader(rutaArchivo);

		T pObject = (T)deserz.Deserialize(reader);

		reader.Close();

		return pObject;
	}

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

		t = new FileInfo(nombreArchivo);
			

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