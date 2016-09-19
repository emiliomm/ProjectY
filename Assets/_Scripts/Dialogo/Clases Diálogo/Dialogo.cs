using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization; 
using System.IO; 
using System.Linq;
using System;

using DialogueTree;

/*
 * 	Clase que contiene la conversación y sus datos
 * 
 *  Es una clase derivada de ObjetoSer, que permite que esta clase sea añadida a una cola con objetos a serializar
 *  También se reescribe el nombre a ObjetoSer al ser serializado en un xml para que funcione la serialización/deserialización de objetos de clase ObjetoSerializable
 */

[XmlRoot("ObjetoSerializable")]
public class Dialogo : ObjetoSerializable
{
	public int IDInteractuable; //ID del Interactuable vinculado al diálogo
	public int ID; //ID del Dialogo

	/*
	 * NOTA: Los intros y mensajes por defecto de los npcs no pueden tener IDGrupo, ya que los
	 * elementos con grupo son añadidos tras modificar un diálogo. Un dialogo con grupo no
	 * sería detectado al borrar elementos de grupo ya que el dialogo no estaría en la ruta de dialogos guardados
     */

	public List<Intro> intros; //Los dialogos que aparecen al principio de la conversación, ordenados por prioridad
	public List<TemaMensaje> temaMensajes; //Agrupan mensajes con los mismos temas
	public List<Mensaje> mensajes; //Diálogos que se muestran como opciones al finalizar las intros

	public Dialogo()
	{
		intros = new List<Intro>();
		mensajes = new List<Mensaje>();
	}

	public int DevuelveNumeroIntros()
	{
		return intros.Count;
	}

	//Devuelve el número de intros que están activos, es decir, que se pueden mostrar actualmente
	//(Ya sea porque su evento está activo, las variables del evento permiten mostrarla o no tienen un evento vinculado)
	//Además, devuelve la posición de la primera intro activa en el dialogo (0 si no hay ninguna)
	public int DevuelveNumeroIntrosActivas(ref int primerIntroActiva)
	{
		int count = 0;
		primerIntroActiva = 0;
		bool primera = false;

		for(int i = 0; i < DevuelveNumeroIntros(); i++)
		{
			if(intros[i].SeMuestra())
			{
				count++;
				if(!primera)
				{
					primerIntroActiva = i;
					primera = true;
				}
			}
		}

		return count;
	}

	public int DevuelveNumeroTemaMensajes()
	{
		return temaMensajes.Count;
	}

	public int DevuelveNumeroTemaMensajesActivos()
	{
		int count = 0;

		for(int i = 0; i < DevuelveNumeroTemaMensajes(); i++)
		{
			if(temaMensajes[i].DevuelveNumeroMensajesActivos() != 0)
			{
				count++;
			}
		}

		return count;
	}

	public bool TemaMensajeEsVisible(int pos)
	{
		return temaMensajes[pos].EstadoVisible();
	}

	public int DevuelveNumeroMensajes()
	{
		return mensajes.Count;
	}

	//Devuelve el número de mensajes que están activos, es decir, que se pueden mostrar actualmente
	//(Ya sea porque su evento está activo, las variables del evento permiten mostrarla o no tienen un evento vinculado)
	public int DevuelveNumeroMensajesActivos()
	{
		int count = 0;

		for(int i = 0; i < DevuelveNumeroMensajes(); i++)
		{
			if(mensajes[i].SeMuestra())
			{
				count++;
			}
		}

		return count;
	}

	public bool MensajeEsVisible(int pos)
	{
		return mensajes[pos].EstadoVisible();
	}

	//Devuelve el temamensaje de la lista con la posición indicada
	public TemaMensaje DevuelveTemaMensaje(int pos)
	{
		return temaMensajes[pos];
	}

	public Mensaje DevuelveMensaje(int posTema, int posMensaje)
	{
		Mensaje mensaje;

		if(posTema == -1)
		{
			mensaje = mensajes[posMensaje];
		}
		else
		{
			mensaje = temaMensajes[posTema].mensajes[posMensaje];
		}

		return mensaje;
	}

	//Devuelve el dialogo de la intro de la lista con la posición indicada
	public Dialogue DevuelveDialogoIntro(int pos)
	{
		return intros[pos].DevuelveDialogo();
	}

	//Devuelve el texto del temamensaje de la lista con la posición indicada
	public string DevuelveTextoTemaMensaje(int pos)
	{
		return temaMensajes[pos].DevuelveTexto();
	}

	//Devuelve el texto del mensaje de la lista con la posición indicada
	public string DevuelveTextoMensaje(int pos)
	{
		return mensajes[pos].DevuelveTexto();
	}

	//Comprueba si hay más intros activas delante de la posición indicada
	//Si devuelve true, es que hay más intros
	//Además guarda en la variable pos pasada por referencia la siguiente intro activa
	public bool AvanzaIntro(ref int pos)
	{
		bool avanza = false;
		bool primera = false;

		var posActual = pos;

		for(int i = posActual + 1; i < this.DevuelveNumeroIntros(); i++)
		{
			if(intros[i].SeMuestra())
			{
				avanza = true;
				if(!primera)
				{
					pos = i;
					primera = true;
				}
			}
		}

		return avanza;
	}

	//Añade la intro al dialogo
	public void AnyadirIntro(Intro intro)
	{
		//Si la intro con el ID especificado no existe en el dialogo, la añadimos
		if(!IntroExiste (intro.ID))
			intros.Add (intro);

		//Ordena las intros por prioridad de mayor a menor, manteniendo el orden de los elementos con la misma prioridad
		//Una intro añadida con la misma prioridad será colocada abajo de los iguales, es decir, como si fuera menor.
		intros = intros.OrderByDescending(x => x.prioridad).ToList();
	}

