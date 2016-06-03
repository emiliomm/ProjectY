using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using DialogueTree;

public class TemaMensaje{

	public int ID;
	//-1 si no tiene grupo, x si tiene. El grupo del TemaMensaje sustituye al grupo de los mensajes
	//contenidos en él, excepto si no tiene grupo
	public int IDGrupo;
	public string texto;
	public List<Mensaje> mensajes;

	public TemaMensaje()
	{
		mensajes = new List<Mensaje>();
	}

	public void AddMensaje(Mensaje m)
	{
		mensajes.Add(m);
	}

	public string DevuelveTexto()
	{
		return texto;
	}

	public int DevuelveNumeroMensajes()
	{
		return mensajes.Count;
	}

	public string DevuelveTextoMensaje(int num_mensaje)
	{
		return mensajes[num_mensaje].DevuelveTexto();
	}

	public static TemaMensaje LoadTemaMensaje(string path)
	{
		TemaMensaje temaMensaje = Manager.Instance.DeserializeDataWithReturn<TemaMensaje>(path);

		return temaMensaje;
	}

}
