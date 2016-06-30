using UnityEngine;

public static class Helper
{
	//estructura con los 4 puntos del rectangulo de vision de la camara que vamos a calcular
	public struct ClipPlanePoints
	{
		public Vector3 UpperLeft;
		public Vector3 UpperRight;
		public Vector3 LowerLeft;
		public Vector3 LowerRight;
	}

	//Calcula el angulo dela camara al moverla entre el eje Y limitado entre dos valores
	public static float ClampAngle(float angle, float min, float max)
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
		return Mathf.Clamp (angle, min, max);
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

		clipPlanePoints.LowerRight = pos + transform.right * width; //lo movemos a la der
		clipPlanePoints.LowerRight -= transform.up * height; //lo movemos hacia abajo
		clipPlanePoints.LowerRight += transform.forward * distance; //Lo movemos hacia delante la distancia + 1 unidad, para que este un poco por delante de la camera

		clipPlanePoints.LowerLeft = pos - transform.right * width; //lo movemos a la izq
		clipPlanePoints.LowerLeft -= transform.up * height; //lo movemos hacia abajo
		clipPlanePoints.LowerLeft += transform.forward * distance; //Lo movemos hacia delante la distancia + 1 unidad, para que este un poco por delante de la camera

		clipPlanePoints.UpperRight = pos + transform.right * width; //lo movemos a la der
		clipPlanePoints.UpperRight += transform.up * height; //lo movemos hacia arriba
		clipPlanePoints.UpperRight += transform.forward * distance; //Lo movemos hacia delante la distancia + 1 unidad, para que este un poco por delante de la camera

		clipPlanePoints.UpperLeft = pos - transform.right * width; //lo movemos a la izq
		clipPlanePoints.UpperLeft += transform.up * height; //lo movemos hacia arriba
		clipPlanePoints.UpperLeft += transform.forward * distance; //Lo movemos hacia delante la distancia + 1 unidad, para que este un poco por delante de la camera

		return clipPlanePoints;
	}

	public static ClipPlanePoints ClipPlaneAtNear2(Vector3 pos, Transform cameraTrans, Vector3 targetPos, float offset_used, float percent_used)
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

		clipPlanePoints.LowerRight = pos + transform.right * width; //lo movemos a la der
		clipPlanePoints.LowerRight -= transform.up * height; //lo movemos hacia abajo
		clipPlanePoints.LowerRight += transform.forward * distance; //Lo movemos hacia delante la distancia + 1 unidad, para que este un poco por delante de la camera

		clipPlanePoints.LowerLeft = pos - transform.right * width; //lo movemos a la izq
		clipPlanePoints.LowerLeft -= transform.up * height; //lo movemos hacia abajo
		clipPlanePoints.LowerLeft += transform.forward * distance; //Lo movemos hacia delante la distancia + 1 unidad, para que este un poco por delante de la camera

		clipPlanePoints.UpperRight = pos + transform.right * width; //lo movemos a la der
		clipPlanePoints.UpperRight += transform.up * height; //lo movemos hacia arriba
		clipPlanePoints.UpperRight += transform.forward * distance; //Lo movemos hacia delante la distancia + 1 unidad, para que este un poco por delante de la camera

		clipPlanePoints.UpperLeft = pos - transform.right * width; //lo movemos a la izq
		clipPlanePoints.UpperLeft += transform.up * height; //lo movemos hacia arriba
		clipPlanePoints.UpperLeft += transform.forward * distance; //Lo movemos hacia delante la distancia + 1 unidad, para que este un poco por delante de la camera

		return clipPlanePoints;
	}

}


