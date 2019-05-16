using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SFB;

public class FileManager : MonoBehaviour {

	string imageFolderPath;
	string labelsPath;

	// Use this for initialization
	void Start() {
	}


	public List<string> loadImageFolder(){
		try{
			this.imageFolderPath = StandaloneFileBrowser.OpenFolderPanel("", "", false)[0];
			DirectoryInfo dir = new DirectoryInfo(this.imageFolderPath);
			FileInfo[] info = dir.GetFiles("*.jpg");
			List<string> l = new List<string>();
			foreach (FileInfo f in info)
			{
				l.Add(f.ToString());			
			}
			return l;
		}
		catch{
			List<string> l = new List<string>();
			return l;
		}
		
	}

	public void saveLabels(List<string> labels){
		try{
			this.labelsPath = StandaloneFileBrowser.SaveFilePanel("Save labels", "", "Labels.txt", "txt");
		
			StreamWriter writer = new StreamWriter(this.labelsPath, true);
			foreach (string l in labels)
			{
				writer.WriteLine(l);
			}
			writer.Close();
		}
		catch{
			return;
		}
		
	}

}
