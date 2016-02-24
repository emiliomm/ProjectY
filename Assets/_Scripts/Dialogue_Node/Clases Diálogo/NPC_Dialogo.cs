using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using DialogueTree;

public class NPC_Dialogo{

	//Los dialogos deben estar ordenados por prioridad

	private List<Dialogue> dialogos = new List<Dialogue>();
	private List<Pregunta> preguntas = new List<Pregunta>();

	private int indice_dialogo = 0; //Indice actual del indice de dialogos

	public NPC_Dialogo()
	{
		Dialogue d = Dialogue.LoadDialogue ("Assets/_Texts/" + "text_dia.xml");
		Dialogue d2 = Dialogue.LoadDialogue ("Assets/_Texts/" + "text_dia2.xml");

//		dialogos.Add (d);
//		dialogos.Add (d2);

		for(int i = 0; i < 5; i++)
		{
			preguntas.Add(new Pregunta("Opcion " + i.ToString(),"text_dia3.xml"));
		}
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
		indice_dialogo++;
	}

	public void PonerIndice(int num)
	{
		indice_dialogo = num;
	}

	public int DevuelveNumeroDialogos()
	{
		return dialogos.Count;
	}

	public int DevuelveNumeroPreguntas()
	{
		return preguntas.Count;
	}

	public Pregunta DevuelvePregunta(int num_pre)
	{
		return preguntas[num_pre];
	}

	public DialogueNode DevuelveNodoDialogoDialogo(int node_id)
	{
		return dialogos[indice_dialogo].Nodes[node_id];
	}

	public DialogueNode DevuelveNodoDialogoPregunta(int node_id)
	{
		return preguntas[indice_dialogo].dia.Nodes[node_id];
	}

	public DialogueNode DevuelveNodoPregunta(int node_id)
	{
		return preguntas[indice_dialogo].dia.Nodes[node_id];
	}

	public int DevuelveNumeroOpcionesNodoDialogo (int node_id)
	{
		return dialogos[indice_dialogo].Nodes[node_id].Options.Count;
	}

	public int DevuelveNumeroOpcionesNodoPregunta (int node_id)
	{
		return preguntas[indice_dialogo].dia.Nodes[node_id].Options.Count;
	}
}
