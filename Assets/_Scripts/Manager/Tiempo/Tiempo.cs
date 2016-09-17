using UnityEngine;

/*
 * 	Clase que se encarga de controlar el tiempo cronológico en el juego
 */
public class Tiempo
{
	public int dia = 0;
	public int hora = 0;
	public int minuto = 0;

	public Tiempo()
	{
		
	}

	public int GetHora()
	{
		return hora;
	}

	public int GetMinuto()
	{
		return minuto;
	}

	public bool AvanzaMinuto()
	{
		bool avanzaHora = false;

		minuto++;

		if(minuto >= 60)
		{
			minuto = 0;
			avanzaHora = true;
		}

		return avanzaHora;
	}

	public void AvanzaHora()
	{
		hora++;
		hora = hora % 24;

		if(hora == 0)
			dia++;

		Serialize();
	}

	//Devuelve una instancia de managertiempo de un xml indicado en la ruta
	public static Tiempo LoadTiempo()
	{
		Tiempo tiempo = Manager.instance.DeserializeData<Tiempo>(Manager.rutaTiempo + "Tiempo.xml");

		return tiempo;
	}

	//Guarda la instancia en un fichero xml en la ruta especificada
	public void Serialize()
	{
		Manager.instance.SerializeData(this, Manager.rutaTiempo, "Tiempo.xml"); //AÑADIR A MANAGER.RUTATIEMPO TIEMPO.XML
	}
}
