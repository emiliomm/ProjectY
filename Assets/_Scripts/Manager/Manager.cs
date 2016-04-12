using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text; 

public class Manager : MonoBehaviour {

	public static Manager Instance { get; private set; } //singleton

	public Dictionary<int,GameObject> npcs; //grupos de npcs cargados en la escena
	public List<Grupo> GruposActivos; //grupos de misiones

	//Lista de rutas
	public static string rutaNPCDialogos;
	public static string rutaNPCDialogosGuardados;
	public static string rutaIntros;
	public static string rutaMensajes;
	public static string rutaGrupos;
	public static string rutaGruposModificados;
	public static string rutaGruposActivos;
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

		GruposActivos = new List<Grupo>();
		npcs = new Dictionary<int,GameObject>();

		//Cargamos las rutas
		rutaNPCDialogos = Application.dataPath + "/StreamingAssets/NPCDialogue/";
		rutaNPCDialogosGuardados = Application.persistentDataPath + "/NPC_Dialogo_Saves/";
		rutaIntros = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLIntros/";
		rutaMensajes = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLMensajes/";
		rutaGrupos = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLGrupos/";
		rutaGruposModificados = Application.persistentDataPath + "/Grupos_Modificados/";
		rutaGruposActivos = Application.persistentDataPath + "/Grupos_Activos/";
		rutaLanzadores = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLGrupos/Lanzador/";

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
		if(!Directory.Exists(rutaGruposActivos))
		{    
			//if it doesn't, create it
			Directory.CreateDirectory(rutaGruposActivos);
		}
		//Si ya existe, comprobamos si existe el fichero de gruposactivos
		else if(System.IO.File.Exists(rutaGruposActivos + "GruposActivos.xml"))
		{
			CargarGruposActivos();
		}

		//Cargamos el escenario
		SceneManager.LoadScene("Demo");
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
		return npcs.Select(d=> d.Value).ToList();
	}

	public void AddToGrupos(Grupo g)
	{
		GruposActivos.Add(g);
	}

	public bool ComprobarGrupo(int id)
	{
		bool existe = true;

		if (DevolverGrupo(id) == null)
			existe = false;

		return existe;
	}

	public Grupo DevolverGrupo(int id)
	{
		return GruposActivos.Find(x => x.DevolverGrupoID() == id);
	}

	public void RemoveFromGrupos(int id)
	{
		Grupo g = DevolverGrupo(id);

		if (g != null)
			GruposActivos.Remove(g);
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
		XmlSerializer deserz = new XmlSerializer(typeof(List<Grupo>));
		StreamReader reader = new StreamReader(rutaGruposActivos + "GruposActivos.xml");

		GruposActivos = (List<Grupo>)deserz.Deserialize(reader);
		reader.Close();
	}

	public void GuardarGruposActivos()
	{
		SerializeToXmlGruposActivos();
	}

	/*
	 * 
	 * SERIALIZACIÓN Y DESERIALIZACIÓN
	 * 
	 */

	public void SerializeToXmlGruposActivos()
	{
		string _data = SerializeObject(GruposActivos); 
		// This is the final resulting XML from the serialization process
		CreateXML(_data);
	}

	//Serializa el objeto en xml que se le pasa
	string SerializeObject(object pObject) 
	{ 
		string XmlizedString = null; 
		MemoryStream memoryStream = new MemoryStream(); 
		XmlSerializer xs = new XmlSerializer(typeof(List<Grupo>)); 
		XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8); 
		xs.Serialize(xmlTextWriter, pObject); 
		memoryStream = (MemoryStream)xmlTextWriter.BaseStream; 
		XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray()); 
		return XmlizedString; 
	} 

	/* The following metods came from the referenced URL */ 
	string UTF8ByteArrayToString(byte[] characters) 
	{      
		UTF8Encoding encoding = new UTF8Encoding(); 
		string constructedString = encoding.GetString(characters); 
		return (constructedString); 
	} 

	void CreateXML(string _data) 
	{
		StreamWriter writer; 

		//check if directory doesn't exit
		if(!Directory.Exists(rutaGruposActivos))
		{    
			//if it doesn't, create it
			Directory.CreateDirectory(rutaGruposActivos);
		}

		FileInfo t = new FileInfo(rutaGruposActivos + "GruposActivos.xml");

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


