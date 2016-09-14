using UnityEngine;
using System.Collections;
using System;

public class LugarActual {

	private Lugar lugarActual;
	private DateTime fechaAnyadido;

	public LugarActual(Lugar lugar, DateTime fecha)
	{
		this.lugarActual = lugar;
		this.fechaAnyadido = fecha;
	}

	public DateTime GetFecha()
	{
		return fechaAnyadido;
	}

	public int GetIDInteractuable()
	{
		return lugarActual.IDInteractuable;
	}

	public Vector3 GetCoordenadasLugar()
	{
		return new Vector3(lugarActual.coordX, lugarActual.coordY, lugarActual.coordZ);
	}

	public Quaternion GetCoordenadasRotacion()
	{
		return new Quaternion(lugarActual.rotX, lugarActual.rotY, lugarActual.rotZ, lugarActual.rotW);
	}
}
