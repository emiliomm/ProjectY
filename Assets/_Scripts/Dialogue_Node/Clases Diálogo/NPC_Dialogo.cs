using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using DialogueTree;

public class NPC_Dialogo : MonoBehaviour{

	//Los dialogos deben estar ordenados por prioridad

	private List<Dialogue> dialogos = new List<Dialogue>();
	private List<Pregunta> preguntas = new List<Pregunta>();

	private int indice_dialogo = 0; //Indice actual del indice de dialogos

	// Use this for initialization
	void Start()
	{
		CargarDialogosPrueba ();
	}

	public void CargarDialogosPrueba()
	{
		Dialogue d = Dialogue.LoadDialogue ("Assets/" + "text_dia.xml");
		Dialogue d2 = Dialogue.LoadDialogue ("Assets/" + "text_dia.xml2");

		dialogos.Add (d);
		dialogos.Add (d2);

//		for(int i = 0; i < 5; i++)
//		{
//			preguntas.Add(new Pregunta("Opcion " + i.ToString(),"_Texts/text_dia3.xml"));
//		}
	}

	public bool HayMasDialogos()
	{
		bool mas = false;
		if (indice_dialogo + 1 < dialogos.Count)
			mas = true;

		return mas;
	}

	public void AvanzaDialogo()
	{
		bool mas = false;
		if (indice_dialogo + 1 < dialogos.Count)
			mas = true;

		return mas;
	}

	public DialogueNode DevuelveNodo(int node_id)
	{
		return dialogos[indice_dialogo].Nodes[node_id];
	}

	public int DevuelveNumeroOpcionesNodo (int node_id)
	{
		return dialogos[indice_dialogo].Nodes[node_id].Options.Count;
	}
}
