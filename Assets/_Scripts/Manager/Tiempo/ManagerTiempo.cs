using UnityEngine;
using System.Collections;

public class ManagerTiempo : MonoBehaviour {

	//Singleton pattern
	public static ManagerTiempo instance { get; private set; }

	private Tiempo tiempo; //Controla el tiempo cronológico del juego

	//0.033 * 60 minutos = 1,98 seg por hora
	public float duracionMinutoEnSegundosReales = 0.033f;

	//Estados del Manager
	public enum Estado
	{
		Pausa, Activo
	}

	public Estado state {get; set; }
	public Estado prevState {get; set; }

	public void SetState(Estado newState) {
		prevState = state;
		state = newState;
	}

	private void Awake()
	{
		// First we check if there are any other instances conflicting
		if(instance != null && instance != this)
		{
			// If that is the case, we destroy other instances
			Destroy(gameObject);
		}

		//Singleton pattern
		instance = this;

		SetState(Estado.Activo);

		CargarTiempo();
	}

	private void CargarTiempo()
	{
		tiempo = new Tiempo();

		ComprobarArchivosDirectorios();

		StartCoroutine(AvanzaMinuto());
	}

	private void ComprobarArchivosDirectorios()
	{
		if (!System.IO.Directory.Exists(Manager.rutaTiempo))
		{
			System.IO.Directory.CreateDirectory(Manager.rutaTiempo);
		}
		else if(System.IO.File.Exists(Manager.rutaTiempo + "Tiempo.xml"))
		{
			tiempo = Tiempo.LoadTiempo();
		}
	}

	public int GetHoraActual()
	{
		return tiempo.GetHora();
	}

	public int GetMinutoActual()
	{
		return tiempo.GetMinuto();
	}

	public IEnumerator AvanzaMinuto()
	{
		while(true)
		{
			yield return new WaitForSeconds (duracionMinutoEnSegundosReales);

			switch(state)
			{
			case Estado.Activo:
				//Si pasa una hora
				if(tiempo.AvanzaMinuto())
				{
					AvanzaHora();
				}
				break;
			case Estado.Pausa:
				break;
			}
		}
	}

	public void AvanzaHora()
	{
		tiempo.AvanzaHora();

		//Comprobamos que rutinas avanzamos
		ManagerRutina.instance.ComprobarRutinas(ManagerTiempo.instance.GetHoraActual());
	}

	//MIRAR SI SE PUEDE ESTANDARIZAR, AÑADIR COSAS QUE SE LLAMAN AL USAR ESTA FUNCIÓN
	//Establece el estado de pausa
	public void SetPausa(bool pausa)
	{
		if(pausa) {
			SetState (Estado.Pausa);
			Time.timeScale = 0.0f;
		}
		else {
			SetState (Estado.Activo);
			Time.timeScale = 1.0f;
		}
	}

	public void GuardarTiempo()
	{
		tiempo.Serialize();
	}
}
