using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class ImageManager : MonoBehaviour {

	List<string> images = new List<string>();
	List<Texture2D> textures = new List<Texture2D>();
	List<List<List<int>>> points = new List<List<List<int>>>();
	int currentImage = 0;
	public Text pointsText;
	public Text imageText;
	public Texture2D dot;

	float width ;
	float height;

	float top;
	float left;
	float bottom;
	float right;
	public Texture2D cursorTexture;
	public Texture2D eraserTexture;
	bool imagesLoaded = false;

	void Start(){
		GameObject scroll = GameObject.Find("Scroll View");
		scroll.SetActive(false);
		scroll.SetActive(true);
		
		float maxWidth = Screen.resolutions[Screen.resolutions.Length - 1].width;
		float setWidth = maxWidth * 3 / 4;

		Screen.SetResolution(Mathf.RoundToInt(setWidth), Mathf.RoundToInt(setWidth * 3 / 5), false);
		
	}

	void Update(){
		
		float x = Input.mousePosition[0];
		float y = Input.mousePosition[1];
		if (x >= this.left && x <= Screen.width - this.right && y <= Screen.height - this.top && y >= this.bottom && imagesLoaded)
		{
			
			if (Input.GetMouseButton(1)){
				Cursor.SetCursor(eraserTexture, new Vector2(46,46), CursorMode.Auto);
			}
			else{
				Cursor.SetCursor(cursorTexture, new Vector2(32,32), CursorMode.Auto);
			}
		}
		else{
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}

		if (this.textures.Count > 0){
			if (Input.GetMouseButtonDown(0)){
				if (x >= this.left && x <= Screen.width - this.right && y <= Screen.height - this.top && y >= this.bottom){			
					List<int> position = new List<int>();
					int new_x = Mathf.Min(Mathf.Max(Mathf.RoundToInt((x - this.left)*1920/this.width), 0), 1919);
					int new_y = Mathf.Min(Mathf.Max(Mathf.RoundToInt((y - this.bottom)*1080/this.height), 0), 1079);
					position.Add(new_x);
					position.Add(new_y);
					this.points[this.currentImage].Add(position);
					
					updateAll();
				}
				
							
			}
			if (Input.GetMouseButton(1)){
				if (x >= this.left && x <= Screen.width - this.right && y <= Screen.height - this.top && y >= this.bottom){			
					int new_x = Mathf.Min(Mathf.Max(Mathf.RoundToInt((x - this.left)*1920/this.width), 0), 1919);
					int new_y = Mathf.Min(Mathf.Max(Mathf.RoundToInt((y - this.bottom)*1080/this.height), 0), 1079);
					

					foreach (List<int> position in this.points[this.currentImage]){
						float dis_x = Mathf.Abs(position[0] - new_x);
						float dis_y = Mathf.Abs(position[1] - new_y);

						if(dis_x < 25 && dis_y < 25){
							this.points[this.currentImage].Remove(position);
							updateAll();
							break;
						}
					}
				}	
			}
		}

		if (Input.GetKeyDown("left"))
        {
            PreviousImage();
        }
		if (Input.GetKeyDown("right"))
        {
            NextImage();
        }

		

		width = Screen.width - 240;
		height = width * 9 / 16;

		this.top = (Screen.height - this.height - 120)/2;
		this.left = -(this.width - Screen.width)/2;
		this.bottom = Screen.height - this.height - this.top;
		this.right = Screen.width - this.width - this.left;

	}

	public void saveLabels(){
		int i = 0;
		List<string> labels = new List<string>();
		foreach(List<List<int>> image in this.points){
			string s = this.images[i];
			foreach(List<int> point in image){
				s = s + "," + point[0].ToString() + " " + point[1].ToString();
			}
			i += 1;
			labels.Add(s);
		}
		gameObject.GetComponent<FileManager>().saveLabels(labels);
	}

	public void LoadImages () {
		this.currentImage = 0;
		List<string> images =  gameObject.GetComponent<FileManager>().loadImageFolder();

		if(images.Count != 0){
			this.images.Clear();
			this.textures.Clear();
			this.points.Clear();

			foreach(string image in images){
				this.images.Add(Path.GetFileName(image));
				this.textures.Add(LoadImage(image));
				this.points.Add(new List<List<int>>());
			}
			imagesLoaded = true;
			updateAll();
		}
	}

	public void updatePoints(){
		string t = "";
		foreach(List<int> points in this.points[this.currentImage]){
			t = t + "(" + points[0].ToString() + ", " + points[1].ToString() + ")\n";
		}
		pointsText.text = t;
	}

	public void NextImage(){
		if(imagesLoaded){
			if(this.currentImage < this.textures.Count -1)
			this.currentImage += 1;
			updateAll();
		}
		
	}
	public void PreviousImage(){
		if(imagesLoaded){
			if(this.currentImage > 0)
			this.currentImage -= 1;
			updateAll();
		}
	}
	public void ClearCurrentPoints(){
		if(imagesLoaded){
			this.points[this.currentImage].Clear();
			updateAll();
		}
		
	}

	public void updateAll(){
		updatePoints();
		imageText.text = this.images[this.currentImage];
	}

	
	void OnGUI()
    {
		
        if (this.textures.Count > 0){
			GUI.DrawTexture(new Rect(this.left, this.top, this.width, this.height), this.textures[this.currentImage], ScaleMode.StretchToFill, true, 1.0F);
			
			foreach(List<int> points in this.points[this.currentImage]){
				float x = points[0] * this.width/1920 + this.left;
				float y = points[1] * this.height/1080 + this.bottom;
				
				GUI.DrawTexture(new Rect(x - 10, Screen.height - y - 10, 20, 20), dot, ScaleMode.StretchToFill, true, 1.0F);
			}
		}
	}

	public static Texture2D LoadImage(string filePath) {
		Texture2D tex = null;
		byte[] fileData;

		if (File.Exists(filePath)){
			fileData = File.ReadAllBytes(filePath);
			tex = new Texture2D(2, 2);
			tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
		}
		return tex;
 	}
}
