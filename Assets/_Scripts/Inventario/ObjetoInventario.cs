using UnityEngine;
using System.Collections;

public class ObjetoInventario{

	public int ID;
	public string nombre;
	public string nombreImagen;
	public string descripcion;

	public ObjetoInventario()
	{
		
	}

	public static ObjetoInventario LoadObjeto(string path)
	{
		ObjetoInventario objeto = Manager.Instance.DeserializeData<ObjetoInventario>(path);

		return objeto;
	}
}
