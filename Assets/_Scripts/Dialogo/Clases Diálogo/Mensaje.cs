using DialogueTree;

/*
 * 	Clase que contiene los dialogos que se muestran como opciones al acabar las intros
 */
public class Mensaje{

	public int ID;

	//-1 --> Sin grupo, otro --> con grupo
	public int IDGrupo;

	// 0 --> falso, 1 --> verdadero
	//Indica si el mensaje se va a destruir al acabar de recorrerl0
	public bool Autodestruye;

	public string texto;
	public Dialogue dia;

	public Mensaje()
	{
		dia = new Dialogue();
	}

	public Dialogue DevuelveDialogo()
	{
		return dia;
	}

	public int DevuelveIDGrupo()
	{
		return IDGrupo;
	}

	public string DevuelveTexto()
	{
		return texto;
	}

	//Marca la variable recorrido a true, indicando que el nodo ya ha sido recorrido
	//y no se volverán a comprobar algunas de sus funciones si se vuelve a recorrer en el futuro
	public void MarcarRecorrido(int pos)
	{
		dia.MarcarRecorrido(pos);
	}

	//Devuelve el mensaje de un xml indicado en la ruta
	public static Mensaje LoadMensaje(string path)
	{
		Mensaje men = Manager.Instance.DeserializeData<Mensaje>(path);

		return men;
	}

}
