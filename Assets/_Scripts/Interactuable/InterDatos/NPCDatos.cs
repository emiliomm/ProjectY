using UnityEngine;

using System.Collections.Generic;
using System.Xml.Serialization;

/*
 * 	Clase derivada de InterDatos que contiene datos especificos de un subtipo de Interactuables: los NPC
 *  Deriva de ObjetoSerializable ya que es una clase que se serializa en una cola con objetos de esta clase
 * 
 *  También un XmlRoot que modifica el nombre de la clase al serializarla, para que la serialización funcione correctamente
 */

[XmlRoot("ObjetoSerializable")]
public class NPCDatos : InterDatos{

	//El indiceNombre que señala el nombre actual en la lista de nombres
	public int indiceNombre;
	public List<string> nombres;

	public NPCDatos()
	{
		nombres = new List<string>();
	}

	//Devuelve el nombre actual en base al indiceNombre
	public override string DevuelveNombreActual()
	{
		return nombres[indiceNombre];
	}

	public void SetIndiceNombre(int indice)
	{
		indiceNombre = indice;
	}
		
	public int DevuelveIndiceNombre()
	{
		return indiceNombre;
	}

	//Devuelve el NPCDatos de un xml indicado en la ruta
	public new static NPCDatos LoadInterDatos(string path)
	{
		NPCDatos interDatos = Manager.instance.DeserializeData<NPCDatos>(path);

		return interDatos;
	}
}
