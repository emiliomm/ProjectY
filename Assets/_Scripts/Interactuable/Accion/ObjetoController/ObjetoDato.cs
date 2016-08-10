using UnityEngine;

/*
 * 	Clase que muestra un punto de información al manipular un objeto de una acción
 */
public class ObjetoDato : MonoBehaviour {

	private Ray rayToCameraPos; //rayo que es lanzado para comprobar colisiones
	private RaycastHit hitInfo; //raycast que almacena si el ray le ha dado al propio objeto
	private LayerMask layerMask; //Indica que layers afectan al ray cast

	private Material materialSprite; //material que usa el sprite del objetoDato por defecto
	private Material materialUI; //material de la UI

	private SpriteRenderer spriteRend; //almacenamos el componente spriterenderer del objetoDato, para modificar el material

	//Inicializa variables
	void Start () {
		hitInfo = new RaycastHit();

		//Carga los dos materiales que se irán alternando en Update
		//El predeterminado del sprite y el UI
		materialSprite = spriteRend.material;
		materialUI = Resources.Load("UI") as Material;

		spriteRend = GetComponent<SpriteRenderer>();

		//Carga la layerMask para que el rayo detecte todos las colisiones con objetos
		//con la layer 8 (UIObjeto), es decir, las colisiones con el objeto, que contiene esta layer
		layerMask = 1 << 8;
	}

	void Update () {

		//Lanzamos y dibujamos el rayo desde el objetoDato hasta la cámara
		rayToCameraPos = new Ray(transform.position, Camera.main.transform.position - transform.position);
		Debug.DrawRay(rayToCameraPos.origin, rayToCameraPos.direction*100, Color.blue);

		//Si el raycast le ha dado al objeto, el objetoDato permanece en la misma posición, permaneciendo no visible
		if(Physics.Raycast(rayToCameraPos, out hitInfo, 1000, layerMask))
		{
			transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
			spriteRend.material = materialSprite;
		}
		//Si el raycast no le ha dado al objeto, cambiamos el material del ObjetoDato para que sea visible totalmente aunque el objeto "le tape".
		//Además, el objetodato permance mirando hacia la cámara hasta que el rayo le de al objeto
		else
		{
			transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
			spriteRend.material = materialUI;
		}
	}
}
