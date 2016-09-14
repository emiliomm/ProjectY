using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ManagerRutinas: MonoBehaviour
{
	//AÑADIR DIAS ¿?

	//Singleton pattern
	public static ManagerRutinas instance { get; private set; }

	private int escenaActual = 0;

	//<id_escena, lista - lugar_actual>
	//lugar_actual : lugar --> id_interactuable
	//	     			   --> coord
	//				 fecha_añadido --> indica la fecha en la que se ha añadido

	//Se recorre cada vez que se cambia de escena, eliminando los datos con una fecha_añadido anterior a info_interactuables

	//ATENCIÓN, PUEDEN ACUMULARSE CACHES ANTIGUOS DEBIDO A QUE NO ES RECORRIDO ENTERO SIEMPRE, COMO LOS OTROS, QUE HACER ¿?
	public Dictionary<int, List<LugarActual>> lugaresActuales;

	//<id_interactuable, info_interactuable>
	//info_interactuable: tipo_interactuable --> Indica el tipo de interactuable(npc u objeto)
	//					  id_escena --> indica el numero de escena actual del interactuable *Se sustituye cada vez que el interactuable avance de lugar o se cambie de rutina
	//					  id_rutina --> indica la rutina actual del interactuable *Se sustituye cada vez que el interactuable cambie rutina (ACTUALMENTE NO SE USA)
	//					  eventos --> lista con los ids de la eventos actuales *Se sustituye cada vez que el interactuable cambie de lugar
	//					  ultimaFechaCambioLugar --> indica la fecha en la que se ha cambiado el lugar por última vez *Se sustituye cada vez que el interactuable avance de lugar o cambie de rutina
	//					  ultimaFechaCambioRutina --> indica la fecha en la que se ha cambiado la rutina por última vez *Se sustituye cada vez que el interactuable cambie de rutina
	public Dictionary<int, InfoInteractuable> infoInteractuables;

	//<hora_cambio_lugar, lista - contenedor>
	//hora_cambio_lugar: 0 - 23
	//contenedor: lista - lugar_sig
	//			  lista - autorutina --> indica que rutinas han acabado y pasan a otra
	//	lugar_sig: id_rutina --> id_rutina del interactuable de la que forma parte el sig_lugar
	//			   lugar --> indica el lugar sustituido arriba
	//			   eventos --> Guarda listas de enteros con el id de los eventos del lugar

	//Se recorre cada cambio de hora, eliminando los datos con id_rutina antiguos mirando en infoInteractuable
	public Dictionary<int, Contenedor> contenedores;

	//<id_evento, eventoLista>
	//eventoLista: interactuable --> lista con las ids de los interactuable vinculados a los eventos
	//			   evento

	//Se recorre al cambiar de rutina, añadiendo los eventos de la nueva rutina y eliminando los eventos de la antigua rutina
	//se elimina si el evento no está vinculado a ningun interactuable
	public Dictionary<int, EventoLista> listaEventos;

	void Awake()
	{
		// First we check if there are any other instances conflicting
		if(instance != null && instance != this)
		{
			// If that is the case, we destroy other instances
			Destroy(gameObject);
		}

		//Singleton pattern
		instance = this;
	
		lugaresActuales = new Dictionary<int,List<LugarActual>>();
		infoInteractuables = new Dictionary<int,InfoInteractuable>();
		contenedores = new Dictionary<int,Contenedor>();
		listaEventos = new Dictionary<int,EventoLista>();
	}

	private void SetEscenaActual(int escenaActual)
	{
		this.escenaActual = escenaActual;
	}

	//Carga el interactuable en el diccionario al iniciar el juego
	public void CargarInteractuable(DatosInteractuable datosInteractuable)
	{
		InfoInteractuable infoInteractuable = new InfoInteractuable();
		infoInteractuable.SetTipoInter(datosInteractuable.tipoInter);
		infoInteractuable.SetIDRutina(datosInteractuable.IDRutinaActual);

		infoInteractuables[datosInteractuable.IDInteractuable] = infoInteractuable;

		//Si el interactuable tiene rutina, cargamos la rutina
		if(datosInteractuable.IDRutinaActual != -1)
			CargarRutina(datosInteractuable.IDRutinaActual, true, false);
	}
		
	public void CargarRutina(int IDRutina, bool inicioJuego, bool comprobacionRutinas)
	{
		Rutina rutina = Rutina.LoadRutina(Manager.rutaRutinas + IDRutina.ToString() + ".xml");
		int IDInteractuable = rutina.posLugarSiguientes[0].lugarSiguiente.lugar.IDInteractuable;

		InfoInteractuable infoInteractuable;

		if (infoInteractuables.TryGetValue(IDInteractuable, out infoInteractuable))
		{
			infoInteractuable.SetIDRutina(IDRutina);
			AddLugaresSiguientes(IDInteractuable, infoInteractuable, rutina.posLugarSiguientes, rutina.autorutina, IDRutina);

			int posRutina = CalculaPosicionRutina(rutina);

			if(comprobacionRutinas)
			{
				if(rutina.posLugarSiguientes[posRutina].hora != Manager.instance.GetHoraActual())
				{
					AddLugarActual(rutina.posLugarSiguientes[posRutina].lugarSiguiente.lugar, rutina.posLugarSiguientes[posRutina].lugarSiguiente.eventos);
				}
			}
			else
			{
				AddLugarActual(rutina.posLugarSiguientes[posRutina].lugarSiguiente.lugar, rutina.posLugarSiguientes[posRutina].lugarSiguiente.eventos);
			}

			if(!inicioJuego)
			{
				DatosInteractuable datosInteractuable = new DatosInteractuable(IDInteractuable, infoInteractuable.DevolverTipo(), IDRutina);
				datosInteractuable.Serialize();
			}
		}
	}

	private void MarcarEventosDesactualizados(InfoInteractuable infoInteractuable)
	{
		for(int i = 0; i < infoInteractuable.DevolverNumeroEventos(); i ++)
		{
			infoInteractuable.DesactualizarEvento(i);
		}
	}
		
	private void AddLugaresSiguientes(int IDInteractuable, InfoInteractuable infoInteractuable, List<PosicionLugarSiguiente> posLugarSiguiente, bool autorutina, int IDRutina)
	{
		DateTime fechaRutina = DateTime.UtcNow;
		infoInteractuable.SetFechaCambioRutina(fechaRutina);
	
		for(int i = 0; i < posLugarSiguiente.Count; i++)
		{
			Contenedor contenedor;

			if (!contenedores.TryGetValue(posLugarSiguiente[i].hora, out contenedor))
			{
				contenedor = new Contenedor();
				contenedores.Add(posLugarSiguiente[i].hora, contenedor);
			}
				
			posLugarSiguiente[i].lugarSiguiente.setFechaRutina(fechaRutina);
			contenedor.AddLugarSig(posLugarSiguiente[i].lugarSiguiente);
		}

		if(autorutina)
			CargarAutorutina(IDRutina, fechaRutina);
	}
		
	private void CargarAutorutina(int IDRutina, DateTime fechaRutina)
	{
		Autorutina autorutina;
		int poshora;

		if (System.IO.File.Exists(Manager.rutaAutorutinasGuardadas + IDRutina.ToString() + ".xml"))
		{
			autorutina = Autorutina.LoadAutoRutina(Manager.rutaAutorutinasGuardadas + IDRutina.ToString() + ".xml");

			poshora = autorutina.posHora;
		}
		else
		{
			autorutina = Autorutina.LoadAutoRutina(Manager.rutaAutorutinas + IDRutina.ToString() + ".xml");

			int hora = Manager.instance.GetHoraActual();
			int numRecorridosMaximos = autorutina.numHoras / 24;

			if(autorutina.numHoras % 24 != 0)
				numRecorridosMaximos++;

			poshora = hora + autorutina.numHoras % 24;

			autorutina.posHora = poshora;
			autorutina.numRecorridosActuales = 0;
			autorutina.numRecorridosMaximos = numRecorridosMaximos;

			autorutina.Serialize(); //Guardamos los datos una vez se haya cargado la autorutina
		}

		autorutina.SetFechaRutina(fechaRutina);

		Contenedor contenedor;

		if (!contenedores.TryGetValue(poshora, out contenedor))
		{
			contenedor = new Contenedor();
			contenedores.Add(poshora, contenedor);
		}

		contenedor.AddAutorutina(autorutina);
	}

	private void CargarEvento(int IDEvento, int IDInteractuable)
	{
		EventoLista eventoLista;

		//Si no existe el evento, lo añadimos
		if (!listaEventos.TryGetValue(IDEvento, out eventoLista))
		{
			Evento evento;

			//Miramos primero en la lista de grupos modificados
			if (System.IO.File.Exists (Manager.rutaEventosGuardados + IDEvento.ToString () + ".xml"))
			{
				evento = Evento.LoadEvento (Manager.rutaEventosGuardados + IDEvento.ToString () + ".xml");
			}
			//Si no está ahí, miramos en el directorio predeterminado
			else
			{
				evento = Evento.LoadEvento (Manager.rutaEventos + IDEvento.ToString () + ".xml");
			}

			eventoLista = new EventoLista(IDInteractuable, evento);

			listaEventos.Add(IDEvento, eventoLista);
		}
		else
		{
			eventoLista.AddInteractuable(IDInteractuable);
		}
	}

	private void AddEventoAInteractuable(InfoInteractuable infoInteractuable, int IDEvento)
	{
		infoInteractuable.AddEvento(IDEvento);
	}

	private void EliminarEventosDesactualizados(int IDInteractuable, InfoInteractuable infoInteractuable)
	{
		for(int i = infoInteractuable.DevolverNumeroEventos() - 1; i >= 0; i--)
		{
			if(!infoInteractuable.DevuelveEventoActualizado(i))
			{
				int IDEvento = infoInteractuable.DevuelveIDEvento(i);
				infoInteractuable.EliminarEvento(i);

				EventoLista eventoLista;

				//Si existe
				if (listaEventos.TryGetValue(IDEvento, out eventoLista))
				{
					int pos = eventoLista.PosicionInteractuable(IDInteractuable);

					if(pos != -1)
					{
						eventoLista.BorrarInteractuable(pos);
						if(eventoLista.IsInterEmpty())
						{
							listaEventos.Remove(IDEvento);
						}
					}
				}
			}
		}
	}

	private int CalculaPosicionRutina(Rutina rutina)
	{
		int horaActual = Manager.instance.GetHoraActual();
		int posRutina = -1;

		if(horaActual < rutina.posLugarSiguientes[0].hora)
		{
			posRutina = rutina.posLugarSiguientes.Count - 1;
		}
		else
		{
			for(int i = 0; i < rutina.posLugarSiguientes.Count; i++)
			{
				if(horaActual >= rutina.posLugarSiguientes[i].hora)
				{
					posRutina++;
				}
			}
		}

		return posRutina;
	}
		
	private void AddLugarActual(Lugar lugar, List<int> eventos)
	{
		DateTime fechaActual = DateTime.UtcNow;

		int IDInteractuable = lugar.IDInteractuable;
		int IDEscena = lugar.IDEscena;

		LugarActual lugarActual = new LugarActual(lugar, fechaActual);

		List<LugarActual> lista;
		if (!lugaresActuales.TryGetValue(IDEscena, out lista))
		{
			lista = new List<LugarActual>();
			lugaresActuales.Add(IDEscena, lista);
		}

		lista.Add(lugarActual);

		InfoInteractuable infoInteractuable;
		if (infoInteractuables.TryGetValue(IDInteractuable, out infoInteractuable))
		{
			ComprobarInteractuableEscenaActual(IDInteractuable, infoInteractuable.DevolverTipo(), infoInteractuable.DevolverIDEscena(), IDEscena, new Vector3(lugar.coordX, lugar.coordY, lugar.coordZ), new Quaternion(lugar.rotX, lugar.rotY, lugar.rotZ, lugar.rotW));
			infoInteractuable.SetIDEscena(IDEscena);
			infoInteractuable.SetFechaCambioLugar(fechaActual);

			MarcarEventosDesactualizados(infoInteractuable);

			for(int j = 0; j < eventos.Count; j++)
			{
				CargarEvento(eventos[j], IDInteractuable);
				AddEventoAInteractuable(infoInteractuable, eventos[j]);
			}

			EliminarEventosDesactualizados(IDInteractuable, infoInteractuable);
		}
	}

	private void ComprobarInteractuableEscenaActual(int IDInteractuable, int tipoInteractuable, int IDEscenaAnterior, int IDEscenaActual, Vector3 coord, Quaternion rot)
	{
		if(IDEscenaAnterior == escenaActual)
		{
			//El interactuable se mueve desde un punto de la escena actual a otro
			if(IDEscenaAnterior == IDEscenaActual)
			{
				Manager.instance.MoverInteractuableEnEscena(tipoInteractuable, IDInteractuable, coord, rot);
			}
			//El interactuable debe abandonar la escena actual hacia otra
			else
			{
				Manager.instance.MoverInteractuableHaciaOtraEscena(tipoInteractuable, IDInteractuable, IDEscenaActual);
			}
		}
		//El interactuable no existe, se crea en la escena
		else if(IDEscenaActual == escenaActual)
		{
			Manager.instance.MoverInteractuableDesdeOtraEscena(IDInteractuable, tipoInteractuable, IDEscenaAnterior, coord, rot);
		}
	}
		
	public void ComprobarRutinas(int horaActual)
	{
		Contenedor contenedor;

		if (contenedores.TryGetValue(horaActual, out contenedor))
		{
			List<Autorutina> listaAutorutinas = contenedor.DevuelveAutorutina();

			if(listaAutorutinas != null)
			{
				ComprobarAutorutinas(listaAutorutinas);
			}

			List<LugarSiguiente> listaLugarSiguiente = contenedor.DevuelveLugarSiguientes();

			if(listaLugarSiguiente != null)
			{
				ComprobarLugaresSiguientes(listaLugarSiguiente);
			}
		}
	}

	public void ComprobarEventosInicio(int horaActual)
	{
		Contenedor contenedor;

		if (contenedores.TryGetValue(horaActual, out contenedor))
		{
			List<LugarSiguiente> listaLugarSiguiente = contenedor.DevuelveLugarSiguientes();

			for(int i = listaLugarSiguiente.Count - 1; i >= 0; i--)
			{
				int IDInteractuable = listaLugarSiguiente[i].lugar.IDInteractuable;
				InfoInteractuable infoInteractuable;
				if (infoInteractuables.TryGetValue(IDInteractuable, out infoInteractuable))
				{
					//Ejecutamos los eventos del lugar actual
					ComprobarEventos(listaLugarSiguiente[i].eventos);
				}
			}
		}
	}

	private void ComprobarAutorutinas(List<Autorutina> listaAutorutinas)
	{
		for(int i = listaAutorutinas.Count - 1; i >= 0; i--)
		{
			InfoInteractuable infoInteractuable;
			if (infoInteractuables.TryGetValue(listaAutorutinas[i].IDInteractuable, out infoInteractuable))
			{
				//Comprueba la fecha de la rutina
				if(listaAutorutinas[i].GetFechaRutina() == infoInteractuable.GetFechaCambioRutina())
				{
					if(listaAutorutinas[i].Recorrido())
					{
						CargarRutina(listaAutorutinas[i].IDSigRutina, false, true);
						listaAutorutinas.RemoveAt(i);
					}
				}
				//El interactuable no tiene la misma rutina que la que marca la autorutina, la destruimos
				else
				{
					listaAutorutinas.RemoveAt(i);
				}
			}
			//El interactuable ya no existe, destruimos la autorutina
			//(ESTE CASO AÚN NO SE PRODUCE)
			else
			{
				listaAutorutinas.RemoveAt(i);
			}
		}
	}

	private void ComprobarLugaresSiguientes(List<LugarSiguiente> listaLugarSiguiente)
	{
		for(int i = listaLugarSiguiente.Count - 1; i >= 0; i--)
		{
			int IDInteractuable = listaLugarSiguiente[i].lugar.IDInteractuable;
			InfoInteractuable infoInteractuable;
			if (infoInteractuables.TryGetValue(IDInteractuable, out infoInteractuable))
			{
				//Si las fechas de rutina no coinciden, eliminamos el lugarSiguiente de la lista
				if (infoInteractuable.GetFechaCambioRutina() != listaLugarSiguiente[i].getFechaRutina())
				{
					listaLugarSiguiente.RemoveAt(i);
				}
				else
				{
					//Añadimos el lugar Actual
					AddLugarActual(listaLugarSiguiente[i].lugar, listaLugarSiguiente[i].eventos);

					//Ejecutamos los eventos del lugar actual
					ComprobarEventos(listaLugarSiguiente[i].eventos);
				}
			}
		}
	}

	private void ComprobarEventos(List<int> eventos)
	{
		EventoLista eventoLista;
		Evento evento;

		for(int i = 0; i < eventos.Count; i++)
		{
			if (listaEventos.TryGetValue(eventos[i], out eventoLista))
			{
				evento = eventoLista.DevuelveEvento();

				if(evento.activo)
					evento.EjecutarEvento();
			}
		}
	}

	public void CargarEscena(int escena)
	{
		//Establecemos la escena actual
		SetEscenaActual(escena);

		List<LugarActual> listaLugarActual;
		if (lugaresActuales.TryGetValue(escena, out listaLugarActual))
		{
			for(int i = listaLugarActual.Count - 1; i >= 0; i--)
			{
				int IDInteractuable = listaLugarActual[i].GetIDInteractuable();
				InfoInteractuable infoInteractuable;
				if (infoInteractuables.TryGetValue(IDInteractuable, out infoInteractuable))
				{
					if(infoInteractuable.GetFechaCambioLugar() > listaLugarActual[i].GetFecha())
					{
						listaLugarActual.RemoveAt(i);
					}
					else
					{
						int tipo = infoInteractuable.DevolverTipo();

						Manager.instance.CrearInteractuable(IDInteractuable, tipo, listaLugarActual[i].GetCoordenadasLugar(), listaLugarActual[i].GetCoordenadasRotacion());
					}
				}
			}
		}
	}

	//Devuelve un evento vacío si no es encontrado o si el evento no está activo
	public Evento DevuelveEvento(int IDEvento)
	{
		Evento evento = null;
		EventoLista eventoListo;

		if (listaEventos.TryGetValue(IDEvento, out eventoListo))
		{
			evento = eventoListo.DevuelveEvento();
			if(!evento.activo)
				evento = null;
		}

		return evento;
	}

	public void GuardaEvento(int IDEvento)
	{
		EventoLista eventoLista;
		Evento evento;

		if (listaEventos.TryGetValue(IDEvento, out eventoLista))
		{
			evento = eventoLista.DevuelveEvento();
			evento.Serialize();
		}
	}

	//FUNCIÓN DEBUG
	public void RecorrerLugaresActuales()
	{
//		foreach(KeyValuePair<int, List<Lugar_Actual>> entry in lugaresActuales)
//		{
//			// do something with entry.Value or entry.Key
//			for(int i = 0; i < entry.Value.Count; i++)
//			{
//				Debug.Log(entry.Key);
//				Debug.Log("Coordenadas: " + entry.Value[i].getCoordLugar());
//				Debug.Log("----------------------");
//			}
//			Debug.Log("************************");
//		}
//		Debug.Log("__________________________");
	}
}