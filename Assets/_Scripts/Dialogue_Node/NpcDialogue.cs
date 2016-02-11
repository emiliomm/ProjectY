using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;

using DialogueTree;

public class NpcDialogue : MonoBehaviour {

	public static TextBox box; //Nos permite lidiar con la caja de texto
	public bool requiredButtonPress; //indica si se requiere que se pulse una tecla para iniciar la conversación
	public NPC npc; //NPC del cual carga el dialogo

	private bool waitForPress;

	// Use this for initialization
	void Awake () {
		box = FindObjectOfType<TextBox>();
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
			if (!box.isActive)
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
		//Si está esperando a pulsar la tecla y pulsamos J,
		if (waitForPress && Input.GetKeyDown(KeyCode.J) && !box.isActive)
		{
			IniciaDialogo();
		}
	}

	//Inicia el dialogo
	void IniciaDialogo()
	{
		box.reloadDialogue (npc.dialogos[npc.indice]);
	}
}
