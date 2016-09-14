using DialogueTree;

using System.Collections.Generic;

/*
 * 	Clase que contiene los dialogos que se muestran como opciones al acabar las intros
 */
public class MensajeDialogo : Mensaje
{
	public Dialogue dialogo;

	public MensajeDialogo()
	{
		dialogo = new Dialogue();
	}

	public Dialogue DevuelveDialogo()
	{
		return dialogo;
	}
}
