using UnityEngine;
using UnityEngine.UI;

using System.Xml.Serialization;

public class DatosAccionTienda : DatosAccion{

	public int IDInventario; //contiene los objetos que vende la tienda

	//Número de objetos por fila y columna en cada pantalla
//	public int numX;
	public int numY;

//	public Font fuente;
//
//	public Image fondo;

	public DatosAccionTienda()
	{
		
	}

	public override void EjecutarAccion()
	{
		Manager.Instance.setPausa(true);
		Manager.Instance.stopNavMeshAgents();
		Cursor.visible = true; //Muestra el cursor del ratón

		var ObjetoTienda = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Tienda/PanelTiendaPrefab"));

		Inventario inv = Inventario.LoadInventario(Manager.rutaInventarioTienda + IDInventario.ToString() + ".xml");

		TiendaController objetoController = ObjetoTienda.AddComponent<TiendaController>();
//		objetoController.InicializarTienda(numX, numY, fuente, inv, fondo);
		objetoController.InicializarTienda(numY, inv);

		//Se establece el modo de la cámara en el Modo Objeto
		Camera.main.GetComponent<TP_Camera>().setObjectMode();
	}
}
