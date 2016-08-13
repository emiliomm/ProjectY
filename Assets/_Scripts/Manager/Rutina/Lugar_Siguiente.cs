using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Lugar_Siguiente{
	
	private DateTime fechaRutina; //no se serializa

	public Lugar lugar;
	public List<int> eventos;

	public Lugar_Siguiente()
	{
		lugar = new Lugar();
		eventos = new List<int>();
	}

	public void setFechaRutina(DateTime nuevaFecha)
	{
		fechaRutina = nuevaFecha;
	}

	public DateTime getFechaRutina()
	{
		return fechaRutina;
	}
}