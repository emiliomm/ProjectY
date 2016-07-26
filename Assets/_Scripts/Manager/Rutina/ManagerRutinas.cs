using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ManagerRutinas{

	//AÑADIR DIAS ¿?

	private int escenaActual = 0;

	//<id_escena, lugar_actual>
	//lugar_actual : lugar --> id_interactuable
	//	     			   --> coord
	//				 fecha_añadido --> indica la fecha en la que se ha añadido

	//Se recorre cada vez que se cambia de escena, eliminando los datos con una fecha_añadido anterior a info_interactuables

	//ATENCIÓN, PUEDEN ACUMULARSE CACHES ANTIGUOS DEBIDO A QUE NO ES RECORRIDO ENTERO SIEMPRE, COMO LOS OTROS, QUE HACER ¿?
	public Dictionary<int,List<Lugar_Actual>> lugaresActuales;

	//<id_interactuable, info_interactuable>
	//info_interactuable: tipo_interactuable --> Indica el tipo de interactuable(npc u objeto)
	//					  id_escena --> indica el numero de escena actual del interactuable *Se sustituye cada vez que el interactuable avance de lugar o se cambie de rutina
	//					  id_rutina --> indica la rutina actual del interactuable *Se sustituye cada vez que el interactuable cambie rutina
	//					  eventos --> lista con los ids de la eventos actuales *Se sustituye cada vez que el interactuable cambie rutina
	//					  fecha_añadido --> indica la fecha en la que se ha cambiado el lugar por última vez *Se sustituye cada vez que el interactuable avance de lugar o cambie de rutina
	public Dictionary<int,Info_Interactuable> infoInteractuable;

	//<hora_cambio_lugar, sig_lugar>
	//hora_cambio_lugar: 0 - 23
	//sig_lugar: id_rutina --> id_rutina del interactuable de la que forma parte el sig_lugar
	//			 lugar --> indica el lugar sustituido arriba
	//			 eventos --> Guarda listas de enteros con el id de los eventos del lugar

	//Se recorre cada cambio de hora, eliminando los datos con id_rutina antiguos mirando en infoInteractuable
	public Dictionary<int,List<Lugar_Siguiente>> lugaresSiguientes;

	//<id_evento, eventoLista>
	//eventoLista: interactuable --> lista con las ids de los interactuable vinculados a los eventos
	//			   evento

	//Se recorre al cambiar de rutina, añadiendo los eventos de la nueva rutina y eliminando los eventos de la antigua rutina
	//se elimina si el evento no está vinculado a ningun interactuable
	public Dictionary<int, EventoLista> listaEventos;

	public ManagerRutinas()
	{
		lugaresActuales = new Dictionary<int,List<Lugar_Actual>>();
		infoInteractuable = new Dictionary<int,Info_Interactuable>();
		lugaresSiguientes = new Dictionary<int,List<Lugar_Siguiente>>();
		listaEventos = new Dictionary<int,EventoLista>();
	}

	private void setEscenaActual(int eActual)
	{
		escenaActual = eActual;
	}

	//Carga el interactuable en el diccionario al iniciar el juego
	public void cargarInteractuable(Datos_Interactuable dInt)
	{
		Info_Interactuable infoInter = new Info_Interactuable();
		infoInter.setTipoInter(dInt.tipoInter);
		infoInter.setIDRutina(dInt.IDRutinaActual);

		infoInteractuable[dInt.IDInter] = infoInter;

		cargarRutina(infoInter.devolverIDRutina());
	}

	public void cargarRutina(int idRutina)
	{
		Rutina rutina = Manager.Instance.DeserializeData<Rutina>(Manager.rutaRutinas + idRutina.ToString() + ".xml");
		int IDInter = rutina.posLugarSiguientes[0].lugarSiguiente.lugar.IDInter;

		Info_Interactuable infoInter;

		if (infoInteractuable.TryGetValue(IDInter, out infoInter))
		{
			infoInter.setIDRutina(idRutina);
			MarcarEventosDesactualizados(infoInter);
			AddLugaresSiguientes(IDInter, infoInter, rutina.posLugarSiguientes);
			AddLugarActual(rutina.posLugarSiguientes[CalculaPosicionRutina(rutina)].lugarSiguiente.lugar);
		}
	}

	private void MarcarEventosDesactualizados(Info_Interactuable infoInter)
	{
		for(int i = 0; i < infoInter.devolverNumeroEventos(); i ++)
		{
			infoInter.actualizarEvento(i);
		}
	}

	private void AddLugaresSiguientes(int IDInter, Info_Interactuable infoInter, List<PosicionLugarSiguiente> posLugarSiguiente)
	{
		for(int i = 0; i < posLugarSiguiente.Count; i++)
		{
			List<Lugar_Siguiente> lugSiguientes;

			if (!lugaresSiguientes.TryGetValue(posLugarSiguiente[i].hora, out lugSiguientes))
			{
				lugSiguientes = new List<Lugar_Siguiente>();
				lugaresSiguientes.Add(posLugarSiguiente[i].hora, lugSiguientes);
			}

			lugSiguientes.Add(posLugarSiguiente[i].lugarSiguiente);

			for(int j = 0; j < posLugarSiguiente[i].lugarSiguiente.eventos.Count; j++)
			{
				cargarEvento(posLugarSiguiente[i].lugarSiguiente.eventos[j], IDInter);
				AddEventoAInter(infoInter, posLugarSiguiente[i].lugarSiguiente.eventos[j]);
			}
		}

		EliminarEventosDesactualizados(IDInter, infoInter);
	}

	private void cargarEvento(int IDEvento, int IDInter)
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

			eventoLista = new EventoLista(IDInter, evento);

			listaEventos.Add(IDEvento, eventoLista);
		}
		else
		{
			eventoLista.addInter(IDInter);
		}
	}

	private void AddEventoAInter(Info_Interactuable infoInter, int IDEvento)
	{
		infoInter.addEvento(IDEvento);
	}

	private void EliminarEventosDesactualizados(int IDInter, Info_Interactuable infoInter)
	{
		for(int i = infoInter.devolverNumeroEventos() - 1; i >= 0; i--)
		{
			if(!infoInter.devuelveEventoActualizado(i))
			{
				int IDEvento = infoInter.devuelveIDEvento(i);
				infoInter.eliminarEvento(i);

				EventoLista eventoLista;

				//Si existe
				if (listaEventos.TryGetValue(IDEvento, out eventoLista))
				{
					int pos = eventoLista.posicionInter(IDInter);

					if(pos != -1)
					{
						eventoLista.borrarInter(pos);
						if(eventoLista.isInterEmpty())
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
		int horaActual = Manager.Instance.getHoraActual();
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

	private void AddLugarActual(Lugar lugar)
	{
		DateTime fechaActual = DateTime.Now;

		int IDInter = lugar.IDInter;
		int IDEscena = lugar.IDEscena;

		Lugar_Actual lAct = new Lugar_Actual(lugar, fechaActual);

		List<Lugar_Actual> lista;
		if (!lugaresActuales.TryGetValue(IDEscena, out lista))
		{
			lista = new List<Lugar_Actual>();
			lugaresActuales.Add(IDEscena, lista);
		}

		lista.Add(lAct);

		Info_Interactuable infoInter;
		if (infoInteractuable.TryGetValue(IDInter, out infoInter))
		{
			ComprobarInteractuableEscenaActual(IDInter, infoInter.devolverTipo(), infoInter.devolverIDEscena(), IDEscena, new Vector3(lugar.coordX, lugar.coordY, lugar.coordZ));
			infoInter.setIDEscena(IDEscena);
			infoInter.setFecha(fechaActual);
		}
	}

	private void ComprobarInteractuableEscenaActual(int IDInter, int tipoInter, int IDEscenaAnterior, int IDEscenaActual, Vector3 coord)
	{
		if(IDEscenaAnterior == escenaActual)
		{
			if(IDEscenaAnterior == IDEscenaActual)
			{
				Manager.Instance.moverInteractuable(IDInter, coord);
			}
			else
			{
				Manager.Instance.destruirInteractuable(IDInter);
			}
		}
		else if(IDEscenaActual == escenaActual)
		{
			Manager.Instance.crearInteractuable(IDInter, tipoInter, coord);
		}
	}

	public void ComprobarRutinas(int horaActual)
	{
		List<Lugar_Siguiente> lista;

		if (lugaresSiguientes.TryGetValue(horaActual, out lista))
		{
			for(int i = lista.Count - 1; i >= 0; i--)
			{
				int IDInter = lista[i].lugar.IDInter;
				Info_Interactuable infoInter;
				if (infoInteractuable.TryGetValue(IDInter, out infoInter))
				{
					if (infoInter.devolverIDRutina() != lista[i].IDRutina)
					{
						lista.RemoveAt(i);
					}
					else
					{
						//Ejecutamos los eventos
						ComprobarEventos();

						//Añadimos el lugar Actual
						AddLugarActual(lista[i].lugar);
					}
				}
			}
		}
	}

	private void ComprobarEventos()
	{
		
	}

	public void CargarEscena(int escena)
	{
		//Establecemos la escena actual
		setEscenaActual(escena);

		List<Lugar_Actual> lista;
		if (lugaresActuales.TryGetValue(escena, out lista))
		{
			for(int i = lista.Count - 1; i >= 0; i--)
			{
				int IDInter = lista[i].getIDInter();
				Info_Interactuable infoInter;
				if (infoInteractuable.TryGetValue(IDInter, out infoInter))
				{
					if(infoInter.getFecha() > lista[i].getFecha())
					{
						lista.RemoveAt(i);
					}
					else
					{
						int tipo = infoInter.devolverTipo();

						Manager.Instance.crearInteractuable(IDInter, tipo, lista[i].getCoordLugar());
					}
				}
			}
		}
	}
}