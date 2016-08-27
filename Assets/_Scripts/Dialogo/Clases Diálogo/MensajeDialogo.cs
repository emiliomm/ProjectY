using DialogueTree;

using System.Collections.Generic;

/*
 * 	Clase que contiene los dialogos que se muestran como opciones al acabar las intros
 */
public class MensajeDialogo : Mensaje
{
	public Dialogue dia;

	public MensajeDialogo()
	{
		dia = new Dialogue();
	}

	public Dialogue DevuelveDialogo()
	{
		return dia;
	}
}
