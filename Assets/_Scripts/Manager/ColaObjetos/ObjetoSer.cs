using UnityEngine;
using System.Collections;
using System.Xml; 
using System.Xml.Serialization; 

[XmlInclude(typeof(Grupo))]
[XmlInclude(typeof(NPC_Dialogo))]
[XmlInclude(typeof(InterDatos))]
[XmlInclude(typeof(Inventario))]
public class ObjetoSer{
}
