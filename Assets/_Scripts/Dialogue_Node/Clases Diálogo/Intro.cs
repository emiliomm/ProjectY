using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;

using DialogueTree;

public class Intro{

	public int ID;
	public int IDGrupo; //-1 --> Sin grupo, otro --> con grupo
	public bool Autodestruye; // 0 --> falso, 1 --> verdadero
	public int prioridad; //mayor prioridad, aparece antes
	public int indice_inicial;
	public Dialogue dia;

	public Intro()
	{
		dia = new Dialogue();
	}

	public Dialogue DevuelveDialogo()
	{
		return dia;
	}

	public int DevuelvePrioridad()
	{
		return prioridad;
	}

	public int DevuelveIDGrupo()
	{
		return IDGrupo;
	}

	public void MarcarRecorrido(int node_id)
	{
		dia.MarcarRecorrido(node_id);
	}

	public static Intro LoadIntro(string path, int prioridad)
	{
		Intro intro = Manager.Instance.DeserializeData<Intro>(path);

		intro.prioridad = prioridad;

		return intro;
	}
}
