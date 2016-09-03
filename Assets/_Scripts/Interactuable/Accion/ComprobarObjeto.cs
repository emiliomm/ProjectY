/*
 * 	Clase que indica si una Accion tiene condiciones respecto a objetos del Inventario
 *  Antes de mostrarse una acción en la interfaz del interactuable, se tienen que comprobar estos parámetros
 */
public class ComprobarObjeto{

	//Si equipado es true, la acción se mostrará si se tiene el objeto con el IDObjeto
	//Si equipado es false, la acción se mostrará si no se tiene el objeto con el IDObjeto
	public int IDObjeto;
	public bool equipado;

	public ComprobarObjeto()
	{
		
	}
}
