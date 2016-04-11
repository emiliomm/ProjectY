using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

using System.Xml; 
using System.Text; 

using DialogueTree;

public class Grupo{
	
	public int idGrupo;
	public List<int> variables;

	public Grupo()
	{
		variables = new List<int>();
	}

	//Devuelve un grupo
	public static Grupo CreateGrupo(string path)
	{
		XmlSerializer deserz = new XmlSerializer(typeof(Grupo));
		StreamReader reader = new StreamReader(path);

		Grupo grup = (Grupo)deserz.Deserialize(reader);
		reader.Close();

		return grup;
	}

	//Carga un grupo, lo añade al manager y lanza los intros/mensajes de este
	public static Grupo LoadGrupo(string path, int ID_NPC, int tipo_dialogo, ref int num_dialogo)
	{
		XmlSerializer deserz = new XmlSerializer(typeof(Grupo));
		StreamReader reader = new StreamReader(path);

		Grupo grup = (Grupo)deserz.Deserialize(reader);
		reader.Close();

		Lanzador.LoadLanzador(Manager.rutaLanzadores + grup.idGrupo.ToString() + ".xml", ID_NPC, tipo_dialogo, ref num_dialogo);

		Manager.Instance.AddToGrupos(grup);

		return grup;
	}

	public int DevolverGrupoID()
	{
		return idGrupo;
	}

	/*
	 * 
	 * SERIALIZACIÓN Y DESERIALIZACIÓN
	 * 
	 */

	//CUIDADO !!! ESTO SERIALIZA A LA CARPETA DE GRUPOS MODIFICADOS, NO USAR PARA GUARDAR LA LISTA DE GRUPOS
	public void SerializeToXml()
	{
		string _data = SerializeObject(this); 
		// This is the final resulting XML from the serialization process
		CreateXML(_data);
	}

	//Serializa el objeto en xml que se le pasa
	string SerializeObject(object pObject) 
	{ 
		string XmlizedString = null; 
		MemoryStream memoryStream = new MemoryStream(); 
		XmlSerializer xs = new XmlSerializer(typeof(Grupo)); 
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
		if(!Directory.Exists(Manager.rutaGruposModificados))
		{    
			//if it doesn't, create it
			Directory.CreateDirectory(Manager.rutaGruposModificados);
		}

		FileInfo t = new FileInfo(Manager.rutaGruposModificados + idGrupo.ToString()  + ".xml"); 

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

	/*
	 * 
	 * 
	 * CLASE LANZADOR
	 * 
	 */


}
