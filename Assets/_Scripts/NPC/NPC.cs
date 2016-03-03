using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text; 

using UnityEngine.UI;

public class NPC : MonoBehaviour {

	public bool requiredButtonPress; //indica si se requiere que se pulse una tecla para iniciar la conversación
	public NPC_Dialogo npc_diag; //NPC del cual carga el dialogo

	private bool waitForPress;

	void Start()
	{
		npc_diag = new NPC_Dialogo();
	}

	//Si colisionamos con el jugador, cargamos el nuevo texto
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			//Si se necesita pulsar el boton,activamos la variable waitfropress
			if (requiredButtonPress) 
			{
				waitForPress = true;
				return;
			}
			if (!TextBox.Instance.isActive)
				IniciaDialogo();
		}
	}

	//Al salir de la colision, desactivactivamos la variable waitforpress
	void OnTriggerExit(Collider other)
	{
		if(other.tag == "Player")
		{
			waitForPress = false;
		}
	}

	void Update()
	{
		//Si está esperando al input y pulsamos click derecho
		if (waitForPress && Input.GetMouseButtonDown(1) && !TextBox.Instance.isActive)
		{
			IniciaDialogo();
		}
	}

	//Inicia el dialogo
	private void IniciaDialogo()
	{
		TextBox.Instance.StartDialogue(this, npc_diag);
	}

	public void ActualizarDialogo(NPC_Dialogo dia)
	{
		npc_diag = dia;
		string _data = SerializeObject(npc_diag); 
		// This is the final resulting XML from the serialization process
		CreateXML(_data,"_SaveData/");
	}

	string SerializeObject(object pObject) 
	{ 
		string XmlizedString = null; 
		MemoryStream memoryStream = new MemoryStream(); 
		XmlSerializer xs = new XmlSerializer(typeof(NPC_Dialogo)); 
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

	void CreateXML(string _data, string ruta) 
	{ 
		// Where we want to save and load to and from 
		string _FileLocation=Application.persistentDataPath;

		StreamWriter writer; 
		Debug.Log(_FileLocation);
		FileInfo t = new FileInfo(_FileLocation+"\\"+ "file1.xml"); 
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
