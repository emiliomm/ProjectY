using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

using DialogueTree;

public class Grupo{

	int idGrupo;
	public List<int> variables;

	public Grupo()
	{
		variables = new List<int>();
	}

	public static Grupo LoadGrupo(string path)
	{
		XmlSerializer deserz = new XmlSerializer(typeof(Grupo));
		StreamReader reader = new StreamReader(path);

		Grupo grup = (Grupo)deserz.Deserialize(reader);
		reader.Close();



		return grup;
	}

	private class Lanzador{
		public List<DialogueAddIntro> intros;
		public List<DialogueAddMensaje> mensajes;
		//public List<Grupo> grupos;

		public Lanzador()
		{
			intros = new List<DialogueAddIntro>();
			mensajes = new List<DialogueAddMensaje>();
			//grupos = new List<Grupo>();
		}

		public static Lanzador LoadLanzador(string path)
		{
			XmlSerializer deserz = new XmlSerializer(typeof(Lanzador));
			StreamReader reader = new StreamReader(path);

			Lanzador lanz = (Lanzador)deserz.Deserialize(reader);
			reader.Close();



			return lanz;
		}
	}
}
