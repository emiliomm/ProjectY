using System.Collections.Generic;
using System.Xml.Serialization;

/*
 * 	Clase base que contiene información sobre la Acción de un Interactuable. Esta clase
 *  permite realizar múltiples acciones, que están definidas en las clases derivadas.
 * 
 *  Se incluye un XmlInclude de las clases derivadas para la correcta serizalización/deserialización
 */

[XmlInclude(typeof(DatosAccionDialogo))]
[XmlInclude(typeof(DatosAccionObjeto))]
[XmlInclude(typeof(DatosAccionTienda))]
[XmlInclude(typeof(DatosAccionLector))]
[XmlInclude(typeof(DatosAccionTransporte))]
public class DatosAccion {

	public int ID; //ID que identifica a la acción (aún sin usar)
	public string nombre; //nombre de la acción que se muestra en la interfaz

	public List<ComprobarObjeto> objetos; //Lista con clases que condicionan si una acción se muestra dependiendo de los objetos en el Inventario

	public List<ComprobarVariable> variables;

	public DatosAccion()
	{
		objetos = new List<ComprobarObjeto>();
		variables = new List<ComprobarVariable>();
	}

	public string DevolverNombre()
	{
		return nombre;
	}

	//Método virtual usado por las clases derivadas que se ejecuta cuando el jugador
	//activa la acción
	public virtual void EjecutarAccion(){  }
}
