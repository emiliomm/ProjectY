using UnityEngine;

/*
 * 	Clase hija de Interactuable que contiene datos sobre la subclase de los interactuables llamada InteractuableObjeto
 */
public class InteractuableObjeto : Interactuable {

	private ObjetoDatos datos; //Almacena los datos de esta clase

	protected override void Start()
	{
		//Carga los datos del directorio predeterminado o del de guardado si hay datos guardados
		if (System.IO.File.Exists(Manager.rutaNPCDatosGuardados + ID.ToString()  + ".xml"))
		{
			datos = ObjetoDatos.LoadInterDatos(Manager.rutaNPCDatosGuardados + ID.ToString()  + ".xml");
		}
		else
		{
			datos = ObjetoDatos.LoadInterDatos(Manager.rutaNPCDatos + ID.ToString()  + ".xml");
		}

		//Ejecuta el metodo del padre
		base.Start();

		//Establece el nombre del interactuable
		SetNombre(datos.DevuelveNombreActual());

		if(datos.DevuelveIDTransporte() != -1)
			CrearTransporte(datos.DevuelveIDTransporte());
	}

	//necesaria de implementar por la clase base
	//Devuelve un string vacío ya que el nombre de los objetos no aparece en el dialogo
	public override string DevuelveNombreDialogo()
	{
		return "";
	}

	public ObjetoDatos devuelveDatos()
	{
		return datos;
	}

	protected override bool mostrarAccion(DatosAccion dAcc, Inventario inventario)
	{
		bool mostrarAccion = base.mostrarAccion(dAcc, inventario);

		if(mostrarAccion)
		{
			for(int i = 0; i < dAcc.variables.Count; i++)
			{
				switch(dAcc.variables[i].tipo)
				{
				case 0: // > es verdadero
					if(dAcc.variables[i].valor <= datos.DevuelveValorVariable(dAcc.variables[i].num_variable))
						mostrarAccion = false;
					break;
				case 1: // == es verdadero
					if(dAcc.variables[i].valor < datos.DevuelveValorVariable(dAcc.variables[i].num_variable) || dAcc.variables[i].valor < datos.DevuelveValorVariable(dAcc.variables[i].num_variable))
						mostrarAccion = false;
					break;
				case 2: // < es verdadero
					if(dAcc.variables[i].valor >= datos.DevuelveValorVariable(dAcc.variables[i].num_variable))
						mostrarAccion = false;
					break;
				}
			}
		}

		return mostrarAccion;
	}

	private void CrearTransporte(int IDTransporte)
	{
		ObjetoTransporteInter obj = ObjetoTransporteInter.LoadObjetoTransporteInter(Manager.rutaTransportes + IDTransporte.ToString() + ".xml");

		GameObject Transporte = new GameObject("Transporte");
		Transporte.transform.SetParent(gameObject.transform, false);

		if(obj.GetType() == typeof(ObjetoTransporteInter))
		{
			TransporteInterObjeto transIntObj = Transporte.AddComponent<TransporteInterObjeto>();
			transIntObj.Constructor(obj.ID, obj.escenas);
		}
		else if(obj.GetType() == typeof(ObjetoTransportePlayer))
		{
			ObjetoTransportePlayer objPl = obj as ObjetoTransportePlayer;

			TransportePlayerObjeto transPlaObj = Transporte.AddComponent<TransportePlayerObjeto>();
			transPlaObj.Constructor(objPl.ID, objPl.escenas, objPl.haciaEscena);
		}
	}
}