	//Comprueba si la intro con el id especificado existe en el diálogo
	//Devuelve true si existe
	private bool IntroExiste(int IDIntro)
	{
		return intros.Any(x => x.ID == IDIntro);
	}

	//Añade el mensaje indicado al diálogo con el idTema especificado
	//id_tema = -1: el mensaje se añade a la lista de mensajes sin tema
	//id_tema =  x: se añade al tema x
	public void AnyadirMensaje(int IDTema, Mensaje mensaje)
	{
		//Si el idtema es -1, el mensaje no pertence a ningún temamensaje, intentamos añadir el mensaje a la lista de mensajes
		if(IDTema == -1)
		{
			//Si el mensaje con el ID especificado no existe, lo añadimos
			if(!MensajeExiste (IDTema, mensaje.ID))
				mensajes.Add(mensaje);
		}
		//El mensaje tiene idtema perteneciente al temamensaje
		else
		{
			//Buscamos el indice del temamensaje al cual pertence el mensaje que queremos añadir en la lista de temamensajes del dialogo
			//Devuelve -1 si el temamensaje no existe en este diálogo
			int indice = temaMensajes.FindIndex(x => x.ID == IDTema);

			//El tema mensaje no existe, lo creamos, añadimos el temamensaje con el mensaje al diálogo
			if(indice == -1)
			{
				TemaMensaje temaMensaje = TemaMensaje.LoadTemaMensaje(Manager.rutaTemaMensajes + IDTema.ToString() + ".xml");
				temaMensaje.AddMensaje(mensaje);
				this.AnyadirTemaMensaje(temaMensaje);
			}
			//El tema mensaje existe, comprobamos si el mensaje ya existía anteriormente
			else
			{
				//Si el mensaje no existe en el temamensaje especificado, lo añadimos a este
				if(!MensajeExiste(indice, mensaje.ID))
				{
					temaMensajes[indice].mensajes.Add(mensaje);
				}
			}
		}
	}

	/*
	 * ATENCIÓN: ANTES DE USAR ESTA FUNCIÓN COMPROBAR ANTES QUE EL TEMA EXISTE
 	 */

	//Comprueba si el mensaje con el id especificado existe en la clase
	//num_tema = posición del tema en la lista de temamensajes. Si es -1, el mensaje no tiene tema
	private bool MensajeExiste(int posTema, int IDMensaje)
	{
		bool existe = false;

		if(posTema == -1)
			existe = mensajes.Any(x => x.ID == IDMensaje);
		else
			existe = temaMensajes[posTema].mensajes.Any(x => x.ID == IDMensaje);

		return existe;
	}

	//Añade el temamensaje especificado a la lista de temamensajes del diálogo
	private void AnyadirTemaMensaje(TemaMensaje temaMensaje)
	{
		temaMensajes.Add(temaMensaje);
	}

	//Comprueba si la intro/mensaje tiene la opción de destrucción activada y lo elimina de su respectiva lista si la tiene
	//tipo = 0: El dialogo actual está situado en una intro
	//		 1: El dialogo actual está situado en un mensaje que no forma parte de ningún temamensaje
	//		 2: El dialogo actual está situado en un mensaje que forma parte de un temamensaje
	public void MirarSiDialogoSeAutodestruye(int tipo, int posTema, ref int posDialogo)
	{
		//Dependiendo del tipo, destruimos un elemento de una lista u otra
		switch(tipo)
		{
		case 0:
			//Si la propiedad autodestruye está activada, eliminamos la intro de la lista
			if(intros[posDialogo].DevuelveAutodestruye())
			{
				intros.RemoveAt(posDialogo);
				posDialogo--;
			}
			break;
		case 1:
			//Si la propiedad autodestruye está activada, eliminamos el mensaje de la lista
			if(mensajes[posDialogo].DevuelveAutodestruye())
			{
				mensajes.RemoveAt(posDialogo);
			}
			break;
		case 2:
			//Si la propiedad autodestruye está activada, eliminamos el mensajetema de la lista
			if(temaMensajes[posTema].mensajes[posDialogo].DevuelveAutodestruye())
			{
				temaMensajes[posTema].mensajes.RemoveAt(posDialogo);

				//Si el temaMensajes se ha quedado vacío, lo destruimos
				if(temaMensajes[posTema].mensajes.Count == 0)
				{
					temaMensajes.RemoveAt(posTema);
				}
			}
			break;
		}
	}

	//Marca el nodo como leído, y comprueba las propiedades del nodo actual
	//tipo = 0: El dialogo actual está situado en una intro
	//		 1: El dialogo actual está situado en un mensaje que no forma parte de ningún temamensaje
	//		 2: El dialogo actual está situado en un mensaje que forma parte de un temamensaje
	public void MarcaDialogueNodeComoLeido(int tipo, int posTema, ref int posDialogo, DialogueNode node)
	{

		//Si el nodo no ha sido recorrido nunca, ejecutamos las funciones
		if(node.DevuelveRecorrido() != true)
		{
			//Dependiendo del tipo, comprobamos una lista u otra
			switch(tipo)
			{
			case 0:
				//Si está marcado que el dialogo se destruye, activamos la autodestrucción de este
				if(node.destruido)
					intros [posDialogo].ActivarAutodestruye();
				break;
			case 1:
				//Si está marcado que el dialogo se destruye, activamos la autodestrucción de este
				if(node.destruido)
					mensajes [posDialogo].ActivarAutodestruye();
				break;
			case 2:
				//Si está marcado que el dialogo se destruye, activamos la autodestrucción de este
				if(node.destruido)
					temaMensajes[posTema].mensajes[posDialogo].ActivarAutodestruye();
				break;
			}
				
			//Marcamos el nodo como recorrido y comprobamos las funciones del nodo actual
			node.MarcarRecorrido();
			ComprobarPropiedadesNodo(tipo, posTema, ref posDialogo, node);
		}


	}

