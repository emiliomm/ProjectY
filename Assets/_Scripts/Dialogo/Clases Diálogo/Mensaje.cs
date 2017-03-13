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

	//Los eventos que determinan si se muestra o no el mensaje
	public List<DialogoEvento> eventos;

	// 0 --> falso, 1 --> verdadero
	//Indica si el mensaje se va a destruir al acabar de recorrerlo
	protected bool autodestruye;

	public string texto;

	//Guarda el estado del mensaje (si se muestra o no), según las variables de los posibles eventos asignados
	//Para usar esta variable, se tiene que haber usado la función SeMuestra() en el mismo ciclo sabiendo que
	//entre la utilización de la función y el uso de esta variable los eventos y sus variables no pueden cambiar
	//(Como una pequeña cache usada para no tener que recorrer los eventos en el mismo ciclo)
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
			if(!eventos[i].SeCumplenCondiciones())
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
		Mensaje men = Manager.instance.DeserializeData<Mensaje>(path);

		return men;
	}
}
