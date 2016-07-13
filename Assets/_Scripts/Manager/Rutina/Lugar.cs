using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lugar{

	public int Escena;
	public float coordX, coordY, coordZ;

	public List<Evento> eventos;

	public Lugar()
	{
		eventos = new List<Evento>();
	}

}
