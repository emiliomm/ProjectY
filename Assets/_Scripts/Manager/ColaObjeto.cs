using UnityEngine;
using System.Collections;

using System;

public class ColaObjeto{
	private ObjetoSer objeto;
	private string ruta;

	public ColaObjeto(ObjetoSer obj, string r) 
	{
		objeto = obj;
		ruta = r;
	}

	public ObjetoSer GetObjeto()
	{
		return objeto;
	}

	public string GetRuta()
	{
		return ruta;
	}
}
