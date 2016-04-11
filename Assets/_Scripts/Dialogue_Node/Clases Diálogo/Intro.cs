using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;

using DialogueTree;

//Implementa la interfaz System.IComparable para que funcione el metodo compareTo
//public class Intro : System.IComparable<Intro>
public class Intro{

	public int ID;
	public int IDGrupo; //-1 --> Sin grupo, otro --> con grupo
	public bool Autodestruye; // 0 --> falso, 1 --> verdadero
	public int prioridad;
	public int indice_inicial;
	public Dialogue dia;

	public Intro()
	{
		dia = new Dialogue();
	}

	public static Intro LoadIntro(string path, int prioridad)
	{
		XmlSerializer deserz = new XmlSerializer(typeof(Intro));
		StreamReader reader = new StreamReader(path);

		Intro intro = (Intro)deserz.Deserialize(reader);
		reader.Close();

		intro.prioridad = prioridad;

		return intro;
	}

	public Dialogue DevuelveDialogo()
	{
		return dia;
	}

	public int DevuelvePrioridad()
	{
		return prioridad;
	}

	public void MarcarRecorrido(int node_id)
	{
		dia.MarcarRecorrido(node_id);
	}
}
