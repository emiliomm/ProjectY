using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using DialogueTree;

public class NPC_Dialogo{

	private List<DialogoEntrante> dialogos = new List<DialogoEntrante>();
	private List<Pregunta> preguntas = new List<Pregunta>();

	private int indice_dialogo = 0; //Indice actual del indice de dialogos

	public NPC_Dialogo()
	{
		DialogoEntrante d = new DialogoEntrante("text_dia.xml");
		DialogoEntrante d2 = new DialogoEntrante("text_dia2.xml");

		d2.prioridad = 5;

		AnyadirDialogo (d);
		AnyadirDialogo (d2);

		for(int i = 0; i < 5; i++)
		{
			preguntas.Add(new Pregunta("Opcion " + i.ToString(),"text_dia3.xml"));
		}
	}

	public void AnyadirDialogo(DialogoEntrante d)
	{
		dialogos.Add (d);
		dialogos.Sort ();
//		dialogos.Sort (CompareTo);
	}
		
	//Método para usar Sort de List
	public int CompareTo(DialogoEntrante d1, DialogoEntrante d2)
	{
		if (d1.prioridad == d2.prioridad) return 0;
		if (d1.prioridad == 0) return +1;
		if (d2.prioridad == 0) return -1;
		// otherwise compare normally
		return d2.prioridad-d1.prioridad;
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
		return dialogos[indice_dialogo].dia.Nodes[node_id];
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
		return dialogos[indice_dialogo].dia.Nodes[node_id].Options.Count;
	}

	public int DevuelveNumeroOpcionesNodoPregunta (int node_id)
	{
		return preguntas[indice_dialogo].dia.Nodes[node_id].Options.Count;
	}
}
