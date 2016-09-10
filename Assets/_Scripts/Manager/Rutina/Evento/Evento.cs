using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

//AÑADIR EVENTO QUE COMPRUEBE VARIABLES DE OBJETO SI EL INTER QUIERE CAMBIAR RUTINA O HACER ALGO

[XmlInclude(typeof(EventoDialogo))]
public class Evento
{

	public int ID;
	public bool activo; //ACTUALMENTE NO SE USA
	public List<int> variables;

	public Evento()
	{
		variables = new List<int>();
	}

	//Método virtual usado por las clases derivadas que se ejecuta al comprobar eventos en la rutina
	public virtual void EjecutarEvento(){  }

	public static Evento LoadEvento(string path)
	{
		Evento evento = Manager.Instance.DeserializeData<Evento>(path);

		return evento;
	}

	public void Serialize()
	{
		Manager.Instance.SerializeData(this, Manager.rutaEventosGuardados, ID.ToString()  + ".xml");
	}
}
