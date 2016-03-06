using UnityEngine;
using System.Collections;
using System.IO;

using UnityEngine.SceneManagement;

public class Load : MonoBehaviour {

	// Use this for initialization
	void Start () {
		string copyTo = Application.persistentDataPath + "/NPC_Dialogo_Saves/";

		if (!System.IO.Directory.Exists(copyTo))
		{
			System.IO.Directory.CreateDirectory(copyTo);
		}

		SceneManager.LoadScene("Demo");
	}
}
