using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Xml; 
using System.Xml.Serialization; 

[XmlRoot("ObjetoSerializable")]
public class Inventario : ObjetoSerializable
{
	public List<ObjetoInventario> objetos;

	public Inventario()
	{
		objetos = new List<ObjetoInventario>();
	}
		
	public void AddObjeto(int IDObjeto, int cantidad)
	{
		int numObjeto = ObjetoInventarioLugar(IDObjeto);

		//Si no existe el objeto, lo añadimos
		if(numObjeto == -1)
		{
			ObjetoInventario objetoInventario = ObjetoInventario.LoadObjeto(Manager.rutaObjetoInventario + IDObjeto.ToString() + ".xml");
			objetoInventario.cantidad = cantidad;
			objetos.Add(objetoInventario);
			Manager.Instance.addObjetoReciente(objetoInventario, cantidad); //se añade también a la lista de objetos recientes
		}
		//Si existe, aumentamos en 1 la cantidad
		else
		{
			objetos[numObjeto].cantidad += cantidad;
			Manager.Instance.addObjetoReciente(objetos[numObjeto], cantidad); //se añade también a la lista de objetos recientes
		}
	}

	//Devuelve el lugar en la lista de objetos del objeto con el ID
	//-1 si no existe
	private int ObjetoInventarioLugar(int IDObjeto)
	{
		return objetos.FindIndex(x => x.ID == IDObjeto);
	}

	public bool ObjetoInventarioExiste(int IDObjeto)
	{
		return objetos.Any(x => x.ID == IDObjeto);
	}

	public int DevolverNumeroObjetos()
	{
		return objetos.Count;
	}

	public ObjetoInventario DevolverObjeto(int indice)
	{
		return objetos[indice];
	}

	public void SustituyeObjeto(ObjetoInventario objetoInventario, int indice)
	{
		objetos[indice] = objetoInventario;
	}

	public int DevolverID(int indice)
	{
		return objetos[indice].ID;
	}

	public string DevolverNombre(int indice)
	{
		return objetos[indice].nombre;
	}

	public string DevolverNombreImagen(int indice)
	{
		return objetos[indice].nombreImagen;
	}

	public string DevolverDescripcion(int indice)
	{
		return objetos[indice].descripcion;
	}

	public int DevolverCantidad(int indice)
	{
		return objetos[indice].cantidad;
	}

	public static Inventario LoadInventario(string path)
	{
		Inventario inventario = Manager.Instance.DeserializeData<Inventario>(path);

		return inventario;
	}

	public void AddToColaObjetos()
	{
		Manager.Instance.AddToColaObjetos(Manager.rutaInventario + "Inventario.xml", this);
	}
}