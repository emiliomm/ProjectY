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

	//Devuelve true si el objeto ha sido añadido
	//false si no lo ha sido
	public bool addObjeto(int ID)
	{
		bool anyadido = false;

		//Si no existe el objeto, lo añadimos
		if(!ObjetoInventarioExiste(ID))
		{
			ObjetoInventario obj = ObjetoInventario.LoadObjeto(Manager.rutaObjetoInventario + ID.ToString() + ".xml");
			objetos.Add(obj);
			Manager.Instance.addObjetoReciente(obj); //se añade también a la lista de objetos recientes

			anyadido = true;
		}

		return anyadido;
	}

	public bool ObjetoInventarioExiste(int id)
	{
		return objetos.Any(x => x.ID == id);
	}

	public int devolverNumeroObjetos()
	{
		return objetos.Count;
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