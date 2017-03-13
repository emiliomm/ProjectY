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

	//Los eventos que determinan si se muestra o no la intro
	public List<DialogoEvento> eventos;

	public bool aDistancia;
	public int tamX;
	public int tamY;
	public int tamZ;

	// 0 --> falso, 1 --> verdadero
	//Indica si la intro se va a destruir al acabar de recorrerla
	//También indica si una intro ha sido eliminada de un dialogo, para destruir el DialogoDistancia
	//asignado a la intro en el caso de que lo hubiera
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

	public bool DevuelveADistancia()
	{
		return aDistancia;
	}

	public Vector3 DevuelveTamanyoDialogoDistancia()
	{
		return new Vector3(tamX, tamY, tamZ);
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
