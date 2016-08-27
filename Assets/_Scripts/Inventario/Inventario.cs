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
		
	public void addObjeto(int ID, int cantidad)
	{
		int numObjeto = ObjetoInventarioLugar(ID);

		//Si no existe el objeto, lo añadimos
		if(numObjeto == -1)
		{
			ObjetoInventario obj = ObjetoInventario.LoadObjeto(Manager.rutaObjetoInventario + ID.ToString() + ".xml");
			obj.cantidad = cantidad;
			objetos.Add(obj);
			Manager.Instance.addObjetoReciente(obj, cantidad); //se añade también a la lista de objetos recientes
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
	private int ObjetoInventarioLugar(int ID)
	{
		return objetos.FindIndex(x => x.ID == ID);
	}

	public bool ObjetoInventarioExiste(int id)
	{
		return objetos.Any(x => x.ID == id);
	}

	public int devolverNumeroObjetos()
	{
		return objetos.Count;
	}

	public ObjetoInventario devolverObjeto(int indice)
	{
		return objetos[indice];
	}

	public void sustituyeObjeto(ObjetoInventario obj, int indice)
	{
		objetos[indice] = obj;
	}

	public int devolverID(int indice)
	{
		return objetos[indice].ID;
	}

	public string devolverNombre(int indice)
	{
		return objetos[indice].nombre;
	}

	public string devolverNombreImagen(int indice)
	{
		return objetos[indice].nombreImagen;
	}

	public string devolverDescripcion(int indice)
	{
		return objetos[indice].descripcion;
	}

	public int devolverCantidad(int indice)
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