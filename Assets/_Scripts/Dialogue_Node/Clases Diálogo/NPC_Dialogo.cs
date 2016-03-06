using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text; 

using DialogueTree;

public class NPC_Dialogo{

	public List<Intro> intros;
	public List<Mensaje> mensajes;

	private static string _FileLocation;
	private static string rutaDialogos;

	public NPC_Dialogo()
	{
		_FileLocation = Application.persistentDataPath + "/NPC_Dialogo_Saves/";
		rutaDialogos = Application.dataPath + "/StreamingAssets/XMLDialogue/";

		intros = new List<Intro>();
		mensajes = new List<Mensaje>();
//		Prueba();
	}

	private void Prueba()
	{
		Intro d = new Intro("text_dia.xml");
		Intro d2 = new Intro("text_dia2.xml");

		AnyadirIntro (d);
		AnyadirIntro (d2);

		for(int i = 0; i < 5; i++)
		{
			mensajes.Add(new Mensaje("Opcion " + i.ToString(),rutaDialogos + "text_dia3.xml"));
		}
	}

	public void AnyadirIntro(Intro d)
	{
		intros.Add (d);
		intros.Sort ();
	}

	public int DevuelveNumeroIntros()
	{
		return intros.Count;
	}

	public int DevuelveNumeroMensajes()
	{
		return mensajes.Count;
	}

	public Dialogue DevuelveDialogoIntro(int num_intro)
	{
		return intros[num_intro].DevuelveDialogo();
	}

	public Dialogue DevuelveDialogoMensajes(int num_mensaje)
	{
		return mensajes[num_mensaje].DevuelveDialogo();
	}

	public string DevuelveTextoMensaje(int num_mensaje)
	{
		return mensajes[num_mensaje].DevuelveTexto();
	}

	public bool AvanzaIntro(int num_intro)
	{
		bool avanza = false;
		if (num_intro + 1 < intros.Count)
			avanza = true;

		return avanza;
	}

	public void MarcaDialogueNodeComoLeido(int tipo, int num_dialogo, int node_id)
	{
		switch(tipo)
		{
		case 0:
			if(intros [num_dialogo].dia.Nodes [node_id].recorrido != true)
			{
				intros [num_dialogo].dia.Nodes [node_id].recorrido = true;
				AnyadirDialogueAdd(intros [num_dialogo].dia.Nodes [node_id]);
			}
			break;
		case 1:
			if(mensajes [num_dialogo].dia.Nodes [node_id].recorrido != true)
			{
				mensajes [num_dialogo].dia.Nodes [node_id].recorrido = true;
				AnyadirDialogueAdd(mensajes [num_dialogo].dia.Nodes [node_id]);
			}
			break;
		}
	}

	public void MirarSiDialogoSeAutodestruye(int tipo, ref int num_dialogo)
	{
		switch(tipo)
		{
		case 0:
			if(intros [num_dialogo].dia.Autodestruye == true)
			{
				intros.RemoveAt(num_dialogo);
				num_dialogo--;
			}
			break;
		case 1:
			if(mensajes [num_dialogo].dia.Autodestruye == true)
			{
				mensajes.RemoveAt(num_dialogo);
				num_dialogo--;
			}
			break;
		}
	}

	private void AnyadirDialogueAdd(DialogueNode node)
	{
		for(int i = 0; i < node.AddIntro.Count; i++)
		{
			string nombreTexto = node.AddIntro[i].DevuelveNombre();
			int prioridad = node.AddIntro[i].DevuelvePrioridad();

			AnyadirIntro(new Intro(prioridad, rutaDialogos + nombreTexto));
		}

		for(int i = 0; i < node.AddMensaje.Count; i++)
		{
			string mensaje = node.AddMensaje[i].DevuelveMensaje();
			string nombreTexto = node.AddMensaje[i].DevuelveNombre();

			mensajes.Add(new Mensaje(mensaje,rutaDialogos + nombreTexto));
		}
	}

	public static NPC_Dialogo LoadNPCDialogue(string path)
	{
		XmlSerializer deserz = new XmlSerializer(typeof(NPC_Dialogo));
		StreamReader reader = new StreamReader(path);

		NPC_Dialogo npc_dialogo = (NPC_Dialogo)deserz.Deserialize(reader);
		reader.Close();

		return npc_dialogo;
	}

	public void SerializeToXml(int id_npc)
	{
		string _data = SerializeObject(this); 
		// This is the final resulting XML from the serialization process
		CreateXML(_data, id_npc);
	}

	//Serializa el objeto en xml que se le pasa
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
		
	void CreateXML(string _data, int id_npc) 
	{
		StreamWriter writer; 

		//check if directory doesn't exit
		if(!Directory.Exists(_FileLocation))
		{    
			//if it doesn't, create it
			Directory.CreateDirectory(_FileLocation);
		}

		FileInfo t = new FileInfo(_FileLocation + id_npc.ToString()  + ".xml"); 

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
//
//	public void AddDialogoEntrante(int num_dialog, int node_id)
//	{
//		DialogueNode dn = DevuelveNodoDialogoDialogo(num_dialog, node_id);
//		dn.recorrido = true;
//
//		for(int i = 0; i < dn.AddDialogo.Count; i++)
//		{
//			DialogoEntrante de = new DialogoEntrante(dn.AddDialogo[i].DevuelveNombre());
//			AnyadirDialogo (de);
//		}
//	}
//
//	public void AddPreguntaEntrante(int num_dialog, int node_id)
//	{
//		DialogueNode dn = DevuelveNodoDialogoDialogo(num_dialog, node_id);
//		dn.recorrido = true;
//
//		for(int i = 0; i < dn.AddPregunta.Count; i++)
//		{
//			preguntas.Add(new Pregunta("Opcion " + i.ToString(), dn.AddPregunta[i].DevuelveNombre()));
//		}
//	}
//
//	public void AddDialogoRespuestas(int num_dialog, int node_id)
//	{
//		DialogueNode dn = DevuelveNodoDialogoPregunta(num_dialog, node_id);
//		dn.recorrido = true;
//
//		for(int i = 0; i < dn.AddDialogo.Count; i++)
//		{
//			DialogoEntrante de = new DialogoEntrante(dn.AddDialogo[i].DevuelveNombre());
//			AnyadirDialogo (de);
//		}
//	}
//
//	public void AddPreguntaRespuestas(int num_dialog, int node_id)
//	{
//		DialogueNode dn = DevuelveNodoDialogoPregunta(num_dialog, node_id);
//		dn.recorrido = true;
//
//		for(int i = 0; i < dn.AddPregunta.Count; i++)
//		{
//			preguntas.Add(new Pregunta("Opcion " + i.ToString(), dn.AddPregunta[i].DevuelveNombre()));
//		}
//	}
}
