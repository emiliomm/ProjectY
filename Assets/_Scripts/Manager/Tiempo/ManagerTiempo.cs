using UnityEngine;
using System.Collections;

public class ManagerTiempo{

	//Segundos reales que tarda en cambiar de hora el juego
	private int SegundosHora = 60;

	public int Dia = 0;
	public int Hora = 0;
	public int Minutos = 0;

	public ManagerTiempo()
	{
		
	}

	public int getHora()
	{
		return Hora;
	}

	public bool continuaHora()
	{
		bool continua = false;

		if(Minutos == SegundosHora)
		{
			continua = true;
		}

		return continua;
	}

	public void avanzaMinutos()
	{
		Minutos++;
	}

	public void avanzaHora()
	{
		Hora++;
		Hora = Hora % 24;

		if(Hora == 0)
			Dia++;

		Minutos = 0;

		//Guardamos el progreso
		Serialize();
	}

	public static ManagerTiempo LoadManagerTiempo()
	{
		ManagerTiempo managerTiempo = Manager.Instance.DeserializeData<ManagerTiempo>(Manager.rutaTiempo + "Tiempo.xml");

		return managerTiempo;
	}

	public void Serialize()
	{
		Manager.Instance.SerializeData(this, Manager.rutaTiempo, "Tiempo.xml");
	}
}
