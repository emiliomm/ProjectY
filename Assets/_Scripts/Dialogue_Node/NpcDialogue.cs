using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;

using DialogueTree;

public class NpcDialogue : MonoBehaviour {

	public bool requiredButtonPress; //indica si se requiere que se pulse una tecla para iniciar la conversación
	public NPC npc; //NPC del cual carga el dialogo

	private bool waitForPress;

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
			if (!TextBox.Instance.isActive)
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
		if (waitForPress && Input.GetMouseButtonDown(1) && !TextBox.Instance.isActive)
		{
			IniciaDialogo();
		}
	}

	//Inicia el dialogo
	void IniciaDialogo()
	{
<<<<<<< HEAD
		TextBox.Instance.StartDialogue (npc, npc.dialogos[npc.indice]);
=======
		TextBox.Instance.LoadDialogue (npc, npc.dialogos[npc.indice]);
>>>>>>> origin/master
	}
}
