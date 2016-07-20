using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lugar_Siguiente{
	
	public int IDRutina;

	public Lugar lugar;
	public List<int> eventos;

	public Lugar_Siguiente()
	{
		lugar = new Lugar();
		eventos = new List<int>();
	}
}