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
		TextBox.Instance.OcultarInterfaz();

		var ObjetoTienda = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Tienda/PanelTiendaPrefab"));

		Inventario inv = Inventario.LoadInventario(Manager.rutaInventarioTienda + IDInventario.ToString() + ".xml");
		CargarInventario(inv);

		TiendaController objetoController = ObjetoTienda.AddComponent<TiendaController>();
		//		objetoController.InicializarTienda(numX, numY, fuente, inv, fondo);
		objetoController.InicializarTienda(escaparate, numY, inv, true);

		//Se establece el modo de la cámara en el Modo Objeto
		Camera.main.GetComponent<TP_Camera>().setObjectMode();
	}

	private void CargarInventario(Inventario inventario)
	{
		for(int i = 0; i < inventario.devolverNumeroObjetos(); i++)
		{
			inventario.sustituyeObjeto(ObjetoInventario.LoadObjeto(Manager.rutaObjetoInventario + inventario.devolverObjeto(i).ID.ToString() + ".xml"), i);
		}
	}
}
