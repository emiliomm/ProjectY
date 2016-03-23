using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;

public class NPC : MonoBehaviour {

	public int id;
	public bool requiredButtonPress; //indica si se requiere que se pulse una tecla para iniciar la conversación
	public NPC_Dialogo npc_diag; //NPC del cual carga el dialogo

	private bool waitForPress;

	//HACER VARIABLES GLOBALES
	private static string _FileLocation;
	private static string DefaultDialogs;

	void Start()
	{
		_FileLocation = Application.persistentDataPath + "/NPC_Dialogo_Saves/";
		DefaultDialogs = Application.dataPath + "/StreamingAssets/NPCDialogue/";

		//Cargamos el dialogo
		//Si existe un fichero guardado, cargamos ese fichero, sino
		//cargamos el fichero por defecto
		if (System.IO.File.Exists(_FileLocation + id.ToString()  + ".xml"))
		{
			npc_diag = NPC_Dialogo.LoadNPCDialogue(id, _FileLocation + id.ToString()  + ".xml");
		}
		else
		{
			npc_diag = NPC_Dialogo.LoadNPCDialogue(id, DefaultDialogs + id.ToString()  + ".xml");
		}

		//Añadimos el npc al diccionario para tenerlo disponible
		Manager.Instance.AddToNpcs(id, gameObject);
	}

	void OnDestroy()
	{
		//Borramos el valor del diccionario cuando el npc no existe
		Manager.Instance.RemoveFromNpcs(id);
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
			if (!TextBox.Instance.EstaActivo())
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
		if (waitForPress && Input.GetMouseButtonDown(1) && !TextBox.Instance.EstaActivo())
		{
			IniciaDialogo();
		}
	}

	//Inicia el dialogo
	private void IniciaDialogo()
	{
		TextBox.Instance.StartDialogue(this, npc_diag);
	}

	public void ActualizarDialogo(NPC_Dialogo dia)
	{
		npc_diag = dia; //Actualizamos el dialogo del objeto
		dia.SerializeToXml(); //Lo convertimos en XML
	}
}
