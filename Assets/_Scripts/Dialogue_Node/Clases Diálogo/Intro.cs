﻿using UnityEngine;
using System.Collections;

using DialogueTree;

//Implementa la interfaz System.IComparable para que funcione el metodo compareTo
public class Intro : System.IComparable<Intro>{

	public Dialogue dia;
	public int prioridad;
	public int indice_inicial;

	public static string rutaDialogosGuardados = "Assets/_Data/";
	public static string rutaDialogosPredeterminada = "Assets/_Texts/";

	public Intro()
	{
		dia = null;
	}

	public Intro(string d)
	{
		//Si el archivo actualizado existe en el disco duro, lo cargamos
		if (System.IO.File.Exists(rutaDialogosGuardados + d))
		{
			dia = Dialogue.LoadDialogue (rutaDialogosGuardados + d);
		}
		//sino, cargamos el predeterminado
		else{
			dia = Dialogue.LoadDialogue (rutaDialogosPredeterminada + d);
		}

		prioridad = 1;
		indice_inicial = 0;
	}

	//Método utilizado para la comparación entre elementos funcione con el método Sort() de una lista DialogoEntrante
	public int CompareTo(Intro otro)
	{
		if (otro == null) return 1;

		//orden descendente
		return -1 * prioridad.CompareTo(otro.prioridad);
	}

	public Dialogue DevuelveDialogo()
	{
		return dia;
	}
}
