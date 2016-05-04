using UnityEngine;
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

	public Dictionary<int,GameObject> npcs; //grupos de npcs cargados en la escena
	public Dictionary<string,object> ColaObjetos; //cola con los objetos por serializar
	public List<Grupo> GruposActivos; //grupos activos

	private string nombreJugador;

	private List<int> GruposAcabados; //ids de los grupos acabados

	//Lista de rutas
	public static string rutaNPCDatos;
	public static string rutaNPCDatosGuardados;
	public static string rutaNPCDialogos;
	public static string rutaNPCDialogosGuardados;
	public static string rutaIntros;
	public static string rutaMensajes;
	public static string rutaGrupos;
	public static string rutaGruposModificados;
	public static string rutaGruposActivos;
	public static string rutaGruposAcabados;
	public static string rutaLanzadores;

	void Awake()
	{
		// First we check if there are any other instances conflicting
		if(Instance != null && Instance != this)
		{
			// If that is the case, we destroy other instances
			Destroy(gameObject);
		}

		Instance = this;

		DontDestroyOnLoad(gameObject);

		Cursor.visible = false; //Oculta el cursor del ratón

		nombreJugador = "Jugador";

		GruposActivos = new List<Grupo>();
		GruposAcabados = new List<int>();
		npcs = new Dictionary<int,GameObject>();
		ColaObjetos = new Dictionary<string,object>();

		//Cargamos las rutas
		rutaNPCDatos = Application.dataPath + "/StreamingAssets/NPCDatos/";
		rutaNPCDatosGuardados = Application.persistentDataPath + "/NPC_Datos_Saves/";
		rutaNPCDialogos = Application.dataPath + "/StreamingAssets/NPCDialogue/";
		rutaNPCDialogosGuardados = Application.persistentDataPath + "/NPC_Dialogo_Saves/";
		rutaIntros = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLIntros/";
		rutaMensajes = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLMensajes/";
		rutaGrupos = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLGrupos/";
		rutaGruposModificados = Application.persistentDataPath + "/Grupos_Modificados/";
		rutaGruposActivos = Application.persistentDataPath + "/Grupos_Activos/";
		rutaGruposAcabados = Application.persistentDataPath + "/Grupos_Acabados/";
		rutaLanzadores = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLGrupos/Lanzador/";

		//Comprobamos si los directorios necesarios existen y cargamos algunos ficheros
		ComprobarArchivosDirectorios();

		//Cargamos el escenario
		SceneManager.LoadScene("Demo");
	}

	private void ComprobarArchivosDirectorios()
	{
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

	public void AddToNpcs(int id, GameObject gobj)
	{
		npcs.Add(id, gobj);
	}

	public void RemoveFromNpcs(int id)
	{
		npcs.Remove(id);
	}

	public GameObject GetNPC(int id)
	{
		GameObject npc;

		//Coge el GameObject mediante referencia, sino existe, el gameobject es null
		npcs.TryGetValue(id,out npc);

		return npc;
	}

	//Devuelve una lista de los valores del diccionario
	public List<GameObject> GetAllNPCs()
	{
		return npcs.Select(d => d.Value).ToList();
	}

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
		return GruposActivos.Find (x => x.DevolverGrupoID () == id);
	}

	public void RemoveFromGruposActivos(int id)
	{
		Grupo g = DevolverGrupoActivo(id);

		if (g != null)
		{
			GruposAcabados.Add (g.idGrupo); //Añadimos la id del grupo acabado
			GruposActivos.Remove (g);
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
		SerializeData(GruposActivos, rutaGruposActivos, rutaGruposActivos + "GruposActivos.xml");
	}

	public void GuardarGruposAcabados()
	{
		SerializeData(GruposAcabados, rutaGruposAcabados, rutaGruposAcabados + "GruposAcabados.xml");
	}

	//Comprobar si ya existe y si lo hace actualizarlo
	public void AddToColaObjetos(string path, object obj)
	{
		ColaObjetos.Add(path, obj);
	}

	public void SerializarCola()
	{
		foreach(KeyValuePair<string, object> entry in ColaObjetos)
		{
			// do something with entry.Value or entry.Key
			SerializeData(entry.Value, Path.GetDirectoryName(entry.Key), entry.Key);
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
	 * SERIALIZACIÓN Y DESERIALIZACIÓN
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