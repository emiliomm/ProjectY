using UnityEngine;
using System.Collections;
using System;

public class Lugar_Actual {

	private Lugar lugarActual;
	private DateTime fechaAnyadido;

	public Lugar_Actual(Lugar l, DateTime f)
	{
		lugarActual = l;
		fechaAnyadido = f;
	}

	public DateTime getFecha()
	{
		return fechaAnyadido;
	}

	public int getIDInter()
	{
		return lugarActual.IDInter;
	}

	public Vector3 getCoordLugar()
	{
		return new Vector3(lugarActual.coordX, lugarActual.coordY, lugarActual.coordZ);
	}
}
