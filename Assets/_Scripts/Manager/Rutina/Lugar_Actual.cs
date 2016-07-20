using UnityEngine;
using System.Collections;
using System;

public class Lugar_Actual {

	public Lugar lugarActual;
	public DateTime fechaAnyadido;

	public Lugar_Actual(Lugar l, DateTime f)
	{
		lugarActual = l;
		fechaAnyadido = f;
	}
}
