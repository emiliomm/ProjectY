using System.Xml.Serialization;

/*
 * 	Clase base para el almacenamiento de datos sobre los subtipos de interactuables.
 *  Deriva de ObjetoSer ya que es una clase que se serializa en una cola con objetos de esta clase
 * 
 *  Se incluye un XmlInclude de las clases derivadas para la correcta serizalización/deserialización
 *  También un XmlRoot que modifica el nombre de la clase al serializarla, para que la serialización funcione correctamente
 */

[XmlRoot("ObjetoSerializable")]
[XmlInclude(typeof(NPCDatos))]
[XmlInclude(typeof(ObjetoDatos))]
public class InterDatos : ObjetoSerializable{
	
	public int ID;

	public InterDatos()
	{
		
	}

	//Método virtual utilizado por las clases derivadas para devolver el nombre del interactuable
	public virtual string DevuelveNombreActual()
	{
		return "";
	}

	//Devuelve el InterDatos de un xml indicado en la ruta
	public static InterDatos LoadInterDatos(string path)
	{
		InterDatos inter_datos = Manager.Instance.DeserializeData<InterDatos>(path);

		return inter_datos;
	}

	public void Serialize()
	{
		Manager.Instance.SerializeData(this, Manager.rutaNPCDatosGuardados, ID.ToString()  + ".xml");
	}

	//Añade los datos a la cola de objetos serializables
	public void AddToColaObjetos()
	{
		Manager.Instance.AddToColaObjetos(Manager.rutaNPCDatosGuardados + ID.ToString()  + ".xml", this);
	}
}
