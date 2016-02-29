using UnityEngine;
using System.Collections;

using DialogueTree;

public class Mensaje{

	public string texto;
	public Dialogue dia;

	public Mensaje()
	{
		texto = "";
		dia = null;
	}

	public Mensaje(string t, string d)
	{
		texto = t;
		dia = Dialogue.LoadDialogue ("Assets/_Texts/" + d);
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