	//Comprobamos las propiedades del nodo recorriendo sus listas
	//tipo_dialogo = 0: El dialogo actual está situado en una intro
	//		 		 1: El dialogo actual está situado en un mensaje que no forma parte de ningún temamensaje
	//		 		 2: El dialogo actual está situado en un mensaje que forma parte de un temamensaje
	private void ComprobarPropiedadesNodo(int tipoDialogo, int posTema, ref int posDialogo, DialogueNode node)
	{
		//Comprueba los grupos del nodo
		//Se añaden/eliminan los grupos indicados
		for(int i = 0; i < node.DevuelveNumeroGrupos(); i++)
		{
			int IDGrupo = node.grupos[i].DevuelveID();
			bool tipo = node.grupos[i].DevuelveTipo();

			//Si el tipo es verdadero, cargamos el grupo
			if(tipo)
			{
				CrearGrupo(IDGrupo, tipoDialogo, ref posDialogo);
			}
			//Si es falso, destruimos el grupo indicado y las intros/mensajes asignados a él
			else
			{
				EliminarGrupo(IDGrupo, tipoDialogo, posTema, ref posDialogo);
			}
		}

		//Comprueba la lista de intros a añadir
		for(int i = 0; i < node.DevuelveNumeroIntros(); i++)
		{
			int prioridad = node.intros[i].DevuelvePrioridad();
			int ID = node.intros[i].DevuelveIDIntro();
			int IDInteractuable = node.intros[i].devuelveIDInteractuable();
			int IDDialogo = node.intros[i].DevuelveIDDialogo();

			Intro intro = Intro.LoadIntro(Manager.rutaIntros + ID.ToString() + ".xml", prioridad);

			//Si la intro forma parte de un grupo y ese grupo ya ha acabado, no es añadida
			if(!Manager.instance.GrupoAcabadoExiste(intro.DevuelveIDGrupo()))
			{
				//Si nos encontramos en el diálogo el cual queremos añadir una intro
				if(IDInteractuable == this.IDInteractuable && IDDialogo == this.ID)
				{
					//Si estamos en una intro y la prioridad es mayor que la actual, cambiamos el indice de dialogo
					if(tipoDialogo == 0 && prioridad > intros[posDialogo].DevuelvePrioridad())
					{
						posDialogo++;
					}

					AnyadirIntro(intro);
				}
				else
				{
					Dialogo dialogo = BuscarDialogo(IDInteractuable, IDDialogo);

					dialogo.AnyadirIntro(intro);
					dialogo.AddToColaObjetos();
				}
			}
		}

		//Comprueba los mensajes a añadir
		for(int i = 0; i < node.DevuelveNumeroMensajes(); i++)
		{
			int ID = node.mensajes[i].DevuelveID();
			int IDTema = node.mensajes[i].DevuelveIDTema();
			int IDInteractuable = node.mensajes[i].DevuelveIDInteractuable();
			int IDDialogo = node.mensajes[i].DevuelveIDDialogo();

			Mensaje mensaje =  Mensaje.LoadMensaje(Manager.rutaMensajes + ID.ToString() + ".xml");

			//Si el mensaje forma parte de un grupo y ese grupo ya ha acabado, no es añadido
			if(!Manager.instance.GrupoAcabadoExiste(mensaje.DevuelveIDGrupo()))
			{
				//Si nos encontramos en el diálogo el cual queremos añadir un mensaje
				if(IDInteractuable == this.IDInteractuable && IDDialogo == this.ID)
				{
					AnyadirMensaje(IDTema, mensaje);
				}
				else
				{
					Dialogo dialogo = BuscarDialogo(IDInteractuable, IDDialogo);

					dialogo.AnyadirMensaje(IDTema, mensaje);
					dialogo.AddToColaObjetos();
				}
			}
		}

		//Comprobamos los objetos que cambian variables de grupos
		for(int i = 0; i < node.DevuelveNumeroGruposVariables(); i++)
		{
			int IDGrupo = node.gruposVariables[i].DevuelveIDGrupo();
			int tipo = node.gruposVariables[i].DevuelveTipo();
			int num = node.gruposVariables[i].DevuelveNumero();
			int valor = node.gruposVariables[i].DevuelveValor();

			//Si el grupo existe, cambiamos las variables
			if(Manager.instance.GrupoActivoExiste(IDGrupo))
			{
				switch(tipo)
				{
				case 0: //suma la cantidad
					Manager.instance.AddVariablesGrupo(IDGrupo, num, valor);
					break;
				case 1: //establece la cantidad
					Manager.instance.SetVariablesGrupo(IDGrupo, num, valor);
					break;
				}
			}
			//Sino existe, comprobamos que no ha sido eliminado
			else
			{
				//Tras comprobar que no ha sido eliminado, lo añadimos a lista de grupos modificados
				if (!Manager.instance.GrupoAcabadoExiste(IDGrupo))
				{
					Grupo grupo;

					//Comprobamos si el grupo modificado existe en la colaObjetos del Manager
					//Buscamos en la cola de objetos
					ColaObjeto colaObjeto = Manager.instance.GetColaObjetos(Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml");

					//Se ha encontrado en la cola de objetos
					if(colaObjeto != null)
					{
						ObjetoSerializable objetoSerializable = colaObjeto.GetObjeto();
						grupo = objetoSerializable as Grupo;
					}
					//No se ha encontrado en la cola de objetos
					else
					{
						//Comprobamos si está en la lista de grupos modificados
						if (System.IO.File.Exists (Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml"))
						{
							grupo = Grupo.CreateGrupo (Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml");
						}
						//Si no está ahí, miramos en el directorio predeterminado
						else
						{
							grupo = Grupo.CreateGrupo (Manager.rutaGrupos + IDGrupo.ToString () + ".xml");
						}
					}
						
					switch (tipo)
					{
					case 0: //suma la cantidad
						grupo.variables [num] += valor;
						break;
					case 1: //establece la cantidad
						grupo.variables [num] = valor;
						break;
					}

					//Guardamos el grupo en la ruta de grupos modificados
					grupo.AddToColaObjetos();
				}
			}
		}

		//Creamos un objeto inventario
		Inventario inventario;

		//Si se van a dar objetos, cargamos el inventario
		if(node.DevuelveNumeroObjetos() > 0)
		{
			//Buscamos el inventario en la colaobjetos
			ColaObjeto inventarioCola = Manager.instance.GetColaObjetos(Manager.rutaInventario + "Inventario.xml");

			//Se ha encontrado en la cola de objetos
			if(inventarioCola != null)
			{
				ObjetoSerializable objs = inventarioCola.GetObjeto();
				inventario = objs as Inventario;
			}
			//No se ha encontrado en la cola de objetos
			else
			{
				//Cargamos el inventario si existe, sino lo creamos
				if(System.IO.File.Exists(Manager.rutaInventario + "Inventario.xml"))
				{
					inventario = Inventario.LoadInventario(Manager.rutaInventario + "Inventario.xml");
				}
				else
				{
					inventario = new Inventario();
				}
			}

			//Comprobamos los objetos del nodo a añadir
			for(int i = 0; i < node.DevuelveNumeroObjetos(); i++)
			{
				int IDObjeto = node.objetos[i].DevuelveIDObjeto();
				int cantidad = node.objetos[i].DevuelveCantidad();

				//Añadimos el objeto
				inventario.AddObjeto(IDObjeto, cantidad);
			}

			inventario.AddToColaObjetos();
		}

		//Comprobamos que nonmbres de los interactuablesvamos a cambiar
		for(int i = 0; i < node.DevuelveNumeroNombres(); i++)
		{
			int IDInteractuable = node.nombres[i].DevuelveIDInteractuable();
			int indice = node.nombres[i].DevuelveIndiceNombre();

			GameObject interactuableGO;

			//El nombre a cambiar es del interactuable del cual forma parte el dialogo
			if(IDInteractuable == -1)
			{
				interactuableGO = Manager.instance.GetInteractuable(this.IDInteractuable);
			}
			//El nombre a cambiar es de cualquier otro
			else
			{
				interactuableGO = Manager.instance.GetInteractuable(IDInteractuable);
			}

			//El interactuable se ha encontrado en la escena
			if(interactuableGO != null)
			{
				Interactuable interactuable = interactuableGO.GetComponent<Interactuable>() as Interactuable;

				//Si el objeto es un NPC, le cambiamos el nombre
				if(interactuable.GetType() == typeof(InteractuableNPC))
				{
					InteractuableNPC interactuableNPC = interactuable as InteractuableNPC;

					int indiceActual = interactuableNPC.DevuelveIndiceNombre();

					if (indiceActual < indice)
					{
						interactuableNPC.SetIndiceNombre(indice);

						interactuableNPC.AddDatosToColaObjetos();

						//Actualizamos el interactuable para que muestre el nombre modificado
						interactuableNPC.SetNombre(interactuableNPC.DevuelveNombreActual());
					}
				}
			}
			//El interactuable no se ha encontrado en la escena, lo buscamos en la cola de objetos o en directorios
			else
			{
				InterDatos interDatos;

				ColaObjeto colaObjeto = Manager.instance.GetColaObjetos(Manager.rutaInterDatosGuardados + IDInteractuable.ToString()  + ".xml");

				if(colaObjeto != null)
				{
					ObjetoSerializable objetoSerializable = colaObjeto.GetObjeto();
					interDatos = objetoSerializable as InterDatos;
				}
				else
				{
					//Si existe un fichero guardado, cargamos ese fichero, sino cargamos el fichero por defecto
					if (System.IO.File.Exists(Manager.rutaInterDatosGuardados + IDInteractuable.ToString()  + ".xml"))
					{
						interDatos = InterDatos.LoadInterDatos(Manager.rutaInterDatosGuardados + IDInteractuable.ToString()  + ".xml");
					}
					else
					{
						interDatos = InterDatos.LoadInterDatos(Manager.rutaInterDatos + IDInteractuable.ToString()  + ".xml");
					}
				}

				//Si el objeto es un NPC, le cambiamos el nombre
				if(interDatos.GetType() == typeof(NPCDatos))
				{
					NPCDatos npcDatos = interDatos as NPCDatos;
					int indiceActual = npcDatos.DevuelveIndiceNombre();

					if (indiceActual < indice)
					{
						npcDatos.SetIndiceNombre(indice);
						npcDatos.AddToColaObjetos();
					}
				}
			}
		}

		//Comprobamos si hay alguna rutina que cambiar
		for(int i = 0; i < node.DevuelveNumeroRutinas(); i++)
		{
			int IDRutina = node.rutinas[i].DevuelveIDRutina();

			ManagerRutina.instance.CargarRutina(IDRutina, false, false);
		}
	}

	//Busca el diálogo en la escena y si no lo encuentra lo busca en los ficheros
	//Devuelve el diálogo con los parámetros indicados
	public static Dialogo BuscarDialogo(int IDInteractuable, int IDDialogo)
	{
		Dialogo dialogo = null;

		//Miramos si el interactuable en la escena
		GameObject interactuableGO = Manager.instance.GetInteractuable(IDInteractuable);

		//Si el interactuable se ha encontrado en la escena
		if(interactuableGO != null)
		{
			Interactuable interactuable = interactuableGO.GetComponent<Interactuable>() as Interactuable;
			dialogo = interactuable.DevolverDialogo(IDDialogo);
		}

		//Si el dialogo buscado no se ha encontrado en el interactuable de la escena, buscamos en otros sitios
		if(dialogo == null)
		{
			//Buscamos en la cola de objetos
			ColaObjeto colaObjeto = Manager.instance.GetColaObjetos(Manager.rutaInterDialogosGuardados + IDInteractuable.ToString() + "-" + IDDialogo.ToString() + ".xml");

			//Se ha encontrado en la cola de objetos
			if(colaObjeto != null)
			{
				ObjetoSerializable objetoSerializable = colaObjeto.GetObjeto();
				dialogo = objetoSerializable as Dialogo;
			}
			//No se ha encontrado en la cola de objetos, buscamos en los directorios
			else
			{
				//Cargamos el dialogo
				//Si existe un fichero guardado, cargamos ese fichero, sino cargamos el fichero por defecto
				if (System.IO.File.Exists(Manager.rutaInterDialogosGuardados + IDInteractuable.ToString()  + "-" + IDDialogo.ToString() + ".xml"))
				{
					dialogo = Dialogo.LoadDialogo(Manager.rutaInterDialogosGuardados + IDInteractuable.ToString()  + "-" + IDDialogo.ToString() + ".xml");
				}
				else
				{
					dialogo = Dialogo.LoadDialogo(Manager.rutaInterDialogos + IDInteractuable.ToString()  + "-" + IDDialogo.ToString() + ".xml");
				}
			}
		}

		return dialogo;
	}

	private void CrearGrupo(int IDGrupo, int tipoDialogo, ref int posDialogo)
	{
		//Si el grupo no está activo y no está en la lista de grupos acabados, lo añadimos
		if (!Manager.instance.GrupoActivoExiste(IDGrupo) && !Manager.instance.GrupoAcabadoExiste(IDGrupo))
		{
			//Comprobamos si el grupo se encuentra como grupo modificado en la lista colaObjetos del Manager
			ColaObjeto colaObjeto = Manager.instance.GetColaObjetos(Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml");

			//El grupo se ha encontrado en la cola de objetos
			if(colaObjeto != null)
			{
				ObjetoSerializable objetoSerializable = colaObjeto.GetObjeto();
				Grupo grupo = objetoSerializable as Grupo;
				Grupo.LoadGrupo(grupo, IDInteractuable, ID, tipoDialogo, ref posDialogo);

				//Borramos el grupo modificado de la cola ahora que ya ha sido añadido
				Manager.instance.RemoveFromColaObjetos(Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml");
			}
			//El grupo no se ha encontrado en la cola de objetos, miramos en los directorios
			else
			{
				//Miramos primero en la lista de grupos modificados
				if (System.IO.File.Exists (Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml"))
				{
					Grupo.LoadGrupo (Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml", IDInteractuable,ID, tipoDialogo, ref posDialogo);
				}
				//Si no está ahí, miramos en el directorio predeterminado
				else
				{
					Grupo.LoadGrupo (Manager.rutaGrupos + IDGrupo.ToString () + ".xml", IDInteractuable,ID, tipoDialogo, ref posDialogo);	
				}
			}
		}
	}

	private void EliminarGrupo(int IDGrupo, int tipoDialogo, int posTema, ref int posDialogo)
	{
		//Solo se pueden eliminar grupos activos actualmente
		if(Manager.instance.GrupoActivoExiste(IDGrupo))
		{
			//Empezamos destruyendo los intros/mensajes del dialogo actual
			switch(tipoDialogo)
			{
			//Si estamos en una intro, comprobamos que posicionamos correctamente el indice en las intros
			case 0:
				//Comprueba el número de intros del nodo
				for(int j = this.DevuelveNumeroIntros() - 1; j >= 0; j--)
				{
					//Si la intro forma parte del grupo que hemos eliminado, eliminamos la intro
					if(this.intros[j].IDGrupo == IDGrupo)
					{
						//Mantenemos el indice en una posicion correcta
						if (j < posDialogo)
						{
							this.intros.RemoveAt(j);
							posDialogo--;
						}
						//Si la intro a destruir es el actual, lo destruimos al acabar el diálogo de la intro (activando la autodestruccion)
						else if(j == posDialogo)
						{
							intros [posDialogo].ActivarAutodestruye();
						}
						else if(j > posDialogo)
						{
							this.intros.RemoveAt(j);
						}
					}
				}

				//Eliminamos aquellos mensajes dentro de los temamensajes cuyo IDGrupo coincide con el que hemos eliminado
				//o aquellos temamensajes con idgrupo
				for(int j = this.DevuelveNumeroTemaMensajes() - 1; j >= 0; j--)
				{
					//si el tema no tiene idgrupo (el valor de IDGrupo es -1), comprobamos los idgrupos de los mensajes de su interior
					if(this.temaMensajes[j].IDGrupo == -1)
					{
						for(int k = this.temaMensajes[j].mensajes.Count - 1; k >= 0; k--)
						{
							if(this.temaMensajes[j].mensajes[k].IDGrupo == IDGrupo)
							{
								this.temaMensajes[j].mensajes.RemoveAt(k);

								//Si el temaMensajes se ha quedado vacío, lo destruimos
								if(temaMensajes[j].mensajes.Count == 0)
								{
									this.temaMensajes.RemoveAt(j);
								}
							}
						}
					}
					//si el tema tiene un idgrupo que coincide con el grupo eliminado, eliminamos todo el temaMensaje
					else if(this.temaMensajes[j].IDGrupo == IDGrupo)
					{
						this.temaMensajes.RemoveAt(j);
					}
				}

				//comprobamos los mensajes sin tema
				//Eliminamos los mensajes que coincidan con el grupo eliminado
				for(int j = this.DevuelveNumeroMensajes() - 1; j >= 0; j--)
				{
					if(this.mensajes[j].IDGrupo == IDGrupo)
					{
						this.mensajes.RemoveAt(j);
					}
				}
				break;
				//Si estamos en un mensaje sin tema, comprobamos que posicionamos correctamente el indice en los mensajes
			case 1:
				//Eliminamos las intros cuyo IDGrupo coincide con el grupo eliminado
				for(int j = this.DevuelveNumeroIntros() - 1; j >= 0; j--)
				{
					if(this.intros[j].IDGrupo == IDGrupo)
					{
						this.intros.RemoveAt(j);
					}
				}

				//Eliminamos aquellos mensajes dentro de los temamensajes cuyo IDGrupo coincide con el que hemos eliminado
				for(int j = this.DevuelveNumeroTemaMensajes() - 1; j >= 0; j--)
				{
					//si el tema no tiene idgrupo (su valor es -1), comprobamos los idgrupos de los mensajes de su interior
					if(this.temaMensajes[j].IDGrupo == -1)
					{
						for(int k = this.temaMensajes[j].mensajes.Count - 1; k >= 0; k--)
						{
							if(this.temaMensajes[j].mensajes[k].IDGrupo == IDGrupo)
							{
								this.temaMensajes[j].mensajes.RemoveAt(k);

								//Si el temaMensajes se ha quedado vacío, lo destruimos
								if(temaMensajes[j].mensajes.Count == 0)
								{
									this.temaMensajes.RemoveAt(j);
								}
							}
						}
					}
					//si el tema tiene un idgrupo que coincide con el grupo eliminado, eliminamos todo el temaMensaje
					else if(this.temaMensajes[j].IDGrupo == IDGrupo)
					{
						this.temaMensajes.RemoveAt(j);
					}
				}

				//comprobamos los mensajes sin tema
				//Eliminamos los mensajes que coincidan con el grupo eliminado
				for(int j = this.DevuelveNumeroMensajes() - 1; j >= 0; j--)
				{
					if(this.mensajes[j].IDGrupo == IDGrupo)
					{
						//Mantenemos el indice en una posicion correcta
						if (j < posDialogo)
						{
							this.mensajes.RemoveAt(j);
							posDialogo--;
						}
						//Si el mensaje a destruir es el actual, lo destruimos al final (activando la autodestruccion)
						else if(j == posDialogo)
						{
							mensajes [posDialogo].ActivarAutodestruye();
						}
						else if(j > posDialogo)
						{
							this.mensajes.RemoveAt(j);
						}
					}
				}
				break;
				//si estamos en un mensaje con tema, comprobamos que posicionamos bien el indice de los mensajes
			case 2:
				//Eliminamos las intros cuyo IDGrupo coincide con el grupo eliminado
				for(int j = this.DevuelveNumeroIntros() -1; j >= 0; j--)
				{
					if(this.intros[j].IDGrupo == IDGrupo)
					{
						this.intros.RemoveAt(j);
					}
				}

				//Eliminamos aquellos mensajes dentro de los temamensajes cuyo IDGrupo coincide con el que hemos eliminado
				//o aquellos temamensajes con idgrupo
				for(int j = this.DevuelveNumeroTemaMensajes() -1; j >= 0; j--)
				{
					//si el tema no tiene idgrupo (su valor es -1), comprobamos los idgrupos de los mensajes de su interior
					if(this.temaMensajes[j].IDGrupo == -1)
					{
						for(int k = this.temaMensajes[j].mensajes.Count - 1; k >= 0; k--)
						{
							//Si el mensaje forma parte del idgrupo que queremos destruir, lo borramos
							if(this.temaMensajes[j].mensajes[k].IDGrupo == IDGrupo)
							{
								//Si nos encontramos en el mensaje que queremos destruir, activamos la autodestruccion
								if(j == posTema && k == posDialogo)
								{
									//Mantenemos el indice en una posicion correcta
									if (k < posDialogo)
									{
										this.temaMensajes[j].mensajes.RemoveAt(k);
										posDialogo--;

										//Si el temaMensajes se ha quedado vacío, lo destruimos
										if(temaMensajes[j].mensajes.Count == 0)
										{
											this.temaMensajes.RemoveAt(j);
										}
									}
									//Si el mensaje a destruir es el actual, lo destruimos al final (activando la autodestruccion)
									else if(k == posDialogo)
									{
										temaMensajes[j].mensajes [posDialogo].ActivarAutodestruye();
									}
									else if(k > posDialogo)
									{
										this.temaMensajes[j].mensajes.RemoveAt(k);

										//Si el temaMensajes se ha quedado vacío, lo destruimos
										if(temaMensajes[j].mensajes.Count == 0)
										{
											this.temaMensajes.RemoveAt(j);
										}
									}
								}
								else
								{
									this.temaMensajes[j].mensajes.RemoveAt(k);

									//Si el temaMensajes se ha quedado vacío, lo destruimos
									if(temaMensajes[j].mensajes.Count == 0)
									{
										this.temaMensajes.RemoveAt(j);
									}
								}
							}
						}
					}
					//Si el temamensaje tiene idgrupo y coincide con el que queremos destruir, comprobamos que no nos encontramos en algun mensaje que debemos destruir
					else if(this.temaMensajes[j].IDGrupo == IDGrupo)
					{
						for(int k = this.temaMensajes[j].mensajes.Count - 1; k >= 0; k--)
						{
							//Si nos encontramos en el mensaje que queremos destruir, activamos la autodestruccion
							if(j == posTema && k == posDialogo)
							{
								temaMensajes[j].mensajes [posDialogo].ActivarAutodestruye();
							}
							else
							{
								this.temaMensajes[j].mensajes.RemoveAt(k);

								//Si el mensajetema ya no tiene mensajes, lo destruimos
								if(temaMensajes[j].mensajes.Count == 0)
								{
									this.temaMensajes.RemoveAt(j);
								}
							}
						}
					}
				}

				//comprobamos los mensajes sin tema
				//Eliminamos los mensajes que coincidan con el grupo eliminado
				for(int j = this.DevuelveNumeroMensajes() - 1; j >= 0; j--)
				{
					if(this.mensajes[j].IDGrupo == IDGrupo)
					{
						this.mensajes.RemoveAt(j);
					}
				}
				break;
			}

			//Ahora comprobamos los dialogos de los interactuables de la cola de objetos del manager
			List<ObjetoSerializable> listaObjetosSerializables = new List<ObjetoSerializable>();
			listaObjetosSerializables = Manager.instance.GetColaObjetosTipo(typeof(Dialogo));

			for(var j = 0; j < listaObjetosSerializables.Count; j++)
			{
				//Indica si el dialogo se ha actualizado, eliminando algún elemento
				//Si lo ha hecho, se añade posteriormente a la cola de objetos para serializar
				bool actualizado = false;

				ObjetoSerializable objetoSerializable = listaObjetosSerializables[j];
				Dialogo dialogo = objetoSerializable as Dialogo;

				//Borramos las intros que forman parte del grupo eliminado
				for(int k = dialogo.DevuelveNumeroIntros() - 1; k >= 0; k--)
				{
					if(dialogo.intros[k].IDGrupo == IDGrupo)
					{
						dialogo.intros.RemoveAt(k);
						actualizado = true;
					}
				}

				//Borramos los mensajes con temamensajes con el idgrupo del grupo eliminado
				//o aquellos temamensajes con este grupo
				for(int k = dialogo.DevuelveNumeroTemaMensajes() - 1; k >= 0; k--)
				{
					//si el tema no tiene idgrupo (su valor es -1), comprobamos los idgrupos de los mensajes de su interior
					if(dialogo.temaMensajes[k].IDGrupo == -1)
					{
						for(int l = dialogo.temaMensajes[k].mensajes.Count - 1; l >= 0; l--)
						{
							if(dialogo.temaMensajes[k].mensajes[l].IDGrupo == IDGrupo)
							{
								dialogo.temaMensajes[k].mensajes.RemoveAt(l);

								//Si el temaMensajes se ha quedado vacío, lo destruimos
								if(dialogo.temaMensajes[k].mensajes.Count == 0)
								{
									dialogo.temaMensajes.RemoveAt(k);
								}

								actualizado = true;
							}
						}
					}
					else if(dialogo.temaMensajes[k].IDGrupo == IDGrupo)
					{
						dialogo.temaMensajes.RemoveAt(k);
						actualizado = true;
					}
				}

				//Borramos los mensajes con el idgrupo del grupo eliminado
				for(int k = dialogo.DevuelveNumeroMensajes() - 1; k >= 0; k--)
				{
					if(dialogo.mensajes[k].IDGrupo == IDGrupo)
					{
						dialogo.mensajes.RemoveAt(k);
						actualizado = true;
					}
				}

				//Si se ha actualizado el dialogo, lo añadimos en la cola de objetos para serializarlo
				if (actualizado) {
					dialogo.AddToColaObjetos ();
				}
			}

			//Ahora comprobamos a los npcs de la escena actual
			List<GameObject> listaInteractuablesGO = Manager.instance.GetAllInteractuables();

			//Lista de dialogos no actualizados, que será utilizada al comprobar los ficheros posteriormente
			List<Dialogo> dialogosNoActualizados = new List<Dialogo>();

			for(int j = 0; j < listaInteractuablesGO.Count; j++)
			{
				GameObject gameobject = listaInteractuablesGO[j];
				Interactuable interactuable = gameobject.GetComponent<Interactuable>() as Interactuable;
				List<Dialogo> dialogos = interactuable.DevolverDialogos();

				for(int z = 0; z < dialogos.Count; z ++)
				{
					Dialogo dialogo = dialogos[z];

					//Si el diálogo no está en la cola de objetos, miramos si hay que borrar algo
					//Y el dialogo no es el actual
					if(!Manager.instance.ColaObjetoExiste(Manager.rutaInterDialogosGuardados + interactuable.ID.ToString() + "-" + dialogo.ID.ToString() + ".xml") && !(IDInteractuable == interactuable.ID && ID == dialogo.ID))
					{
						//Indica si el dialogo se ha actualizado, eliminando algún elemento
						//Si lo ha hecho, se añade posteriormente a la cola de objetos para serializar
						bool actualizado = false;

						//Eliminamos las intros cuyo IDGrupo coincide con el grupo eliminado
						for(int k = dialogo.DevuelveNumeroIntros() - 1; k >= 0; k--)
						{
							if(dialogo.intros[k].IDGrupo == IDGrupo)
							{
								dialogo.intros.RemoveAt(k);
								actualizado = true;
							}
						}

						//Eliminamos aquellos mensajes dentro de los temamensajes cuyo IDGrupo coincide con el que hemos eliminado
						//o aquellos temamensajes con idgrupo
						for(int k = dialogo.DevuelveNumeroTemaMensajes() - 1; k >= 0; k--)
						{
							//si el tema no tiene idgrupo (su valor es -1), comprobamos los idgrupos de los mensajes de su interior
							if(dialogo.temaMensajes[k].IDGrupo == -1)
							{
								for(int l = dialogo.temaMensajes[k].mensajes.Count - 1; l >= 0; l--)
								{
									if(dialogo.temaMensajes[k].mensajes[l].IDGrupo == IDGrupo)
									{
										dialogo.temaMensajes[k].mensajes.RemoveAt(l);

										//Si el temaMensajes se ha quedado vacío, lo destruimos
										if(dialogo.temaMensajes[k].mensajes.Count == 0)
										{
											dialogo.temaMensajes.RemoveAt(k);
										}

										actualizado = true;
									}
								}
							}
							else if(dialogo.temaMensajes[k].IDGrupo == IDGrupo)
							{
								dialogo.temaMensajes.RemoveAt(k);
								actualizado = true;
							}
						}

						//Borramos los mensajes con el idgrupo del grupo eliminado
						for(int k = dialogo.DevuelveNumeroMensajes() - 1; k >= 0; k--)
						{
							if(dialogo.mensajes[k].IDGrupo == IDGrupo)
							{
								dialogo.mensajes.RemoveAt(k);
								actualizado = true;
							}
						}

						//Si se ha actualizado, se añade a la colaObjetos
						//sino, se añade a la lista de dialogos no actualizados
						if(actualizado)
							dialogo.AddToColaObjetos();
						else
							dialogosNoActualizados.Add(dialogo);
					}
				}
			}

			//Ahora recorremos los ficheros guardadados
			DirectoryInfo directoyInfo = new DirectoryInfo(Manager.rutaInterDialogosGuardados);
			FileInfo[] fileInfo = directoyInfo.GetFiles().OrderByDescending( f => f.CreationTime).ToArray(); //los nuevos empiezan al principio de la lista

			for(var j = 0; j < fileInfo.Length; j++)
			{
				//Contiene id_npc-id_diag
				string IDs = Path.GetFileNameWithoutExtension(fileInfo[j].Name);

				//Divide el string en
				//arr[0] = ID_NPC
				//arr[1] = ID_DIAG
				string[] IDsDosPartes = IDs.Split('-');

				//Comprobamos que no es el dialogo actual
				//O uno de los dialogos que hemos comprobado anteriormente de la escena
				if(!(IDInteractuable == Int32.Parse(IDsDosPartes[0]) && ID == Int32.Parse(IDsDosPartes[1])) && !(dialogosNoActualizados.Any(x => x.IDInteractuable == Int32.Parse(IDsDosPartes[0]) && x.ID == Int32.Parse(IDsDosPartes[1]))))
				{
					//Buscamos en la cola de objetos
					//Si existe, no hacemos nada
					//Si no existe, comprobamos el dialogo
					if(!Manager.instance.ColaObjetoExiste(Manager.rutaInterDialogosGuardados + IDs  + ".xml"))
					{
						Dialogo dialogo = Dialogo.LoadDialogo(Manager.rutaInterDialogosGuardados + IDs  + ".xml");

						//Indica si el dialogo se ha actualizado, eliminando algún elemento
						//Si lo ha hecho, se añade posteriormente a la cola de objetos para serializar
						bool actualizado = false;

						for(int k = dialogo.DevuelveNumeroIntros() - 1; k >= 0; k--)
						{
							if(dialogo.intros[k].IDGrupo == IDGrupo)
							{
								dialogo.intros.RemoveAt(k);
								actualizado = true;
							}
						}

						for(int k = dialogo.DevuelveNumeroTemaMensajes() - 1; k >= 0; k--)
						{
							//si el tema no tiene idgrupo, comprobamos los idgrupos de los mensajes de su interior
							//Si el temaMensaje es -1, no forma parte de ningún grupo
							if(dialogo.temaMensajes[k].IDGrupo == -1)
							{
								for(int l = dialogo.temaMensajes[k].mensajes.Count - 1; l >= 0; l--)
								{
									if(dialogo.temaMensajes[k].mensajes[l].IDGrupo == IDGrupo)
									{
										dialogo.temaMensajes[k].mensajes.RemoveAt(l);

										//Si el temaMensajes se ha quedado vacío, lo destruimos
										if(dialogo.temaMensajes[k].mensajes.Count == 0)
										{
											dialogo.temaMensajes.RemoveAt(k);
										}

										actualizado = true;
									}
								}
							}
							else if(dialogo.temaMensajes[k].IDGrupo == IDGrupo)
							{
								dialogo.temaMensajes.RemoveAt(k);
								actualizado = true;
							}
						}

						for(int k = dialogo.DevuelveNumeroMensajes() - 1; k >= 0; k--)
						{
							if(dialogo.mensajes[k].IDGrupo == IDGrupo)
							{
								dialogo.mensajes.RemoveAt(k);
								actualizado = true;
							}
						}

						if (actualizado) {
							dialogo.AddToColaObjetos ();
						}
					}
				}
			}
		}

		//Por último, eliminamos el grupo del Manager
		//Si el grupo no está en la lista de grupos activos, se añade a la lista de grupos acabados y no podrá ser añadido
		Manager.instance.RemoveFromGruposActivos(IDGrupo);

		//FALTA ENVIAR LOS MENSAJES/INTROS DE FINALIZACIÓN
		//ESTOS SOLO PODRÁN SER ENVIADOS SI EL GRUPO ESTABA EN GRUPOS ACTIVOS
	}

	//Devuelve el dialogo de un xml indicado en la ruta
	public static Dialogo LoadDialogo(string path)
	{
		Dialogo dialogo = Manager.instance.DeserializeData<Dialogo>(path);

		return dialogo;
	}

	//Añade el dialogo a la cola de objetos serializables
	public void AddToColaObjetos()
	{
		Manager.instance.AddToColaObjetos (Manager.rutaInterDialogosGuardados + IDInteractuable.ToString() + "-" + ID.ToString()  + ".xml", this);
	}
}