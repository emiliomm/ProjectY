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

	public TemaMensaje()
	{
		mensajes = new List<Mensaje>();
	}

	public void AddMensaje(Mensaje m)
	{
		mensajes.Add(m);
	}

	public string DevuelveTexto()
	{
		return texto;
	}

	public int DevuelveNumeroMensajes()
	{
		return mensajes.Count;
	}

	public string DevuelveTextoMensaje(int num_mensaje)
	{
		return mensajes[num_mensaje].DevuelveTexto();
	}

	//Devuelve el temamensaje de un xml indicado en la ruta
	public static TemaMensaje LoadTemaMensaje(string path)
	{
		TemaMensaje temaMensaje = Manager.Instance.DeserializeData<TemaMensaje>(path);

		return temaMensaje;
	}

}
