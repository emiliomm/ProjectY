using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventoLista{

	private List<int> inter;
	private Evento evento;

	public EventoLista(int IDInterInicial, Evento ev)
	{
		inter = new List<int>();
		inter.Add(IDInterInicial);

		evento = ev;
	}

	public Evento devuelveEvento()
	{
		return evento;
	}

	public void addInter(int IDInter)
	{
		if(!interExiste(IDInter))
		{
			inter.Add(IDInter);
		}
	}

	public int posicionInter(int IDInter)
	{
		return inter.IndexOf(IDInter);
	}

	public void borrarInter(int pos)
	{
		inter.RemoveAt(pos);
	}

	public bool isInterEmpty()
	{
		return inter.Count == 0;
	}

	private bool interExiste(int IDInter)
	{
		bool existe = posicionInter(IDInter) != -1;

		return existe;
	}
}
