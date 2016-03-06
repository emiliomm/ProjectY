using UnityEngine;
using System.Collections;

using DialogueTree;

//Implementa la interfaz System.IComparable para que funcione el metodo compareTo
public class Intro : System.IComparable<Intro>{

	public int prioridad;
	public int indice_inicial;
	public Dialogue dia;

	public Intro()
	{
		dia = new Dialogue();
	}

	public Intro(string d)
	{
		dia = Dialogue.LoadDialogue (d);
		prioridad = 1;
		indice_inicial = 0;
	}

	public Intro(int prior, string nombreDialogo)
	{
		dia = Dialogue.LoadDialogue (nombreDialogo);
		prioridad = prior;
		indice_inicial = 0;
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
}
