﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rutina{

	public int ID;
	public List<PosicionLugarSiguiente> posLugarSiguientes;

	public Rutina()
	{
		posLugarSiguientes = new List<PosicionLugarSiguiente>();
	}
}