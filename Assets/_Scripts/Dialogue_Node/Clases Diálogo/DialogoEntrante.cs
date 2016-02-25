using UnityEngine;
using System.Collections;

using DialogueTree;

//Implementa la interfaz System.IComparable para que funcione el metodo compareTo
public class DialogoEntrante : System.IComparable<DialogoEntrante>{

	public Dialogue dia;
	public int prioridad;
	public int indice_inicial;

	public DialogoEntrante()
	{
		dia = null;
	}

	public DialogoEntrante(string d)
	{
		dia = Dialogue.LoadDialogue ("Assets/_Texts/" + d);
		prioridad = 1;
		indice_inicial = 0;
	}

	//Método utilizado para la comparación entre elementos funcione con el método Sort() de una lista DialogoEntrante
	public int CompareTo(DialogoEntrante otro)
	{
		if (otro == null) return 1;

		//orden descendente
		return -1 * prioridad.CompareTo(otro.prioridad);
	}
}
