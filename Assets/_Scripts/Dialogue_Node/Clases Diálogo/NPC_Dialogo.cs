using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using DialogueTree;

public class NPC_Dialogo{

	public List<Intro> intros = new List<Intro>();
	public List<Mensaje> mensajes = new List<Mensaje>();

	public static string rutaDialogos = "Assets/_Texts/";

	public NPC_Dialogo()
	{
		Intro d = new Intro("text_dia.xml");
		Intro d2 = new Intro("text_dia2.xml");

		d.prioridad = 5;

		AnyadirDialogo (d);
		AnyadirDialogo (d2);

		for(int i = 0; i < 5; i++)
		{
			mensajes.Add(new Mensaje("Opcion " + i.ToString(),rutaDialogos + "text_dia3.xml"));
		}
	}

	public void AnyadirDialogo(Intro d)
	{
		intros.Add (d);
		intros.Sort ();
	}

	public int DevuelveNumeroIntros()
	{
		return intros.Count;
	}

	public int DevuelveNumeroMensajes()
	{
		return mensajes.Count;
	}

	public Dialogue DevuelveDialogoIntro(int num_intro)
	{
		return intros[num_intro].DevuelveDialogo();
	}

	public Dialogue DevuelveDialogoMensajes(int num_mensaje)
	{
		return mensajes[num_mensaje].DevuelveDialogo();
	}

	public string DevuelveTextoMensaje(int num_mensaje)
	{
		return mensajes[num_mensaje].DevuelveTexto();
	}

	public bool AvanzaIntro(int num_intro)
	{
		bool avanza = false;
		if (num_intro + 1 < intros.Count)
			avanza = true;

		return avanza;
	}

	public void MarcaDialogueNodeComoLeido(int tipo, int num_dialogo, int node_id)
	{
		switch(tipo)
		{
		case 0:
			if(intros [num_dialogo].dia.Nodes [node_id].recorrido != true)
			{
				intros [num_dialogo].dia.Nodes [node_id].recorrido = true;
				AnyadirDialogueAdd(intros [num_dialogo].dia.Nodes [node_id]);
			}
			break;
		case 1:
			if(mensajes [num_dialogo].dia.Nodes [node_id].recorrido != true)
			{
				mensajes [num_dialogo].dia.Nodes [node_id].recorrido = true;
				AnyadirDialogueAdd(mensajes [num_dialogo].dia.Nodes [node_id]);
			}
			break;
		}
	}

	private void AnyadirDialogueAdd(DialogueNode node)
	{
		for(int i = 0; i < node.AddIntro.Count; i++)
		{
			//Hacer cosas, proximamente...
		}

		for(int i = 0; i < node.AddMensaje.Count; i++)
		{
			string nombreTexto = node.AddMensaje[i].DevuelveNombre();
			string mensaje = node.AddMensaje[i].DevuelveMensaje();

			mensajes.Add(new Mensaje(mensaje,rutaDialogos + nombreTexto));
		}
	}
//
//	public void AddDialogoEntrante(int num_dialog, int node_id)
//	{
//		DialogueNode dn = DevuelveNodoDialogoDialogo(num_dialog, node_id);
//		dn.recorrido = true;
//
//		for(int i = 0; i < dn.AddDialogo.Count; i++)
//		{
//			DialogoEntrante de = new DialogoEntrante(dn.AddDialogo[i].DevuelveNombre());
//			AnyadirDialogo (de);
//		}
//	}
//
//	public void AddPreguntaEntrante(int num_dialog, int node_id)
//	{
//		DialogueNode dn = DevuelveNodoDialogoDialogo(num_dialog, node_id);
//		dn.recorrido = true;
//
//		for(int i = 0; i < dn.AddPregunta.Count; i++)
//		{
//			preguntas.Add(new Pregunta("Opcion " + i.ToString(), dn.AddPregunta[i].DevuelveNombre()));
//		}
//	}
//
//	public void AddDialogoRespuestas(int num_dialog, int node_id)
//	{
//		DialogueNode dn = DevuelveNodoDialogoPregunta(num_dialog, node_id);
//		dn.recorrido = true;
//
//		for(int i = 0; i < dn.AddDialogo.Count; i++)
//		{
//			DialogoEntrante de = new DialogoEntrante(dn.AddDialogo[i].DevuelveNombre());
//			AnyadirDialogo (de);
//		}
//	}
//
//	public void AddPreguntaRespuestas(int num_dialog, int node_id)
//	{
//		DialogueNode dn = DevuelveNodoDialogoPregunta(num_dialog, node_id);
//		dn.recorrido = true;
//
//		for(int i = 0; i < dn.AddPregunta.Count; i++)
//		{
//			preguntas.Add(new Pregunta("Opcion " + i.ToString(), dn.AddPregunta[i].DevuelveNombre()));
//		}
//	}
}
