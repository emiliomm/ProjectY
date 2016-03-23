using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text; 

using DialogueTree;

public class NPC_Dialogo{
	
	public int ID;
	public List<Intro> intros;
	public List<Mensaje> mensajes;

	//HACER VARIABLES GLOBALES
	private static string _FileLocation;
	private static string rutaIntros;
	private static string rutaMensajes;
	private static string DefaultDialogs;

	public NPC_Dialogo()
	{
		_FileLocation = Application.persistentDataPath + "/NPC_Dialogo_Saves/";
		rutaIntros = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLIntros/";
		rutaMensajes = Application.dataPath + "/StreamingAssets/XMLDialogue/XMLMensajes/";
		DefaultDialogs = Application.dataPath + "/StreamingAssets/NPCDialogue/";

		intros = new List<Intro>();
		mensajes = new List<Mensaje>();
//		Prueba();
	}

	//OBSOLETA
	private void Prueba()
	{
//		Intro d = new Intro("text_dia.xml");
//		Intro d2 = new Intro("text_dia2.xml");
//
//		AnyadirIntro (d);
//		AnyadirIntro (d2);
//
//		for(int i = 0; i < 5; i++)
//		{
//			mensajes.Add(new Mensaje("Opcion " + i.ToString(),rutaDialogos + "text_dia3.xml"));
//		}
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
		if (num_intro + 1 < this.DevuelveNumeroIntros())
			avanza = true;

		return avanza;
	}

	public void MarcaDialogueNodeComoLeido(int tipo, int num_dialogo, int node_id)
	{
		Dialogue d;
		DialogueNode dn;

		switch(tipo)
		{
		case 0:
			d = this.DevuelveDialogoIntro(num_dialogo);
			dn = d.DevuelveNodo(node_id);

			if(dn.DevuelveRecorrido() != true)
			{
				intros[num_dialogo].MarcarRecorrido(node_id);
				AnyadirDialogueAdd(dn);
			}
			break;
		case 1:
			d = this.DevuelveDialogoMensajes(num_dialogo);
			dn = d.DevuelveNodo(node_id);

			if(dn.DevuelveRecorrido() != true)
			{
				mensajes[num_dialogo].MarcarRecorrido(node_id);
				AnyadirDialogueAdd(dn);
			}
			break;
		}
	}

	public void MirarSiDialogoSeAutodestruye(int tipo, ref int num_dialogo)
	{
		switch(tipo)
		{
		case 0:
			if(intros [num_dialogo].Autodestruye == true)
			{
				intros.RemoveAt(num_dialogo);
				num_dialogo--;
			}
			break;
		case 1:
			if(mensajes [num_dialogo].Autodestruye == true)
			{
				mensajes.RemoveAt(num_dialogo);
				num_dialogo--;
			}
			break;
		}
	}

	private void AnyadirDialogueAdd(DialogueNode node)
	{
		for(int i = 0; i < node.DevuelveNumeroIntros(); i++)
		{
			int prioridad = node.Intros[i].DevuelvePrioridad();
			int ID = node.Intros[i].DevuelveID();
			int IDNpc = node.Intros[i].DevuelveIDNpc();

			if(IDNpc == -1)
			{
				AnyadirIntro(Intro.LoadIntro(rutaIntros + ID.ToString() + ".xml", prioridad));
			}
			else
			{
				//Buscamos en el diccionario y lo añadimos
				//si no esta en el diccionario, lo añadimos desde el xml
				GameObject gobj = Manager.Instance.GetGameObject(IDNpc);

				if(gobj != null)
				{
					NPC npc = gobj.GetComponent<NPC>() as NPC;
					npc.npc_diag.AnyadirIntro(Intro.LoadIntro(rutaIntros + ID.ToString() + ".xml", prioridad));
				}
				else
				{
					NPC_Dialogo npc_diag;

					//Cargamos el dialogo
					//Si existe un fichero guardado, cargamos ese fichero, sino
					//cargamos el fichero por defecto
					if (System.IO.File.Exists(_FileLocation + IDNpc.ToString()  + ".xml"))
					{
						npc_diag = NPC_Dialogo.LoadNPCDialogue(IDNpc, _FileLocation + IDNpc.ToString()  + ".xml");
					}
					else
					{
						npc_diag = NPC_Dialogo.LoadNPCDialogue(IDNpc, DefaultDialogs + IDNpc.ToString()  + ".xml");
					}

					npc_diag.AnyadirIntro(Intro.LoadIntro(rutaIntros + ID.ToString() + ".xml", prioridad));
					npc_diag.SerializeToXml();
				}
			}
		}

		for(int i = 0; i < node.DevuelveNumeroMensajes(); i++)
		{
			int ID = node.Mensajes[i].DevuelveID();
			int IDNpc = node.Mensajes[i].DevuelveIDNpc();

			if(IDNpc == -1)
			{
				AnyadirMensaje(Mensaje.LoadMensaje(rutaMensajes + ID.ToString() + ".xml"));
			}
			else
			{
				//Buscamos en el diccionario y lo añadimos
				//si no esta en el diccionario, lo añadimos desde el xml
				GameObject gobj = Manager.Instance.GetGameObject(IDNpc);

				if(gobj != null)
				{
					NPC npc = gobj.GetComponent<NPC>() as NPC;
					npc.npc_diag.AnyadirMensaje(Mensaje.LoadMensaje(rutaMensajes + ID.ToString() + ".xml"));
				}
				else
				{
					NPC_Dialogo npc_diag;

					//Cargamos el dialogo
					//Si existe un fichero guardado, cargamos ese fichero, sino
					//cargamos el fichero por defecto
					if (System.IO.File.Exists(_FileLocation + IDNpc.ToString()  + ".xml"))
					{
						npc_diag = NPC_Dialogo.LoadNPCDialogue(IDNpc, _FileLocation + IDNpc.ToString()  + ".xml");
					}
					else
					{
						npc_diag = NPC_Dialogo.LoadNPCDialogue(IDNpc, DefaultDialogs + IDNpc.ToString()  + ".xml");
					}

					npc_diag.AnyadirMensaje(Mensaje.LoadMensaje(rutaMensajes + ID.ToString() + ".xml"));
					npc_diag.SerializeToXml();
				}
			}
		}
	}

	public void AnyadirIntro(Intro d)
	{
		intros.Add (d);
		intros.Sort ();
	}

	public void AnyadirMensaje(Mensaje m)
	{
		mensajes.Add(m);
	}

	public static NPC_Dialogo LoadNPCDialogue(int id, string path)
	{
		XmlSerializer deserz = new XmlSerializer(typeof(NPC_Dialogo));
		StreamReader reader = new StreamReader(path);

		NPC_Dialogo npc_dialogo = (NPC_Dialogo)deserz.Deserialize(reader);
		reader.Close();

		npc_dialogo.ID = id;

		return npc_dialogo;
	}

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
		
	void CreateXML(string _data) 
	{
		StreamWriter writer; 

		//check if directory doesn't exit
		if(!Directory.Exists(_FileLocation))
		{    
			//if it doesn't, create it
			Directory.CreateDirectory(_FileLocation);
		}

		FileInfo t = new FileInfo(_FileLocation + ID.ToString()  + ".xml"); 

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
