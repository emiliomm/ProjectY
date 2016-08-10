using DialogueTree;

/*
 * 	Clase que contiene los dialogos que se muestran al principio de las conversaciones
 */
public class Intro{

	public int ID;

	//-1 --> Sin grupo, otro --> con grupo
	public int IDGrupo;

	// 0 --> falso, 1 --> verdadero
	//Indica si la intro se va a destruir al acabar de recorrerla
	public bool Autodestruye;

	public int prioridad; //mayor prioridad, aparece primero en el diálogo
	public int indice_inicial; //indice del dialogo por el que empieza la intro (AUN NO IMPLEMENTADO)
	public Dialogue dia;

	public Intro()
	{
		dia = new Dialogue();
	}

	public Dialogue DevuelveDialogo()
	{
		return dia;
	}

	public int DevuelvePrioridad()
	{
		return prioridad;
	}

	public int DevuelveIDGrupo()
	{
		return IDGrupo;
	}

	//Marca la variable recorrido a true, indicando que el nodo ya ha sido recorrido
	//y no se volverán a comprobar algunas de sus funciones si se vuelve a recorrer en el futuro
	public void MarcarRecorrido(int num)
	{
		dia.MarcarRecorrido(num);
	}

	//Devuelve la intro de un xml indicado en la ruta con la prioridad indicada
	public static Intro LoadIntro(string path, int prioridad)
	{
		Intro intro = Manager.Instance.DeserializeData<Intro>(path);

		intro.prioridad = prioridad;

		return intro;
	}
}
