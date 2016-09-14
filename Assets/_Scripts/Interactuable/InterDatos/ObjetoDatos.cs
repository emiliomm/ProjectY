using System.Xml.Serialization;
using System.Collections.Generic;

/*
 * 	Clase derivada de InterDatos que contiene datos especificos de un subtipo de Interactuables: los objetos
 *  Deriva de ObjetoSerializable ya que es una clase que se serializa en una cola con objetos de esta clase
 * 
 *  También un XmlRoot que modifica el nombre de la clase al serializarla, para que la serialización funcione correctamente
 */

[XmlRoot("ObjetoSerializable")]
public class ObjetoDatos : InterDatos{

	//Indica el nombre del objeto. A diferencia de los NPCs, los objetos
	//solo pueden tener un objeto
	public string nombre;

	//Indica el ID del transporte que acompaña al objeto, -1 si no tiene ninguno
	public int IDTransporte;

	public List<int> variables;

	public ObjetoDatos()
	{
		variables = new List<int>();
	}

	public override string DevuelveNombreActual()
	{
		return nombre;
	}

	public int DevuelveIDTransporte()
	{
		return IDTransporte;
	}

	public int DevuelveValorVariable(int num)
	{
		return variables[num];
	}

	public void SetValorVariable(int num, int valor)
	{
		variables[num] = valor;
	}

	//Devuelve el InterDatos de un xml indicado en la ruta
	public new static ObjetoDatos LoadInterDatos(string path)
	{
		ObjetoDatos interDatos = Manager.instance.DeserializeData<ObjetoDatos>(path);

		return interDatos;
	}
}
