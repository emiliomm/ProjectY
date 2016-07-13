using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rutina{

	public int ID;
	public int ID_Inter;

	//0 = Tipo NPC
	//1 = Tipo Objeto
	public int tipo_Inter;

	public List<Lugar> lugares;

	public List<Evento> eventos;

	public int RutinaSiguiente;

	public Rutina()
	{
		lugares = new List<Lugar>();
		eventos = new List<Evento>();
	}
}
