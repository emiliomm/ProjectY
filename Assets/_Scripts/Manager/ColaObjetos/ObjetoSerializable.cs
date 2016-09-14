using System.Xml.Serialization;

/*
 * 	Clase vacía que se usa para serializar en una misma lista a diferentes objetos derivados de esta
 * 
 *  Cada clase derivada debe incluir un [XmlInclude(typeof(nombreClaseDerivada))] para que la serialización/deserialización funcione correctamente
 *  Además, en la clase derivada se debe incluir un [XmlRoot("ObjetoSer")]
 */

[XmlInclude(typeof(Grupo))]
[XmlInclude(typeof(Dialogo))]
[XmlInclude(typeof(InterDatos))]
[XmlInclude(typeof(Inventario))]
public class ObjetoSerializable{  }
