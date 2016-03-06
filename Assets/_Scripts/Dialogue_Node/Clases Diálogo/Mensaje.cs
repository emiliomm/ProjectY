using UnityEngine;
using System.Collections;

using DialogueTree;

public class Mensaje{

	public string texto;
	public Dialogue dia;

	public Mensaje()
	{
		dia = new Dialogue();
	}

	public Mensaje(string t, string d)
	{
		texto = t;
		dia = Dialogue.LoadDialogue (d);
	}

	public Dialogue DevuelveDialogo()
	{
		return dia;
	}

	public string DevuelveTexto()
	{
		return texto;
	}
}
