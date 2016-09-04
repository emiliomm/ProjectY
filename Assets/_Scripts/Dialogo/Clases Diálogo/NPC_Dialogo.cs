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
public class NPC_Dialogo : ObjetoSerializable
{
	public int ID; //ID del Dialogo
	public int ID_NPC; //ID del Interactuable vinculado al diálogo

	/*
	 * NOTA: Los intros y mensajes por defecto de los npcs no pueden tener IDGrupo, ya que los
	 * elementos con grupo son añadidos tras modificar un diálogo. Un dialogo con grupo no
	 * sería detectado al borrar elementos de grupo ya que el dialogo no estaría en la ruta de dialogos guardados
     */

	public List<Intro> intros; //Los dialogos que aparecen al principio de la conversación, ordenados por prioridad
	public List<TemaMensaje> temaMensajes; //Agrupan mensajes con los mismos temas
	public List<Mensaje> mensajes; //Diálogos que se muestran como opciones al finalizar las intros

	public NPC_Dialogo()
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
			if(intros[i].seMuestra())
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
			if(mensajes[i].seMuestra())
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

	public Mensaje DevuelveMensaje(int pos_tema, int pos_mensaje)
	{
		Mensaje men;

		if(pos_tema == -1)
		{
			men = mensajes[pos_mensaje];
		}
		else
		{
			men = temaMensajes[pos_tema].mensajes[pos_mensaje];
		}

		return men;
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
			if(intros[i].seMuestra())
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
	public void AnyadirIntro(Intro d)
	{
		//Si la intro con el ID especificado no existe en el dialogo, la añadimos
		if(!IntroExiste (d.ID))
			intros.Add (d);

		//Ordena las intros por prioridad de mayor a menor, manteniendo el orden de los elementos con la misma prioridad
		//Una intro añadida con la misma prioridad será colocada abajo de los iguales, es decir, como si fuera menor.
		intros = intros.OrderByDescending(x => x.prioridad).ToList();
	}

	//Comprueba si la intro con el id especificado existe en el diálogo
	//Devuelve true si existe
	private bool IntroExiste(int id)
	{
		return intros.Any(x => x.ID == id);
	}

	//Añade el mensaje indicado al diálogo con el idTema especificado
	//id_tema = -1: el mensaje se añade a la lista de mensajes sin tema
	//id_tema =  x: se añade al tema x
	public void AnyadirMensaje(int id_tema, Mensaje m)
	{
		//Si el idtema es -1, el mensaje no pertence a ningún temamensaje, intentamos añadir el mensaje a la lista de mensajes
		if(id_tema == -1)
		{
			//Si el mensaje con el ID especificado no existe, lo añadimos
			if(!MensajeExiste (id_tema, m.ID))
				mensajes.Add(m);
		}
		//El mensaje tiene idtema perteneciente al temamensaje
		else
		{
			//Buscamos el indice del temamensaje al cual pertence el mensaje que queremos añadir en la lista de temamensajes del dialogo
			//Devuelve -1 si el temamensaje no existe en este diálogo
			int indice = temaMensajes.FindIndex(x => x.ID == id_tema);

			//El tema mensaje no existe, lo creamos, añadimos el temamensaje con el mensaje al diálogo
			if(indice == -1)
			{
				TemaMensaje tm = TemaMensaje.LoadTemaMensaje(Manager.rutaTemaMensajes + id_tema.ToString() + ".xml");
				tm.AddMensaje(m);
				this.AnyadirTemaMensaje(tm);
			}
			//El tema mensaje existe, comprobamos si el mensaje ya existía anteriormente
			else
			{
				//Si el mensaje no existe en el temamensaje especificado, lo añadimos a este
				if(!MensajeExiste(indice, m.ID))
				{
					temaMensajes[indice].mensajes.Add(m);
				}
			}
		}
	}

	/*
	 * ATENCIÓN: ANTES DE USAR ESTA FUNCIÓN COMPROBAR ANTES QUE EL TEMA EXISTE
 	 */

	//Comprueba si el mensaje con el id especificado existe en la clase
	//num_tema = posición del tema en la lista de temamensajes. Si es -1, el mensaje no tiene tema
	private bool MensajeExiste(int pos_tema, int id)
	{
		bool existe = false;

		if(pos_tema == -1)
			existe = mensajes.Any(x => x.ID == id);
		else
			existe = temaMensajes[pos_tema].mensajes.Any(x => x.ID == id);

		return existe;
	}

	//Añade el temamensaje especificado a la lista de temamensajes del diálogo
	private void AnyadirTemaMensaje(TemaMensaje tm)
	{
		temaMensajes.Add(tm);
	}

	//Comprueba si la intro/mensaje tiene la opción de destrucción activada y lo elimina de su respectiva lista si la tiene
	//tipo = 0: El dialogo actual está situado en una intro
	//		 1: El dialogo actual está situado en un mensaje que no forma parte de ningún temamensaje
	//		 2: El dialogo actual está situado en un mensaje que forma parte de un temamensaje
	public void MirarSiDialogoSeAutodestruye(int tipo, int pos_tema, ref int pos_dialogo)
	{
		//Dependiendo del tipo, destruimos un elemento de una lista u otra
		switch(tipo)
		{
		case 0:
			//Si la propiedad autodestruye está activada, eliminamos la intro de la lista
			if(intros[pos_dialogo].DevuelveAutodestruye())
			{
				intros.RemoveAt(pos_dialogo);
				pos_dialogo--;
			}
			break;
		case 1:
			//Si la propiedad autodestruye está activada, eliminamos el mensaje de la lista
			if(mensajes[pos_dialogo].DevuelveAutodestruye())
			{
				mensajes.RemoveAt(pos_dialogo);
			}
			break;
		case 2:
			//Si la propiedad autodestruye está activada, eliminamos el mensajetema de la lista
			if(temaMensajes[pos_tema].mensajes[pos_dialogo].DevuelveAutodestruye())
			{
				temaMensajes[pos_tema].mensajes.RemoveAt(pos_dialogo);

				//Si el temaMensajes se ha quedado vacío, lo destruimos
				if(temaMensajes[pos_tema].mensajes.Count == 0)
				{
					temaMensajes.RemoveAt(pos_tema);
				}
			}
			break;
		}
	}

	//Marca el nodo como leído, y comprueba las propiedades del nodo actual
	//tipo = 0: El dialogo actual está situado en una intro
	//		 1: El dialogo actual está situado en un mensaje que no forma parte de ningún temamensaje
	//		 2: El dialogo actual está situado en un mensaje que forma parte de un temamensaje
	public void MarcaDialogueNodeComoLeido(int tipo, int pos_tema, ref int pos_dialogo, DialogueNode node)
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
					intros [pos_dialogo].ActivarAutodestruye();
				break;
			case 1:
				//Si está marcado que el dialogo se destruye, activamos la autodestrucción de este
				if(node.destruido)
					mensajes [pos_dialogo].ActivarAutodestruye();
				break;
			case 2:
				//Si está marcado que el dialogo se destruye, activamos la autodestrucción de este
				if(node.destruido)
					temaMensajes[pos_tema].mensajes[pos_dialogo].ActivarAutodestruye();
				break;
			}
				
