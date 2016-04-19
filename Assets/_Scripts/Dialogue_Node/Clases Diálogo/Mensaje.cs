using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;

using DialogueTree;

public class Mensaje{

	public int ID;
	public int IDGrupo; //-1 --> Sin grupo, otro --> con grupo
	public bool Autodestruye; // 0 --> falso, 1 --> verdadero
	public string texto;
	public Dialogue dia;

	public Mensaje()
	{
		dia = new Dialogue();
	}

	public static Mensaje LoadMensaje(string path)
	{
		Mensaje men = Manager.Instance.DeserializeDataWithReturn<Mensaje>(path);

		return men;
	}

	public Dialogue DevuelveDialogo()
	{
		return dia;
	}

	public string DevuelveTexto()
	{
		return texto;
	}

	public void MarcarRecorrido(int node_id)
	{
		dia.MarcarRecorrido(node_id);
	}

}
