using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;

using DialogueTree;

//Implementa la interfaz System.IComparable para que funcione el metodo compareTo
public class Intro : System.IComparable<Intro>{

	public int ID;
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

	//Método utilizado para la comparación entre elementos funcione con el método Sort() de una lista DialogoEntrante
	public int CompareTo(Intro otro)
	{
		if (otro == null) return 1;

		//orden descendente
		return -1 * prioridad.CompareTo(otro.prioridad);
	}

	public Dialogue DevuelveDialogo()
	{
		return dia;
	}

	public void MarcarRecorrido(int node_id)
	{
		dia.MarcarRecorrido(node_id);
	}
}
