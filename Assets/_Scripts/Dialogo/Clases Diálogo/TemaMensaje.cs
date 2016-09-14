using System.Collections.Generic;

/*
 * 	Clase que contiene mensajes relacionados con un tema común
 */
public class TemaMensaje{

	public int ID; //ID Tema

	//-1 si no tiene grupo, x si tiene. El grupo del TemaMensaje sustituye al grupo de los mensajes, excepto si el temamensaje no tiene grupo
	public int IDGrupo;

	public string texto;
	public List<Mensaje> mensajes;

	private bool visible;

	public TemaMensaje()
	{
		mensajes = new List<Mensaje>();
	}

	public void AddMensaje(Mensaje mensaje)
	{
		mensajes.Add(mensaje);
	}

	public string DevuelveTexto()
	{
		return texto;
	}

	public int DevuelveNumeroMensajes()
	{
		return mensajes.Count;
	}

	//Devuelve el número de mensajes que están activos, es decir, que se pueden mostrar actualmente
	//(Ya sea porque su evento está activo, las variables del evento permiten mostrarla o no tienen un evento vinculado)
	public int DevuelveNumeroMensajesActivos()
	{
		int count = 0;

		for(int i = 0; i < DevuelveNumeroMensajes(); i++)
		{
			if(mensajes[i].SeMuestra())
			{
				count++;
			}
		}

		if(count == 0)
			visible = false;
		else
			visible = true;

		return count;
	}

	public bool EstadoVisible()
	{
		return visible;
	}

	public string DevuelveTextoMensaje(int numMensaje)
	{
		return mensajes[numMensaje].DevuelveTexto();
	}

	//Devuelve el temamensaje de un xml indicado en la ruta
	public static TemaMensaje LoadTemaMensaje(string path)
	{
		TemaMensaje temaMensaje = Manager.Instance.DeserializeData<TemaMensaje>(path);

		return temaMensaje;
	}

}
