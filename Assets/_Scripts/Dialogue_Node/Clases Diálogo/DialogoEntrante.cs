using UnityEngine;
using System.Collections;

using DialogueTree;

public class DialogoEntrante{

	public Dialogue dia;
	public int prioridad;
	public int indice_inicial { get; set; }

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

	// Default comparer for DialogueEntrante type
	//Para el metodo Sort de List
	public int CompareTo(DialogoEntrante comparePart)
	{
		// A null value means that this object is greater.
		if (DialogoEntrante == null)
			return 1;

		else
			return this.indice_inicial.CompareTo(DialogoEntrante.indice_inicial);
	}
}
