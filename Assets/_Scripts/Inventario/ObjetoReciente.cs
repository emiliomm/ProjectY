using UnityEngine;
using System.Collections;

public class ObjetoReciente
{
	private ObjetoInventario objeto;
	private int cantidad;

	public ObjetoReciente(ObjetoInventario objetoInventario, int cantidad)
	{
		objeto = objetoInventario;
		this.cantidad = cantidad;
	}

	public ObjetoInventario DevuelveObjeto()
	{
		return objeto;
	}

	public int DevuelveCantidad()
	{
		return cantidad;
	}
}
