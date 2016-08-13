using UnityEngine;

/*
 * 	Clase hija de Interactuable que contiene datos sobre la subclase de los interactuables llamada NPC
 */
public class InteractuableNPC : Interactuable {

	NPCDatos datos; //Almacena los datos de esta clase

	NavMeshAgent agent; //Agente de prueba

	protected override void Start()
	{
		//Ejecuta el metodo del padre
		base.Start();

		//Carga los datos del directorio predeterminado o del de guardado si hay datos guardados
		if (System.IO.File.Exists(Manager.rutaNPCDatosGuardados + ID.ToString()  + ".xml"))
		{
			datos = NPCDatos.LoadInterDatos(Manager.rutaNPCDatosGuardados + ID.ToString()  + ".xml");
		}
		else
		{
			datos = NPCDatos.LoadInterDatos(Manager.rutaNPCDatos + ID.ToString()  + ".xml");
		}

		agent = GetComponent<NavMeshAgent>();
//		agent.SetDestination(new Vector3(0f,0f,0f)); para mover el interactuable al lugar indicado

		//Establece el nombre del interactuable
		SetNombre(datos.DevuelveNombreActual());
	}

	//prueba
	public void setRuta(Vector3 ruta)
	{
		agent.enabled = true;
		agent.SetDestination(ruta);// para mover el interactuable al lugar indicado
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
