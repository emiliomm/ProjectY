﻿using UnityEngine;
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
		Manager.Instance.setPausa(true);
		Manager.Instance.stopNavMeshAgents();
		Cursor.visible = true; //Muestra el cursor del ratón

		var ObjetoTienda = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Tienda/PanelTiendaPrefab"));

		Inventario inv = Inventario.LoadInventario(Manager.rutaInventarioTienda + IDInventario.ToString() + ".xml");
		CargarInventario(inv);

		TiendaController objetoController = ObjetoTienda.AddComponent<TiendaController>();
//		objetoController.InicializarTienda(numX, numY, fuente, inv, fondo);
		objetoController.InicializarTienda(escaparate, numY, inv, false);

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