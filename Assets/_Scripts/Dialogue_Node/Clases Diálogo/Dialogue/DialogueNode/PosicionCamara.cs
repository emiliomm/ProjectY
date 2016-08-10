/*
*   Clase que contiene información sobre la posición de la cámara durante una parte del diálogo
*/
public class PosicionCamara{

	//Indica a donde debe mirar la cámara
	//-1 = jugador
	//0 = NPC que habla
	public int lookAt;

	//Indica la posición de la cámara respecto a quién está mirando a la cámara
	//usando el eje local de este
	public float coordX;
	public float coordY;
	public float coordZ;

	public PosicionCamara()
	{
		
	}
}
