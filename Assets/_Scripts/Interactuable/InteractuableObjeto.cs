/*
 * 	Clase hija de Interactuable que contiene datos sobre la subclase de los interactuables llamada InteractuableObjeto
 */
public class InteractuableObjeto : Interactuable {

	ObjetoDatos datos; //Almacena los datos de esta clase

	protected override void Start()
	{
		//Ejecuta el metodo del padre
		base.Start();

		//Carga los datos del directorio predeterminado o del de guardado si hay datos guardados
		if (System.IO.File.Exists(Manager.rutaNPCDatosGuardados + ID.ToString()  + ".xml"))
		{
			datos = ObjetoDatos.LoadInterDatos(Manager.rutaNPCDatosGuardados + ID.ToString()  + ".xml");
		}
		else
		{
			datos = ObjetoDatos.LoadInterDatos(Manager.rutaNPCDatos + ID.ToString()  + ".xml");
		}

		//Establece el nombre del interactuable
		SetNombre(datos.DevuelveNombreActual());
	}

	//necesaria de implementar por la clase base
	//Devuelve un string vacío ya que el nombre de los objetos no aparece en el dialogo
	public override string DevuelveNombreDialogo()
	{
		
		return "";
	}
}

