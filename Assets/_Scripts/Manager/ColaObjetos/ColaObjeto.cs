﻿/*
 * 	Clase que se usa para almacenar un objetoSer (serializable) en una lista donde todos los objetos son de esta clase
 */
public class ColaObjeto
{
	private ObjetoSerializable objeto;
	private string ruta; //Indica en que directorio, con el nombre del fichero, se debe serializar el objetoSer

	public ColaObjeto(ObjetoSerializable objetoSerializable, string ruta) 
	{
		objeto = objetoSerializable;
		this.ruta = ruta;
	}

	public ObjetoSerializable GetObjeto()
	{
		return objeto;
	}

	public string GetRuta()
	{
		return ruta;
	}
}
