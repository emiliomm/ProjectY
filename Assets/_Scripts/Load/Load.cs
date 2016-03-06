using UnityEngine;
using System.Collections;
using System.IO;

using UnityEngine.SceneManagement;

public class Load : MonoBehaviour {

	// Use this for initialization
	void Start () {
		string path = Application.dataPath + "/Resources/";
		string copyTo = Application.persistentDataPath + "/NPC_Dialogo_Saves/";

		if (!System.IO.Directory.Exists(copyTo))
		{
			System.IO.Directory.CreateDirectory(copyTo);
		}

		foreach (string file in System.IO.Directory.GetFiles(path))
		{
			TextAsset xmlAsset;
			string xmlContent;

			if (Path.GetExtension(file) == ".xml")
			{
				if(!System.IO.File.Exists(copyTo + Path.GetFileName(file)))
				{
					xmlAsset = Resources.Load(Path.GetFileNameWithoutExtension(file)) as TextAsset;
					xmlContent = xmlAsset.text;

					System.IO.File.WriteAllText(copyTo + Path.GetFileName(file), xmlContent);
				}
			}
		}

		SceneManager.LoadScene("Demo");
	}
}
