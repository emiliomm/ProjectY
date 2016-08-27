using UnityEngine;
using System.Collections;

public class ObjetoReciente
{
	private ObjetoInventario objeto;
	private int cantidad;

	public ObjetoReciente(ObjetoInventario obj, int cant)
	{
		objeto = obj;
		cantidad = cant;
	}

	public ObjetoInventario devuelveObjeto()
	{
		return objeto;
	}

	public int devuelveCantidad()
	{
		return cantidad;
	}
}
