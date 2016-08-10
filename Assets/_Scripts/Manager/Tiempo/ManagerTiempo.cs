/*
 * 	Clase que se encarga de controlar el tiempo cronológico en el juego
 */
public class ManagerTiempo{

	//Segundos reales que tarda en cambiar de hora el juego
	private int SegundosHora = 2;

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

	//Si los Minutos han llegado al máximo de tiempo que establece SegundosHora, devuelve true
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

		//Guardamos el progreso de las variables del tiempo
		Serialize();
	}

	//Devuelve una instancia de managertiempo de un xml indicado en la ruta
	public static ManagerTiempo LoadManagerTiempo()
	{
		ManagerTiempo managerTiempo = Manager.Instance.DeserializeData<ManagerTiempo>(Manager.rutaTiempo + "Tiempo.xml");

		return managerTiempo;
	}

	//Guarda la instancia en un fichero xml en la ruta especificada
	public void Serialize()
	{
		Manager.Instance.SerializeData(this, Manager.rutaTiempo, "Tiempo.xml"); //AÑADIR A MANAGER.RUTATIEMPO TIEMPO.XML
	}
}
