using UnityEngine;
using System.Collections;

public class MensajeTienda : Mensaje
{
	public int IDInventario; //contiene los objetos que vende la tienda

	//Número de objetos por fila y columna en cada pantalla
	//	public int numX;
	public int numY;

	public bool escaparate;

	//	public Font fuente;
	//
	//	public Image fondo;

	public MensajeTienda()
	{

	}

	public void MostrarTienda()
	{
		TextBox.instance.OcultarInterfaz();

		var ObjetoTienda = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Tienda/PanelTiendaPrefab"));

		Inventario inventario = Inventario.LoadInventario(Manager.rutaInventarioTienda + IDInventario.ToString() + ".xml");
		CargarInventario(inventario);

		TiendaController tiendaController = ObjetoTienda.AddComponent<TiendaController>();
		//		objetoController.InicializarTienda(numX, numY, fuente, inv, fondo);
		tiendaController.InicializarTienda(escaparate, numY, inventario, true);

		//Se establece el modo de la cámara en el Modo Objeto
		Camera.main.GetComponent<TP_Camera>().setObjectMode();
	}

	private void CargarInventario(Inventario inventario)
	{
		for(int i = 0; i < inventario.DevolverNumeroObjetos(); i++)
		{
			inventario.SustituyeObjeto(ObjetoInventario.LoadObjeto(Manager.rutaObjetoInventario + inventario.DevolverObjeto(i).ID.ToString() + ".xml"), i);
		}
	}
}
