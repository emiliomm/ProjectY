using System.Collections.Generic;
using System.Xml.Serialization;

/*
 * 	Clase que contiene un grupo y sus variables. Es usado para comprobar si está "activo". Cuando es activado lanza intros/mensajes a determinados diálogos
 *  Se mantiene en memoria en una lista y se usa para controlar determinados parámetros de un diálogo
 * 
 *  Es una clase derivada de ObjetoSerializable, que permite que esta clase sea añadida a una cola con objetos a serializar
 *  También se reescribe el nombre a ObjetoSer al ser serializado en un xml para que funcione la serialización/deserialización de objetos de clase ObjetoSer
 */

[XmlRoot("ObjetoSerializable")]
public class Grupo : ObjetoSerializable{
	
	public int IDGrupo; //ID que identifica al grupo
	public List<int> variables; //lista de variables con valores enteros

	public Grupo()
	{
		variables = new List<int>();
	}

	public int DevolverIDGrupo()
	{
		return IDGrupo;
	}

	//Lee un grupo de un archivo xml en la ruta indicada en el path y lo devuelve
	public static Grupo CreateGrupo(string path)
	{
		return Manager.instance.DeserializeData<Grupo>(path);
	}
		
	/*
 	 *  Lee un grupo de un archivo xml en la ruta indicada en el path
 	 *  Lo añade a la lista de grupos activos de la clase Manager
 	 *  Añade los intros/mensajes asociados al grupo a los diálogos correspondientes
 	 *
 	 *  Los parámetros que se le pasan son la ruta del archivo
 	 *  el ID del interactuable del diálogo actual, el ID del dialogo actual, el tipo de diálogo actual y la posición del dialogo actual
	 */
	public static void LoadGrupo(string path, int IDInteractuable, int IDDialogoActual, int tipoDialogo, ref int posDialogo)
	{
		Grupo grupo = Manager.instance.DeserializeData<Grupo>(path);

		//Se encarga de cargar los intros/mensajes iniciales para añadirlos a los diálogos correspondientes
		Lanzador.LoadLanzador(Manager.rutaLanzadores + grupo.IDGrupo.ToString() + ".xml", IDInteractuable,IDDialogoActual, tipoDialogo, ref posDialogo);

		Manager.instance.AddToGruposActivos(grupo);
	}

	/*
 	 *  Igual que la función anterior, pero pasándole un objeto grupo directamente
 	 *  Lo añade a la lista de grupos activos de la clase Manager
 	 *  Añade los intros/mensajes asociados al grupo a los diálogos correspondientes
 	 *
 	 *  Los parámetros que se le pasan son la ruta del archivo
 	 *  el ID del interactuable del diálogo actual, el ID del dialogo actual, el tipo de diálogo actual y la posición del dialogo actual
	 */
	public static void LoadGrupo(Grupo grupo, int IDInteractuable, int IDDialogoActual, int tipoDialogo, ref int posDialogo)
	{
		//Se encarga de cargar los intros/mensajes iniciales para añadirlos a los diálogos correspondientes
		Lanzador.LoadLanzador(Manager.rutaLanzadores + grupo.IDGrupo.ToString() + ".xml", IDInteractuable,IDDialogoActual, tipoDialogo, ref posDialogo);

		Manager.instance.AddToGruposActivos(grupo);
	}

	//Añade el grupo a la cola de objetos serializables
	//El grupo se serializa en el directorio de grupos modificados
	//Los grupos activos se serializan directamente en la clase Manager
	public void AddToColaObjetos()
	{
		Manager.instance.AddToColaObjetos(Manager.rutaGruposModificados + IDGrupo.ToString()  + ".xml", this);
	}
}