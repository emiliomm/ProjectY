using UnityEngine;

/*
 * 	Clase que crea un rectángulo de visión de la cámara
 *  Autor clase original: Tutorial Cámara 3DBuzz (https://www.3dbuzz.com/training/view/3rd-person-character-system)
 * 	Modificada por mí
 */
public static class Helper
{
	//estructura con los 4 puntos del rectangulo de vision de la camara que vamos a calcular
	public struct ClipPlanePoints
	{
		public Vector3 upperLeft;
		public Vector3 upperRight;
		public Vector3 lowerLeft;
		public Vector3 lowerRight;
	}

	//Calcula el angulo dela camara al moverla entre el eje Y limitado entre dos valores
	public static float ClampAngle(float angle, float minAngle, float maxAngle)
	{
		//se ejecuta hasta que el angulo este entre -360 y 360
		do
		{
			if (angle < -360)
				angle += 360;
			if (angle > 360)
				angle -= 360;
		}while (angle < -360 || angle > 360);

		//Devuelve el valor entre el min y el max especificado
		return Mathf.Clamp (angle, minAngle, maxAngle);
	}

	//Calcula el rectangulo de vision basado en la posicion de la camara y del vector que le pasamos
	public static ClipPlanePoints ClipPlaneAtNear(Vector3 pos)
	{
		var clipPlanePoints = new ClipPlanePoints();

		//Si la camera no existe, no creamos el cuadrado
		if (Camera.main == null)
			return clipPlanePoints;

		var transform = Camera.main.transform;
		//Necesitamos la mitad del fov en radianes
		var halfFOV =  (Camera.main.fieldOfView/2) * Mathf.Deg2Rad;
		var aspect = Camera.main.aspect;
		var distance = Camera.main.nearClipPlane;
		//Como se trata de un triangulo rectangulo, con la tangente de la mitad del FOV, hallamos la altura
		var height = distance * Mathf.Tan(halfFOV);
		var width = height * aspect;

		clipPlanePoints.lowerRight = pos + transform.right * width; //lo movemos a la der
		clipPlanePoints.lowerRight -= transform.up * height; //lo movemos hacia abajo
		clipPlanePoints.lowerRight += transform.forward * distance; //Lo movemos hacia delante la distancia + 1 unidad, para que este un poco por delante de la camera

		clipPlanePoints.lowerLeft = pos - transform.right * width; //lo movemos a la izq
		clipPlanePoints.lowerLeft -= transform.up * height; //lo movemos hacia abajo
		clipPlanePoints.lowerLeft += transform.forward * distance; //Lo movemos hacia delante la distancia + 1 unidad, para que este un poco por delante de la camera

		clipPlanePoints.upperRight = pos + transform.right * width; //lo movemos a la der
		clipPlanePoints.upperRight += transform.up * height; //lo movemos hacia arriba
		clipPlanePoints.upperRight += transform.forward * distance; //Lo movemos hacia delante la distancia + 1 unidad, para que este un poco por delante de la camera

		clipPlanePoints.upperLeft = pos - transform.right * width; //lo movemos a la izq
		clipPlanePoints.upperLeft += transform.up * height; //lo movemos hacia arriba
		clipPlanePoints.upperLeft += transform.forward * distance; //Lo movemos hacia delante la distancia + 1 unidad, para que este un poco por delante de la camera

		return clipPlanePoints;
	}
}


