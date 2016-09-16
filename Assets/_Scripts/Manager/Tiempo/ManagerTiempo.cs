using UnityEngine;

/*
 * 	Clase que se encarga de controlar el tiempo cronológico en el juego
 */
public class ManagerTiempo
{
	//Segundos reales que tarda en cambiar de hora el juego
	private int duracionHoraEnSegundosReales = 2;

	public int dia = 0;
	public int hora = 0;
	public int minutos = 0;

	public ManagerTiempo()
	{
		
	}

	public int GetHora()
	{
		return hora;
	}

	//Si los Minutos han llegado al máximo de tiempo que establece SegundosHora, devuelve true
	public bool ContinuaHora()
	{
		bool continua = false;

		if(minutos == duracionHoraEnSegundosReales)
		{
			continua = true;
		}

		return continua;
	}

	public void AvanzaMinutos()
	{
		minutos++;
	}

	public void AvanzaHora()
	{
		hora++;
		hora = hora % 24;

		if(hora == 0)
			dia++;

		minutos = 0;

		//Guardamos el progreso de las variables del tiempo
		Serialize();
	}

	//Devuelve una instancia de managertiempo de un xml indicado en la ruta
	public static ManagerTiempo LoadManagerTiempo()
	{
		ManagerTiempo managerTiempo = Manager.instance.DeserializeData<ManagerTiempo>(Manager.rutaTiempo + "Tiempo.xml");

		return managerTiempo;
	}

	//Guarda la instancia en un fichero xml en la ruta especificada
	public void Serialize()
	{
		Manager.instance.SerializeData(this, Manager.rutaTiempo, "Tiempo.xml"); //AÑADIR A MANAGER.RUTATIEMPO TIEMPO.XML
	}
}
