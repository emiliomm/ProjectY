using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

using System.Xml; 
using System.Text; 

using DialogueTree;

[XmlRoot("ObjetoSer")]
public class Grupo : ObjetoSer{
	
	public int idGrupo;
	public List<int> variables;

	public Grupo()
	{
		variables = new List<int>();
	}

	//Devuelve un grupo
	public static Grupo CreateGrupo(string path)
	{
		return Manager.Instance.DeserializeDataWithReturn<Grupo>(path);
	}

	//Carga un grupo, lo añade al manager y lanza los intros/mensajes de este
	public static Grupo LoadGrupo(string path, int ID_NPC, int tipo_dialogo, ref int num_dialogo)
	{
		Grupo grup = Manager.Instance.DeserializeDataWithReturn<Grupo>(path);

		Lanzador.LoadLanzador(Manager.rutaLanzadores + grup.idGrupo.ToString() + ".xml", ID_NPC, tipo_dialogo, ref num_dialogo);

		Manager.Instance.AddToGruposActivos(grup);

		return grup;
	}

	public int DevolverGrupoID()
	{
		return idGrupo;
	}
		
	//CUIDADO !!! ESTO SERIALIZA A LA CARPETA DE GRUPOS MODIFICADOS, NO USAR PARA GUARDAR LA LISTA DE GRUPOS
	public void Serialize()
	{
		Manager.Instance.SerializeData(this, Manager.rutaGruposModificados, Manager.rutaGruposModificados + idGrupo.ToString()  + ".xml");
	}

	public void AddToColaObjetos()
	{
		Manager.Instance.AddToColaObjetos(Manager.rutaGruposModificados + idGrupo.ToString()  + ".xml", this);
	}
}
