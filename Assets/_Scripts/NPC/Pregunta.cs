using UnityEngine;
using System.Collections;

public class Pregunta{

	public string texto;
	public string dialogue_path;

	public Pregunta()
	{
		texto = "";
		dialogue_path= "";
	}

	public Pregunta(string t, string d)
	{
		texto = t;
		dialogue_path= d;
	}
}
