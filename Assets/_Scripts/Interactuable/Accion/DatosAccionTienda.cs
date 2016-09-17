using UnityEngine;
using UnityEngine.UI;

using System.Xml.Serialization;

public class DatosAccionTienda : DatosAccion{

	public int IDInventario; //contiene los objetos que vende la tienda

	//Número de objetos por fila y columna en cada pantalla
//	public int numX;
	public int numY;

	public bool escaparate;

//	public Font fuente;
//
//	public Image fondo;

	public DatosAccionTienda()
	{
		
	}

	//PASAR A OBJETO TIENDA ALGUNAS DE LAS FUNCIONES
	public override void EjecutarAccion()
	{
		ManagerTiempo.instance.SetPausa(true);
		Manager.instance.StopNavMeshAgents();
		Cursor.visible = true; //Muestra el cursor del ratón

		var objetoTienda = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Tienda/PanelTiendaPrefab"));

		Inventario inventario = Inventario.LoadInventario(Manager.rutaInventarioTienda + IDInventario.ToString() + ".xml");
		CargarInventario(inventario);

		TiendaController tiendaController = objetoTienda.AddComponent<TiendaController>();
//		objetoController.InicializarTienda(numX, numY, fuente, inv, fondo);
		tiendaController.InicializarTienda(escaparate, numY, inventario, false);

		//Se establece el modo de la cámara en el Modo Objeto
		Camera.main.GetComponent<TPCamera>().SetObjectMode();
	}

	private void CargarInventario(Inventario inventario)
	{
		for(int i = 0; i < inventario.DevolverNumeroObjetos(); i++)
		{
			inventario.SustituyeObjeto(ObjetoInventario.LoadObjeto(Manager.rutaObjetoInventario + inventario.DevolverObjeto(i).ID.ToString() + ".xml"), i);
		}
	}
}
