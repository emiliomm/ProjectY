using UnityEngine;
using UnityEngine.AI;

/*
 * 	Clase hija de Interactuable que contiene datos sobre la subclase de los interactuables llamada InteractuableObjeto
 */
public class InteractuableObjeto : Interactuable {

	private NavMeshObstacle obstacle;
	private ObjetoDatos datos; //Almacena los datos de esta clase

	private int numInteractuablesEnTransito;

	protected override void Start()
	{
		numInteractuablesEnTransito = 0;

		//Carga los datos del directorio predeterminado o del de guardado si hay datos guardados
		if (System.IO.File.Exists(Manager.rutaInterDatosGuardados + ID.ToString()  + ".xml"))
		{
			datos = ObjetoDatos.LoadInterDatos(Manager.rutaInterDatosGuardados + ID.ToString()  + ".xml");
		}
		else
		{
			datos = ObjetoDatos.LoadInterDatos(Manager.rutaInterDatos + ID.ToString()  + ".xml");
		}

		//Ejecuta el metodo del padre
		base.Start();

		obstacle = GetComponent<NavMeshObstacle>();

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

	public ObjetoDatos DevuelveDatos()
	{
		return datos;
	}

	protected override bool MostrarAccion(DatosAccion datosAccion, Inventario inventario)
	{
		bool mostrarAccion = base.MostrarAccion(datosAccion, inventario);

		if(mostrarAccion)
		{
			for(int i = 0; i < datosAccion.variables.Count; i++)
			{
				switch(datosAccion.variables[i].tipo)
				{
				case 0: // > es verdadero
					if(datosAccion.variables[i].valor <= datos.DevuelveValorVariable(datosAccion.variables[i].numVariable))
						mostrarAccion = false;
					break;
				case 1: // == es verdadero
					if(datosAccion.variables[i].valor < datos.DevuelveValorVariable(datosAccion.variables[i].numVariable) || datosAccion.variables[i].valor < datos.DevuelveValorVariable(datosAccion.variables[i].numVariable))
						mostrarAccion = false;
					break;
				case 2: // < es verdadero
					if(datosAccion.variables[i].valor >= datos.DevuelveValorVariable(datosAccion.variables[i].numVariable))
						mostrarAccion = false;
					break;
				}
			}
		}

		return mostrarAccion;
	}

	private void CrearTransporte(int IDTransporte)
	{
		ObjetoTransporteInter objetoTransporteInter = ObjetoTransporteInter.LoadObjetoTransporteInter(Manager.rutaTransportes + IDTransporte.ToString() + ".xml");

		GameObject transporteGO = new GameObject("Transporte");
		transporteGO.transform.SetParent(gameObject.transform, false);

		if(objetoTransporteInter.GetType() == typeof(ObjetoTransporteInter))
		{
			TransporteInterObjeto transporteInterObjeto = transporteGO.AddComponent<TransporteInterObjeto>();
			transporteInterObjeto.Constructor(objetoTransporteInter.ID, objetoTransporteInter.escenas);
		}
	}

	public void SetNavObstacle(bool estado)
	{
		if(!estado)
		{
			numInteractuablesEnTransito++;
			obstacle.enabled = estado;
		}
		else
		{
			numInteractuablesEnTransito--;

			if(numInteractuablesEnTransito == 0)
				obstacle.enabled = estado;
		}
	}
}

