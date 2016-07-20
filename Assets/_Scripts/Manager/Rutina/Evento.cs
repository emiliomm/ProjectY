using UnityEngine;
using System.Collections;

public class Evento {

	public Evento()
	{
		
	}

	public static Evento LoadEvento(string path)
	{
		Evento evento = Manager.Instance.DeserializeData<Evento>(path);

		return evento;
	}
}