			//Marcamos el nodo como recorrido y comprobamos las funciones del nodo actual
			node.MarcarRecorrido();
			ComprobarPropiedadesNodo(tipo, pos_tema, ref pos_dialogo, node);
		}


	}

	//Comprobamos las propiedades del nodo recorriendo sus listas
	//tipo_dialogo = 0: El dialogo actual está situado en una intro
	//		 		 1: El dialogo actual está situado en un mensaje que no forma parte de ningún temamensaje
	//		 		 2: El dialogo actual está situado en un mensaje que forma parte de un temamensaje
	private void ComprobarPropiedadesNodo(int tipo_dialogo, int pos_tema, ref int pos_dialogo, DialogueNode node)
	{
		//Comprueba los grupos del nodo
		//Se añaden/eliminan los grupos indicados
		for(int i = 0; i < node.DevuelveNumeroGrupos(); i++)
		{
			int IDGrupo = node.Grupos[i].DevuelveID();
			bool tipo = node.Grupos[i].DevuelveTipo();

			//Si el tipo es verdadero, cargamos el grupo
			if(tipo)
			{
				CrearGrupo(IDGrupo, tipo_dialogo, ref pos_dialogo);
			}
			//Si es falso, destruimos el grupo indicado y las intros/mensajes asignados a él
			else
			{
				EliminarGrupo(IDGrupo, tipo_dialogo, pos_tema, ref pos_dialogo);
			}
		}

		//Comprueba la lista de intros a añadir
		for(int i = 0; i < node.DevuelveNumeroIntros(); i++)
		{
			int prioridad = node.Intros[i].DevuelvePrioridad();
			int ID = node.Intros[i].DevuelveID();
			int IDNpc = node.Intros[i].DevuelveIDNpc();
			int IDDialogo = node.Intros[i].DevuelveIDDialogo();

			Intro intro = Intro.LoadIntro(Manager.rutaIntros + ID.ToString() + ".xml", prioridad);

			//Si la intro forma parte de un grupo y ese grupo ya ha acabado, no es añadida
			if(!Manager.Instance.GrupoAcabadoExiste(intro.DevuelveIDGrupo()))
			{
				//Si nos encontramos en el diálogo el cual queremos añadir una intro
				if(IDNpc == this.ID_NPC && IDDialogo == this.ID)
				{
					//Si estamos en una intro y la prioridad es mayor que la actual, cambiamos el indice de dialogo
					if(tipo_dialogo == 0 && prioridad > intros[pos_dialogo].DevuelvePrioridad())
					{
						pos_dialogo++;
					}

					AnyadirIntro(intro);
				}
				else
				{
					NPC_Dialogo dialog = BuscarDialogo(IDNpc, IDDialogo);

					dialog.AnyadirIntro(intro);
					dialog.AddToColaObjetos();
				}
			}
		}

		//Comprueba los mensajes a añadir
		for(int i = 0; i < node.DevuelveNumeroMensajes(); i++)
		{
			int ID = node.Mensajes[i].DevuelveID();
			int IDTema = node.Mensajes[i].DevuelveIDTema();
			int IDNpc = node.Mensajes[i].DevuelveIDNpc();
			int IDDialogo = node.Mensajes[i].DevuelveIDDialogo();

			Mensaje mensaje =  Mensaje.LoadMensaje(Manager.rutaMensajes + ID.ToString() + ".xml");

			//Si el mensaje forma parte de un grupo y ese grupo ya ha acabado, no es añadido
			if(!Manager.Instance.GrupoAcabadoExiste(mensaje.DevuelveIDGrupo()))
			{
				//Si nos encontramos en el diálogo el cual queremos añadir un mensaje
				if(IDNpc == this.ID_NPC && IDDialogo == this.ID)
				{
					AnyadirMensaje(IDTema, mensaje);
				}
				else
				{
					NPC_Dialogo dialog = BuscarDialogo(IDNpc, IDDialogo);

					dialog.AnyadirMensaje(IDTema, mensaje);
					dialog.AddToColaObjetos();
				}
			}
		}

		//Comprobamos los objetos que cambian variables de grupos
		for(int i = 0; i < node.DevuelveNumeroGruposVariables(); i++)
		{
			int IDGrupo = node.GruposVariables[i].DevuelveIDGrupo();
			int tipo = node.GruposVariables[i].DevuelveTipo();
			int num = node.GruposVariables[i].DevuelveNumero();
			int valor = node.GruposVariables[i].DevuelveValor();

			//Si el grupo existe, cambiamos las variables
			if(Manager.Instance.GrupoActivoExiste(IDGrupo))
			{
				switch(tipo)
				{
				case 0: //suma la cantidad
					Manager.Instance.AddVariablesGrupo(IDGrupo, num, valor);
					break;
				case 1: //establece la cantidad
					Manager.Instance.SetVariablesGrupo(IDGrupo, num, valor);
					break;
				}
			}
			//Sino existe, comprobamos que no ha sido eliminado
			else
			{
				//Tras comprobar que no ha sido eliminado, lo añadimos a lista de grupos modificados
				if (!Manager.Instance.GrupoAcabadoExiste(IDGrupo))
				{
					Grupo g;

					//Comprobamos si el grupo modificado existe en la colaObjetos del Manager
					//Buscamos en la cola de objetos
					ColaObjeto cobj = Manager.Instance.GetColaObjetos(Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml");

					//Se ha encontrado en la cola de objetos
					if(cobj != null)
					{
						ObjetoSerializable objs = cobj.GetObjeto();
						g = objs as Grupo;
					}
					//No se ha encontrado en la cola de objetos
					else
					{
						//Comprobamos si está en la lista de grupos modificados
						if (System.IO.File.Exists (Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml"))
						{
							g = Grupo.CreateGrupo (Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml");
						}
						//Si no está ahí, miramos en el directorio predeterminado
						else
						{
							g = Grupo.CreateGrupo (Manager.rutaGrupos + IDGrupo.ToString () + ".xml");
						}
					}
						
					switch (tipo)
					{
					case 0: //suma la cantidad
						g.variables [num] += valor;
						break;
					case 1: //establece la cantidad
						g.variables [num] = valor;
						break;
					}

					//Guardamos el grupo en la ruta de grupos modificados
					g.AddToColaObjetos();
				}
			}
		}

		//Creamos un objeto inventario
		Inventario inventario;

		//Si se van a dar objetos, cargamos el inventario
		if(node.DevuelveNumeroObjetos() > 0)
		{
			//Buscamos el inventario en la colaobjetos
			ColaObjeto inventarioCola = Manager.Instance.GetColaObjetos(Manager.rutaInventario + "Inventario.xml");

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
				int IDObjeto = node.Objetos[i].DevuelveIDObjeto();
				int cantidad = node.Objetos[i].devuelveCantidad();

				//Añadimos el objeto
				inventario.addObjeto(IDObjeto, cantidad);
			}

			inventario.AddToColaObjetos();
		}

		//Comprobamos que nonmbres de los interactuablesvamos a cambiar
		for(int i = 0; i < node.DevuelveNumeroNombres(); i++)
		{
			int IDNpc = node.Nombres[i].DevuelveIDNpc();
			int Indice = node.Nombres[i].DevuelveIndiceNombre();

			GameObject gobj;

			//El nombre a cambiar es del interactuable del cual forma parte el dialogo
			if(IDNpc == -1)
			{
				gobj = Manager.Instance.GetInteractuable(ID_NPC);
			}
			//El nombre a cambiar es de cualquier otro
			else
			{
				gobj = Manager.Instance.GetInteractuable(IDNpc);
			}

			//El interactuable se ha encontrado en la escena
			if(gobj != null)
			{
				Interactuable inter = gobj.GetComponent<Interactuable>() as Interactuable;

				//Si el objeto es un NPC, le cambiamos el nombre
				if(inter.GetType() == typeof(InteractuableNPC))
				{
					InteractuableNPC intern = inter as InteractuableNPC;

					int indiceActual = intern.DevuelveIndiceNombre();

					if (indiceActual < Indice)
					{
						intern.SetIndiceNombre(Indice);
					}

					intern.AddDatosToColaObjetos();

					//Actualizamos el interactuable para que muestre el nombre modificado
					intern.SetNombre(intern.DevuelveNombreActual());
				}
			}
			//El interactuable no se ha encontrado en la escena, lo buscamos en la cola de objetos o en directorios
			else
			{
				InterDatos d;

				ColaObjeto cobj = Manager.Instance.GetColaObjetos(Manager.rutaNPCDatosGuardados + IDNpc.ToString()  + ".xml");

				if(cobj != null)
				{
					ObjetoSerializable objs = cobj.GetObjeto();
					d = objs as InterDatos;
				}
				else
				{
					//Si existe un fichero guardado, cargamos ese fichero, sino cargamos el fichero por defecto
					if (System.IO.File.Exists(Manager.rutaNPCDatosGuardados + IDNpc.ToString()  + ".xml"))
					{
						d = InterDatos.LoadInterDatos(Manager.rutaNPCDatosGuardados + IDNpc.ToString()  + ".xml");
					}
					else
					{
						d = InterDatos.LoadInterDatos(Manager.rutaNPCDatos + IDNpc.ToString()  + ".xml");
					}
				}

				//Si el objeto es un NPC, le cambiamos el nombre
				if(d.GetType() == typeof(NPCDatos))
				{
					NPCDatos dnpc = d as NPCDatos;
					int indiceActual = dnpc.DevuelveIndiceNombre();

					if (indiceActual < Indice)
					{
						dnpc.SetIndiceNombre(Indice);
					}

					dnpc.AddToColaObjetos();
				}
			}
		}

		//Comprobamos si hay alguna rutina que cambiar
		for(int i = 0; i < node.DevuelveNumeroRutinas(); i++)
		{
			int IDRutina = node.Rutinas[i].devuelveIDRutina();

			Manager.Instance.cambiarRutina(IDRutina);
		}
	}

	//Busca el diálogo en la escena y si no lo encuentra lo busca en los ficheros
	//Devuelve el diálogo con los parámetros indicados
	public static NPC_Dialogo BuscarDialogo(int IDInter, int IDDialogo)
	{
		NPC_Dialogo dialog = null;

		//Miramos si el interactuable en la escena
		GameObject gobj = Manager.Instance.GetInteractuable(IDInter);

		//Si el interactuable se ha encontrado en la escena
		if(gobj != null)
		{
			Interactuable inter = gobj.GetComponent<Interactuable>() as Interactuable;
			dialog = inter.DevolverDialogo(IDDialogo);
		}

		//Si el dialogo buscado no se ha encontrado en el interactuable de la escena, buscamos en otros sitios
		if(dialog == null)
		{
			//Buscamos en la cola de objetos
			ColaObjeto cobj = Manager.Instance.GetColaObjetos(Manager.rutaNPCDialogosGuardados + IDInter.ToString() + "-" + IDDialogo.ToString() + ".xml");

			//Se ha encontrado en la cola de objetos
			if(cobj != null)
			{
				ObjetoSerializable objs = cobj.GetObjeto();
				dialog = objs as NPC_Dialogo;
			}
			//No se ha encontrado en la cola de objetos, buscamos en los directorios
			else
			{
				//Cargamos el dialogo
				//Si existe un fichero guardado, cargamos ese fichero, sino cargamos el fichero por defecto
				if (System.IO.File.Exists(Manager.rutaNPCDialogosGuardados + IDInter.ToString()  + "-" + IDDialogo.ToString() + ".xml"))
				{
					dialog = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogosGuardados + IDInter.ToString()  + "-" + IDDialogo.ToString() + ".xml");
				}
				else
				{
					dialog = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogos + IDInter.ToString()  + "-" + IDDialogo.ToString() + ".xml");
				}
			}
		}

		return dialog;
	}

	private void CrearGrupo(int IDGrupo, int tipo_dialogo, ref int pos_dialogo)
	{
		//Si el grupo no está activo y no está en la lista de grupos acabados, lo añadimos
		if (!Manager.Instance.GrupoActivoExiste(IDGrupo) && !Manager.Instance.GrupoAcabadoExiste(IDGrupo))
		{
			//Comprobamos si el grupo se encuentra como grupo modificado en la lista colaObjetos del Manager
			ColaObjeto cobj = Manager.Instance.GetColaObjetos(Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml");

			//El grupo se ha encontrado en la cola de objetos
			if(cobj != null)
			{
				ObjetoSerializable objs = cobj.GetObjeto();
				Grupo g = objs as Grupo;
				Grupo.LoadGrupo(g, ID_NPC, ID, tipo_dialogo, ref pos_dialogo);

				//Borramos el grupo modificado de la cola ahora que ya ha sido añadido
				Manager.Instance.RemoveFromColaObjetos(Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml");
			}
			//El grupo no se ha encontrado en la cola de objetos, miramos en los directorios
			else
			{
				//Miramos primero en la lista de grupos modificados
				if (System.IO.File.Exists (Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml"))
				{
					Grupo.LoadGrupo (Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml", ID_NPC,ID, tipo_dialogo, ref pos_dialogo);
				}
				//Si no está ahí, miramos en el directorio predeterminado
				else
				{
					Grupo.LoadGrupo (Manager.rutaGrupos + IDGrupo.ToString () + ".xml", ID_NPC,ID, tipo_dialogo, ref pos_dialogo);	
				}
			}
		}
	}

	private void EliminarGrupo(int IDGrupo, int tipo_dialogo, int pos_tema, ref int pos_dialogo)
	{
		//Solo se pueden eliminar grupos activos actualmente
		if(Manager.Instance.GrupoActivoExiste(IDGrupo))
		{
			//Empezamos destruyendo los intros/mensajes del dialogo actual
			switch(tipo_dialogo)
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
						if (j < pos_dialogo)
						{
							this.intros.RemoveAt(j);
							pos_dialogo--;
						}
						//Si la intro a destruir es el actual, lo destruimos al acabar el diálogo de la intro (activando la autodestruccion)
						else if(j == pos_dialogo)
						{
							intros [pos_dialogo].ActivarAutodestruye();
						}
						else if(j > pos_dialogo)
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
						if (j < pos_dialogo)
						{
							this.mensajes.RemoveAt(j);
							pos_dialogo--;
						}
						//Si el mensaje a destruir es el actual, lo destruimos al final (activando la autodestruccion)
						else if(j == pos_dialogo)
						{
							mensajes [pos_dialogo].ActivarAutodestruye();
						}
						else if(j > pos_dialogo)
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
								if(j == pos_tema && k == pos_dialogo)
								{
									//Mantenemos el indice en una posicion correcta
									if (k < pos_dialogo)
									{
										this.temaMensajes[j].mensajes.RemoveAt(k);
										pos_dialogo--;

										//Si el temaMensajes se ha quedado vacío, lo destruimos
										if(temaMensajes[j].mensajes.Count == 0)
										{
											this.temaMensajes.RemoveAt(j);
										}
									}
									//Si el mensaje a destruir es el actual, lo destruimos al final (activando la autodestruccion)
									else if(k == pos_dialogo)
									{
										temaMensajes[j].mensajes [pos_dialogo].ActivarAutodestruye();
									}
									else if(k > pos_dialogo)
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
							if(j == pos_tema && k == pos_dialogo)
							{
								temaMensajes[j].mensajes [pos_dialogo].ActivarAutodestruye();
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
			List<ObjetoSerializable> listCola = new List<ObjetoSerializable>();
			listCola = Manager.Instance.GetColaObjetosTipo(typeof(NPC_Dialogo));

			for(var j = 0; j < listCola.Count; j++)
			{
				//Indica si el dialogo se ha actualizado, eliminando algún elemento
				//Si lo ha hecho, se añade posteriormente a la cola de objetos para serializar
				bool actualizado = false;

				ObjetoSerializable objs = listCola[j];
				NPC_Dialogo n_diag = objs as NPC_Dialogo;

				//Borramos las intros que forman parte del grupo eliminado
				for(int k = n_diag.DevuelveNumeroIntros() - 1; k >= 0; k--)
				{
					if(n_diag.intros[k].IDGrupo == IDGrupo)
					{
						n_diag.intros.RemoveAt(k);
						actualizado = true;
					}
				}

				//Borramos los mensajes con temamensajes con el idgrupo del grupo eliminado
				//o aquellos temamensajes con este grupo
				for(int k = n_diag.DevuelveNumeroTemaMensajes() - 1; k >= 0; k--)
				{
					//si el tema no tiene idgrupo (su valor es -1), comprobamos los idgrupos de los mensajes de su interior
					if(n_diag.temaMensajes[k].IDGrupo == -1)
					{
						for(int l = n_diag.temaMensajes[k].mensajes.Count - 1; l >= 0; l--)
						{
							if(n_diag.temaMensajes[k].mensajes[l].IDGrupo == IDGrupo)
							{
								n_diag.temaMensajes[k].mensajes.RemoveAt(l);

								//Si el temaMensajes se ha quedado vacío, lo destruimos
								if(n_diag.temaMensajes[k].mensajes.Count == 0)
								{
									n_diag.temaMensajes.RemoveAt(k);
								}

								actualizado = true;
							}
						}
					}
					else if(n_diag.temaMensajes[k].IDGrupo == IDGrupo)
					{
						n_diag.temaMensajes.RemoveAt(k);
						actualizado = true;
					}
				}

				//Borramos los mensajes con el idgrupo del grupo eliminado
				for(int k = n_diag.DevuelveNumeroMensajes() - 1; k >= 0; k--)
				{
					if(n_diag.mensajes[k].IDGrupo == IDGrupo)
					{
						n_diag.mensajes.RemoveAt(k);
						actualizado = true;
					}
				}

				//Si se ha actualizado el dialogo, lo añadimos en la cola de objetos para serializarlo
				if (actualizado) {
					n_diag.AddToColaObjetos ();
				}
			}

			//Ahora comprobamos a los npcs de la escena actual
			List<GameObject> interactuables = Manager.Instance.GetAllInteractuables();

			//Lista de dialogos no actualizados, que será utilizada al comprobar los ficheros posteriormente
			List<NPC_Dialogo> dialogosNoActualizados = new List<NPC_Dialogo>();

			for(int j = 0; j < interactuables.Count; j++)
			{
				GameObject gobj = interactuables[j];
				Interactuable inter = gobj.GetComponent<Interactuable>() as Interactuable;
				List<NPC_Dialogo> dialogos = inter.DevolverDialogos();

				for(int z = 0; z < dialogos.Count; z ++)
				{
					NPC_Dialogo n_diag = dialogos[z];

					//Si el diálogo no está en la cola de objetos, miramos si hay que borrar algo
					//Y el dialogo no es el actual
					if(!Manager.Instance.ColaObjetoExiste(Manager.rutaNPCDialogosGuardados + inter.ID.ToString() + "-" + n_diag.ID.ToString() + ".xml") && !(ID_NPC == inter.ID && ID == n_diag.ID))
					{
						//Indica si el dialogo se ha actualizado, eliminando algún elemento
						//Si lo ha hecho, se añade posteriormente a la cola de objetos para serializar
						bool actualizado = false;

						//Eliminamos las intros cuyo IDGrupo coincide con el grupo eliminado
						for(int k = n_diag.DevuelveNumeroIntros() - 1; k >= 0; k--)
						{
							if(n_diag.intros[k].IDGrupo == IDGrupo)
							{
								n_diag.intros.RemoveAt(k);
								actualizado = true;
							}
						}

						//Eliminamos aquellos mensajes dentro de los temamensajes cuyo IDGrupo coincide con el que hemos eliminado
						//o aquellos temamensajes con idgrupo
						for(int k = n_diag.DevuelveNumeroTemaMensajes() - 1; k >= 0; k--)
						{
							//si el tema no tiene idgrupo (su valor es -1), comprobamos los idgrupos de los mensajes de su interior
							if(n_diag.temaMensajes[k].IDGrupo == -1)
							{
								for(int l = n_diag.temaMensajes[k].mensajes.Count - 1; l >= 0; l--)
								{
									if(n_diag.temaMensajes[k].mensajes[l].IDGrupo == IDGrupo)
									{
										n_diag.temaMensajes[k].mensajes.RemoveAt(l);

										//Si el temaMensajes se ha quedado vacío, lo destruimos
										if(n_diag.temaMensajes[k].mensajes.Count == 0)
										{
											n_diag.temaMensajes.RemoveAt(k);
										}

										actualizado = true;
									}
								}
							}
							else if(n_diag.temaMensajes[k].IDGrupo == IDGrupo)
							{
								n_diag.temaMensajes.RemoveAt(k);
								actualizado = true;
							}
						}

						//Borramos los mensajes con el idgrupo del grupo eliminado
						for(int k = n_diag.DevuelveNumeroMensajes() - 1; k >= 0; k--)
						{
							if(n_diag.mensajes[k].IDGrupo == IDGrupo)
							{
								n_diag.mensajes.RemoveAt(k);
								actualizado = true;
							}
						}

						//Si se ha actualizado, se añade a la colaObjetos
						//sino, se añade a la lista de dialogos no actualizados
						if(actualizado)
							n_diag.AddToColaObjetos();
						else
							dialogosNoActualizados.Add(n_diag);
					}
				}
			}

			//Ahora recorremos los ficheros guardadados
			var info = new DirectoryInfo(Manager.rutaNPCDialogosGuardados);
			var fileInfo = info.GetFiles().OrderByDescending( f => f.CreationTime).ToArray(); //los nuevos empiezan al principio de la lista

			for(var j = 0; j < fileInfo.Length; j++)
			{
				//Contiene id_npc-id_diag
				string ids = Path.GetFileNameWithoutExtension(fileInfo[j].Name);

				//Divide el string en
				//arr[0] = ID_NPC
				//arr[1] = ID_DIAG
				string[] arr = ids.Split('-');

				//Comprobamos que no es el dialogo actual
				//O uno de los dialogos que hemos comprobado anteriormente de la escena
				if(!(ID_NPC == Int32.Parse(arr[0]) && ID == Int32.Parse(arr[1])) && !(dialogosNoActualizados.Any(x => x.ID_NPC == Int32.Parse(arr[0]) && x.ID == Int32.Parse(arr[1]))))
				{
					//Buscamos en la cola de objetos
					//Si existe, no hacemos nada
					//Si no existe, comprobamos el dialogo
					if(!Manager.Instance.ColaObjetoExiste(Manager.rutaNPCDialogosGuardados + ids  + ".xml"))
					{
						NPC_Dialogo n_diag = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogosGuardados + ids  + ".xml");

						//Indica si el dialogo se ha actualizado, eliminando algún elemento
						//Si lo ha hecho, se añade posteriormente a la cola de objetos para serializar
						bool actualizado = false;

						for(int k = n_diag.DevuelveNumeroIntros() - 1; k >= 0; k--)
						{
							if(n_diag.intros[k].IDGrupo == IDGrupo)
							{
								n_diag.intros.RemoveAt(k);
								actualizado = true;
							}
						}

						for(int k = n_diag.DevuelveNumeroTemaMensajes() - 1; k >= 0; k--)
						{
							//si el tema no tiene idgrupo, comprobamos los idgrupos de los mensajes de su interior
							//Si el temaMensaje es -1, no forma parte de ningún grupo
							if(n_diag.temaMensajes[k].IDGrupo == -1)
							{
								for(int l = n_diag.temaMensajes[k].mensajes.Count - 1; l >= 0; l--)
								{
									if(n_diag.temaMensajes[k].mensajes[l].IDGrupo == IDGrupo)
									{
										n_diag.temaMensajes[k].mensajes.RemoveAt(l);

										//Si el temaMensajes se ha quedado vacío, lo destruimos
										if(n_diag.temaMensajes[k].mensajes.Count == 0)
										{
											n_diag.temaMensajes.RemoveAt(k);
										}

										actualizado = true;
									}
								}
							}
							else if(n_diag.temaMensajes[k].IDGrupo == IDGrupo)
							{
								n_diag.temaMensajes.RemoveAt(k);
								actualizado = true;
							}
						}

						for(int k = n_diag.DevuelveNumeroMensajes() - 1; k >= 0; k--)
						{
							if(n_diag.mensajes[k].IDGrupo == IDGrupo)
							{
								n_diag.mensajes.RemoveAt(k);
								actualizado = true;
							}
						}

						if (actualizado) {
							n_diag.AddToColaObjetos ();
						}
					}
				}
			}
		}

		//Por último, eliminamos el grupo del Manager
		//Si el grupo no está en la lista de grupos activos, se añade a la lista de grupos acabados y no podrá ser añadido
		Manager.Instance.RemoveFromGruposActivos(IDGrupo);

		//FALTA ENVIAR LOS MENSAJES/INTROS DE FINALIZACIÓN
		//ESTOS SOLO PODRÁN SER ENVIADOS SI EL GRUPO ESTABA EN GRUPOS ACTIVOS
	}

	//Devuelve el dialogo de un xml indicado en la ruta
	public static NPC_Dialogo LoadNPCDialogue(string path)
	{
		NPC_Dialogo npc_dialogo = Manager.Instance.DeserializeData<NPC_Dialogo>(path);

		return npc_dialogo;
	}

	//Añade el dialogo a la cola de objetos serializables
	public void AddToColaObjetos()
	{
		Manager.Instance.AddToColaObjetos (Manager.rutaNPCDialogosGuardados + ID_NPC.ToString() + "-" + ID.ToString()  + ".xml", this);
	}
}