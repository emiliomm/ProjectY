using UnityEngine;
using System.Collections;

public class Lector : MonoBehaviour {

	private ObjetoDatos datos;

	private int num_variable;
	private int valorInicial;

	private bool activado;
	private bool activadoActual;

	public void cargarValor(int IDObjeto, int num_variable, int valorNegativo)
	{
		GameObject interGO = Manager.Instance.GetInteractuable(IDObjeto);

		if(interGO == null)
		{
			//Carga los datos del directorio predeterminado o del de guardado si hay datos guardados
			if (System.IO.File.Exists(Manager.rutaNPCDatosGuardados + IDObjeto.ToString()  + ".xml"))
			{
				datos = ObjetoDatos.LoadInterDatos(Manager.rutaNPCDatosGuardados + IDObjeto.ToString()  + ".xml");
			}
			else
			{
				datos = ObjetoDatos.LoadInterDatos(Manager.rutaNPCDatos + IDObjeto.ToString()  + ".xml");
			}
		}
		else
		{
			InteractuableObjeto inter = interGO.GetComponent<InteractuableObjeto>();
			datos = inter.devuelveDatos();
		}

		valorInicial = datos.DevuelveValorVariable(num_variable);

		if(valorInicial == valorNegativo)
			activado = false;
		else
			activado = true;

		this.num_variable = num_variable;
		activadoActual = activado;

		cargarLuz();
	}

	private void cargarLuz()
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
			cambiarEstado();
		}
	}

	private void cambiarEstado()
	{
		activadoActual = !activadoActual;
		cargarLuz();
	}

	public void guardarValor()
	{
		if(activado != activadoActual)
		{
			if(activadoActual)
			{
				datos.SetValorVariable(num_variable, valorInicial+1);
			}
			else
			{
				datos.SetValorVariable(num_variable, valorInicial-1);
			}
			datos.Serialize();

			//CAMBIAR EN EL FUTURO
			//Buscamos el inventario en la colaobjetos
			Inventario inventario;
			ColaObjeto inventarioCola = Manager.Instance.GetColaObjetos(Manager.rutaInventario + "Inventario.xml");

			//Se ha encontrado en la cola de objetos
			if(inventarioCola != null)
			{
				ObjetoSerializable objs = inventarioCola.GetObjeto();
				inventario = objs as Inventario;
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
			Manager.Instance.actualizarAcciones(inventario);
		}
	}
}
