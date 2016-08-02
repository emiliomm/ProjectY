using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventario{

	public List<ObjetoInventario> objetos;

	public Inventario()
	{
		objetos = new List<ObjetoInventario>();
	}

	public static Inventario LoadInventario(string path)
	{
		Inventario inventario = Manager.Instance.DeserializeData<Inventario>(path);

		return inventario;
	}
}