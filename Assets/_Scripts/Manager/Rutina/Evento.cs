using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Evento {

	public int ID;
	public List<int> variables;

	public Evento()
	{
		variables = new List<int>();
	}

	public static Evento LoadEvento(string path)
	{
		Evento evento = Manager.Instance.DeserializeData<Evento>(path);

		return evento;
	}
}
