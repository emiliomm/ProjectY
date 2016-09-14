using DialogueTree;
using UnityEngine;

using System.Collections.Generic;

/*
 * 	Clase que contiene los dialogos que se muestran al principio de las conversaciones
 */
public class Intro{

	public int ID;

	//-1 --> Sin grupo, otro --> con grupo
	public int IDGrupo;

	public List<DialogoEvento> eventos;

	// 0 --> falso, 1 --> verdadero
	//Indica si la intro se va a destruir al acabar de recorrerla
	private bool autodestruye;

	public int prioridad; //mayor prioridad, aparece primero en el diálogo
	public Dialogue dialogo;

	public Intro()
	{
		eventos = new List<DialogoEvento>();
		dialogo = new Dialogue();
	}

	public Dialogue DevuelveDialogo()
	{
		return dialogo;
	}

	public int DevuelvePrioridad()
	{
		return prioridad;
	}

	public int DevuelveIDGrupo()
	{
		return IDGrupo;
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

		return mostrar;
	}

	//Devuelve la intro de un xml indicado en la ruta con la prioridad indicada
	public static Intro LoadIntro(string path, int prioridad)
	{
		Intro intro = Manager.instance.DeserializeData<Intro>(path);

		intro.prioridad = prioridad;

		return intro;
	}
}
