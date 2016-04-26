using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;

public class NPC : MonoBehaviour {

	public int ID;
	public bool requiredButtonPress; //indica si se requiere que se pulse una tecla para iniciar la conversación
	public NPC_Dialogo npc_diag; //NPC del cual carga el dialogo

	private bool waitForPress;

	void Start()
	{
		//Cargamos el dialogo
		//Si existe un fichero guardado, cargamos ese fichero, sino
		//cargamos el fichero por defecto
		if (System.IO.File.Exists(Manager.rutaNPCDialogosGuardados + ID.ToString()  + ".xml"))
		{
			npc_diag = NPC_Dialogo.LoadNPCDialogue(ID, Manager.rutaNPCDialogosGuardados + ID.ToString()  + ".xml");
		}
		else
		{
			npc_diag = NPC_Dialogo.LoadNPCDialogue(ID, Manager.rutaNPCDialogos + ID.ToString()  + ".xml");
		}

		//Añadimos el npc al diccionario para tenerlo disponible
		Manager.Instance.AddToNpcs(ID, gameObject);

		StartCoroutine(run());

	}

	public IEnumerator run()
	{
		yield return new WaitForSeconds (0.25f);

		Interactuable inter = transform.parent.gameObject.GetComponent<Interactuable>();

		GameObject AccionGO = new GameObject("Accion");
		AccionDialogo ac = AccionGO.AddComponent<AccionDialogo>();
		ac.ConstructorAccion("Hablar", ID);

		inter.AddAccion(AccionGO);
	}

	void OnDestroy()
	{
		//Borramos el valor del diccionario cuando el npc no existe
		Manager.Instance.RemoveFromNpcs(ID);
	}

	//Si colisionamos con el jugador, cargamos el nuevo texto
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			//Si se necesita pulsar el boton,activamos la variable waitfropress
			if (requiredButtonPress) 
			{
				waitForPress = true;
				return;
			}
			if (TP_Controller.Instance.CurrentState == TP_Controller.State.Normal)
				IniciaDialogo();
		}
	}

	//Al salir de la colision, desactivactivamos la variable waitforpress
	void OnTriggerExit(Collider other)
	{
		if(other.tag == "Player")
		{
			waitForPress = false;
		}
	}

	void Update()
	{
		//Si está esperando al input y pulsamos click derecho
		if (waitForPress && Input.GetMouseButtonDown(1) && TP_Controller.Instance.CurrentState == TP_Controller.State.Normal)
		{
			IniciaDialogo();
		}
	}

	//Inicia el dialogo
	public void IniciaDialogo()
	{
		TextBox.Instance.StartDialogue(this, npc_diag);
	}

	public void ActualizarDialogo(NPC_Dialogo dia)
	{
		npc_diag = dia; //Actualizamos el dialogo del objeto
		dia.Serialize(); //Lo convertimos en XML
	}
}
