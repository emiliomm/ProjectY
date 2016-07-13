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

	public int DevolverIDGrupo()
	{
		return idGrupo;
	}

	//Devuelve un grupo
	public static Grupo CreateGrupo(string path)
	{
		return Manager.Instance.DeserializeDataWithReturn<Grupo>(path);
	}

	//Carga un grupo, lo añade al manager y lanza los intros/mensajes de este
	public static void LoadGrupo(string path, int ID_NPC, int ID_DiagActual, int tipo_dialogo, ref int num_dialogo)
	{
		Grupo grup = Manager.Instance.DeserializeDataWithReturn<Grupo>(path);

		Lanzador.LoadLanzador(Manager.rutaLanzadores + grup.idGrupo.ToString() + ".xml", ID_NPC,ID_DiagActual, tipo_dialogo, ref num_dialogo);

		Manager.Instance.AddToGruposActivos(grup);
	}

	//Similar a la función anterior, pero pasándole el grupo
	//Carga un grupo, lo añade al manager y lanza los intros/mensajes de este
	public static void LoadGrupo(Grupo g, int ID_NPC, int ID_DiagActual, int tipo_dialogo, ref int num_dialogo)
	{
		Lanzador.LoadLanzador(Manager.rutaLanzadores + g.idGrupo.ToString() + ".xml", ID_NPC,ID_DiagActual, tipo_dialogo, ref num_dialogo);

		Manager.Instance.AddToGruposActivos(g);
	}

	//CUIDADO !!! ESTO SERIALIZA EL GRUPO EN EL DIRECTORIO DE GRUPOS MODIFICADOS. DE GUARDAR UN GRUPO EN LA LISTA DE GRUPOS ACTIVOS SE ENCARGA EL MANAGER
	public void AddToColaObjetos()
	{
		Manager.Instance.AddToColaObjetos(Manager.rutaGruposModificados + idGrupo.ToString()  + ".xml", this);
	}
}
