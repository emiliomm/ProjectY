using UnityEngine;
using System.Collections;

using System.Collections.Generic;

using System.Xml.Serialization;

[XmlInclude(typeof(MensajeDialogo))]
[XmlInclude(typeof(MensajeTienda))]
public class Mensaje{

	public int ID;

	//-1 --> Sin grupo, otro --> con grupo
	public int IDGrupo;

	public List<DialogoEvento> eventos;

	// 0 --> falso, 1 --> verdadero
	//Indica si el mensaje se va a destruir al acabar de recorrerlo
	protected bool autodestruye;

	public string texto;

	protected bool visible;

	public Mensaje()
	{
		eventos = new List<DialogoEvento>();
	}

	public int DevuelveIDGrupo()
	{
		return IDGrupo;
	}

	public string DevuelveTexto()
	{
		return texto;
	}

	public bool DevuelveAutodestruye()
	{
		return autodestruye;
	}

	public void ActivarAutodestruye()
	{
		autodestruye = true;
	}

	public bool SeMuestra()
	{
		bool mostrar = true;

		for(int i = 0; i < eventos.Count; i++)
		{
			if(!eventos[i].EstaActivo())
				mostrar = false;
		}

		visible = mostrar;

		return mostrar;
	}

	public bool EstadoVisible()
	{
		return visible;
	}

	//Devuelve el mensaje de un xml indicado en la ruta
	public static Mensaje LoadMensaje(string path)
	{
		Mensaje men = Manager.Instance.DeserializeData<Mensaje>(path);

		return men;
	}
}
