using UnityEngine;

/*
 * 	Clase derivada de DatosAccion que permite a una acción iniciar el modo Objeto, que permite interactuar con
 *  un objeto
 */
public class DatosAccionObjeto : DatosAccion{

	public string nombreObjecto; //Indica el nombre del gameObject que se cargará en la ruta de prefabs de Objetos

	//Layermask del objeto que se cargará
	//Pasar al Manager
	private int layerMask = 8; //UIObjeto

	public DatosAccionObjeto()
	{
		
	}

	//Inicia el modo objeto
	//PASAR A OBJETO CONTROLLER ALGUNAS DE LAS FUNCIONES
	public override void EjecutarAccion()
	{
		Manager.Instance.setPausa(true);
		Manager.Instance.stopNavMeshAgents();
		Cursor.visible = true; //Muestra el cursor del ratón

		//Carga el objeto de Resources, lo mueve delante de la cámara y le establece una layermask
		var objeto = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Objetos/" + nombreObjecto));
		objeto.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
		objeto.layer = layerMask;

		//Se añade al objeto un objeto de la clase ObjetoController
		//Que controla la interfaz de al manipular el objeto
		ObjetoController objetoController = objeto.AddComponent<ObjetoController>();
		objetoController.AsignarObjeto(objeto);

		//Se establece el modo de la cámara en el Modo Objeto
		Camera.main.GetComponent<TP_Camera>().setObjectMode();
	}
}
