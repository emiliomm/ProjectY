using UnityEngine;

/*
 * 	Clase derivada de DatosAccion que permite a una acción iniciar el modo Objeto, que permite interactuar con
 *  un objeto
 */
public class DatosAccionObjeto : DatosAccion{

	public string nombreObjecto; //Indica el nombre del gameObject que se cargará en la ruta de prefabs de Objetos

	//Layermask del objeto que se cargará
	//Pasar al Manager
	int layerMask = 8; //UIObjeto

	public DatosAccionObjeto()
	{
		
	}

	//Inicia el modo objeto
	public override void EjecutarAccion()
	{
		Manager.Instance.setPausa(true);
		Cursor.visible = true; //Muestra el cursor del ratón

		//Carga el objeto de Resources, lo mueve delante de la cámara y le establece una layermask
		var Objeto = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Objetos/" + nombreObjecto));
		Objeto.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
		Objeto.layer = layerMask;

		//Se añade al objeto un objeto de la clase ObjetoController
		//Que controla la interfaz de al manipular el objeto
		ObjetoController objetoController = Objeto.AddComponent<ObjetoController>();
		objetoController.AsignarObjeto(Objeto);

		//Se establece el modo de la cámara en el Modo Objeto
		Camera.main.GetComponent<TP_Camera>().setObjectMode();
	}
}
