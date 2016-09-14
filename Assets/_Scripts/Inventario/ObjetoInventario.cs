using UnityEngine;
using System.Collections;

public class ObjetoInventario{

	public int ID;
	public string nombre;
	public string nombreImagen;
	public string descripcion;

	public int cantidad;

	public ObjetoInventario()
	{
		
	}

	public static ObjetoInventario LoadObjeto(string path)
	{
		ObjetoInventario objetoInventario = Manager.instance.DeserializeData<ObjetoInventario>(path);

		return objetoInventario;
	}
}
