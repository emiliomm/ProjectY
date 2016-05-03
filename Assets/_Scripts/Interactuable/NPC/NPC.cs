using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;

public class NPC : MonoBehaviour {

	public int ID;

	public bool requiredButtonPress; //indica si se requiere que se pulse una tecla para iniciar la conversación

	private NPC_Dialogo npc_diag; //NPC del cual carga el dialogo
	private NPCDatos datos;

	void Start()
	{
		StartCoroutine(Cargar());
	}

	//Carga las acciones y el nombre
	public IEnumerator Cargar()
	{
		yield return new WaitForSeconds (0.25f);

		//Si existe un fichero guardado, cargamos ese fichero, sino
		//cargamos el fichero por defecto
		if (System.IO.File.Exists(Manager.rutaNPCDatosGuardados + ID.ToString()  + ".xml"))
		{
			datos = NPCDatos.LoadNPCDatos(Manager.rutaNPCDatosGuardados + ID.ToString()  + ".xml");
		}
		else
		{
			datos = NPCDatos.LoadNPCDatos(Manager.rutaNPCDatos + ID.ToString()  + ".xml");
		}

		//Cargamos el dialogo
		//Si existe un fichero guardado, cargamos ese fichero, sino
		//cargamos el fichero por defecto
		if (System.IO.File.Exists(Manager.rutaNPCDialogosGuardados + ID.ToString()  + ".xml"))
		{
			npc_diag = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogosGuardados + ID.ToString()  + ".xml");
		}
		else
		{
			npc_diag = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogos + ID.ToString()  + ".xml");
		}

		//Añadimos el npc al diccionario para tenerlo disponible
		Manager.Instance.AddToNpcs(ID, gameObject);

		Interactuable inter = transform.parent.gameObject.GetComponent<Interactuable>();

		GameObject AccionGO = new GameObject("Accion");
		AccionDialogo ac = AccionGO.AddComponent<AccionDialogo>();
		ac.ConstructorAccion("Hablar", ID);

		inter.AddAccion(AccionGO);
		inter.SetNombre(datos.DevuelveNombreActual());
	}

	void OnDestroy()
	{
		//Borramos el valor del diccionario cuando el npc no existe
		Manager.Instance.RemoveFromNpcs(ID);
	}
		
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			if (!requiredButtonPress && TP_Controller.Instance.CurrentState == TP_Controller.State.Normal) 
			{
				IniciaDialogo();
			}
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

	public NPC_Dialogo DevuelveDialogo()
	{
		return npc_diag;
	}

	public NPCDatos DevuelveDatos()
	{
		return datos;
	}

	public string DevuelveNombre()
	{
		return datos.DevuelveNombreActual();
	}
}
