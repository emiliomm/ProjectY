using UnityEngine;
using System.Collections;

public class Lector : MonoBehaviour {

	private ObjetoDatos datos;

	private int numVariable;
	private int valorInicial;

	private bool activado;
	private bool activadoActual;

	public void CargarValor(int IDObjeto, int numVariable, int valorNegativo)
	{
		GameObject interactuableGO = Manager.instance.GetInteractuable(IDObjeto);

		if(interactuableGO == null)
		{
			//Carga los datos del directorio predeterminado o del de guardado si hay datos guardados
			if (System.IO.File.Exists(Manager.rutaInterDatosGuardados + IDObjeto.ToString()  + ".xml"))
			{
				datos = ObjetoDatos.LoadInterDatos(Manager.rutaInterDatosGuardados + IDObjeto.ToString()  + ".xml");
			}
			else
			{
				datos = ObjetoDatos.LoadInterDatos(Manager.rutaInterDatos + IDObjeto.ToString()  + ".xml");
			}
		}
		else
		{
			InteractuableObjeto interactuableObjeto = interactuableGO.GetComponent<InteractuableObjeto>();
			datos = interactuableObjeto.DevuelveDatos();
		}

		valorInicial = datos.DevuelveValorVariable(numVariable);

		if(valorInicial == valorNegativo)
			activado = false;
		else
			activado = true;

		this.numVariable = numVariable;
		activadoActual = activado;

		CargarLuz();
	}

	private void CargarLuz()
	{
		GameObject luzGO = transform.GetChild(2).gameObject;

		if(activadoActual)
		{
			Light luz = luzGO.GetComponent<Light>();
			luz.color = new Color(0, 255, 12);
		}
		else
		{
			Light luz = luzGO.GetComponent<Light>();
			luz.color = new Color(255, 0, 0);
		}
	}
		
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "LLave")
		{
			CambiarEstado();
		}
	}

	private void CambiarEstado()
	{
		activadoActual = !activadoActual;
		CargarLuz();
	}

	public void GuardarValor()
	{
		if(activado != activadoActual)
		{
			if(activadoActual)
			{
				datos.SetValorVariable(numVariable, valorInicial+1);
			}
			else
			{
				datos.SetValorVariable(numVariable, valorInicial-1);
			}
			datos.Serialize();

			//CAMBIAR EN EL FUTURO
			//Buscamos el inventario en la colaobjetos
			Inventario inventario;
			ColaObjeto inventarioCola = Manager.instance.GetColaObjetos(Manager.rutaInventario + "Inventario.xml");

			//Se ha encontrado en la cola de objetos
			if(inventarioCola != null)
			{
				ObjetoSerializable objetoSerializable = inventarioCola.GetObjeto();
				inventario = objetoSerializable as Inventario;
			}
			//No se ha encontrado en la cola de objetos
			else
			{
				//Cargamos el inventario si existe, sino lo creamos
				if(System.IO.File.Exists(Manager.rutaInventario + "Inventario.xml"))
				{
					inventario = Inventario.LoadInventario(Manager.rutaInventario + "Inventario.xml");
				}
				else
				{
					inventario = new Inventario();
				}
			}
			Manager.instance.ActualizarAcciones(inventario);
		}
	}
}
