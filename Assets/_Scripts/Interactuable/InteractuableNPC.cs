using UnityEngine;

/*
 * 	Clase hija de Interactuable que contiene datos sobre la subclase de los interactuables llamada NPC
 */
public class InteractuableNPC : Interactuable {

	private NavMeshAgent agente; //Agente que permite al interactuable moverse por la escena
	private NPCDatos datos; //Almacena los datos de esta clase

	protected override void Start()
	{
		//Carga los datos del directorio predeterminado o del de guardado si hay datos guardados
		if (System.IO.File.Exists(Manager.rutaInterDatosGuardados + ID.ToString()  + ".xml"))
		{
			datos = NPCDatos.LoadInterDatos(Manager.rutaInterDatosGuardados + ID.ToString()  + ".xml");
		}
		else
		{
			datos = NPCDatos.LoadInterDatos(Manager.rutaInterDatos + ID.ToString()  + ".xml");
		}

		//Ejecuta el metodo del padre
		base.Start();

		agente = GetComponent<NavMeshAgent>();

		//Establece el nombre del interactuable
		SetNombre(datos.DevuelveNombreActual());
	}
		
	//Establece la ruta a seguir por el interactuable hacia la posición indicada
	public void SetRuta(Vector3 ruta)
	{
		//Comprobamos que el agent está inicializado, ya que hay casos en los que la función es llamada
		//antes de Start()
		if(agente == null)
			agente = GetComponent<NavMeshAgent>();

		//Añadimos el NavMesh a la lista del Manager con NavMeshActivos
		Manager.instance.AddNavMeshAgent(agente);

		//Activamos el agente y establecemos la ruta
		agente.enabled = true;
		agente.SetDestination(ruta);// para mover el interactuable al lugar indicado
	}

	public NavMeshAgent DevuelveNavhMeshAgent()
	{
		return agente;
	}

	public int DevuelveIndiceNombre()
	{
		return datos.DevuelveIndiceNombre();
	}

	public void SetIndiceNombre(int num)
	{
		datos.SetIndiceNombre(num);
	}

	public string DevuelveNombreActual()
	{
		return datos.DevuelveNombreActual();
	}

	//Igual que la función DevuelveNombreActual() es este caso, pero necesaria de implementar por la clase base
	public override string DevuelveNombreDialogo()
	{
		return DevuelveNombreActual();
	}

	//Añade los datos a la cola de objetos para que sean serializados
	public void AddDatosToColaObjetos()
	{
		datos.AddToColaObjetos();
	}
}
