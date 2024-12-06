using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

public class TestEditorWindow : EditorWindow
{
	[MenuItem ("Test/Test Window")]
	public static void ShowTestEditorWindow()
	{
		var window = GetWindow<TestEditorWindow>();
		window.titleContent = new UnityEngine.GUIContent("The Window");
	}

	private Color[,] pixels;
	private int width;
	private int height;
	private bool editLive;
	private Texture2D pixelBGTexture;
	private Color selectedColor;
	private Color eraseColor;
	private Renderer textureTarget;

	public void OnEnable()
	{
		pixelBGTexture = EditorGUIUtility.whiteTexture;
		selectedColor = Color.white;

		if(textureTarget == null || textureTarget.sharedMaterial.mainTexture == null)
		{
			width = height = 8;
			pixels = new Color[width, height];
			for(int i = 0; i < width; i++)
				for(int j = 0; j < height; j++)
					pixels[i, j] = RandomColor();
		}
		else
			LoadTargetTexture();
	}

	private Color RandomColor()
	{
		return new Color(Random.value, Random.value, Random.value, 1.0f);
	}

	public void OnGUI()
	{
		CustomEditorTools.SetLabelWidth(80);
		GUILayout.BeginHorizontal();
		DoControls();

		DoCanvas();
		GUILayout.EndHorizontal();
	}

	void DoControls()
	{
		GUILayout.MaxWidth(200);
		GUILayout.BeginVertical();
		GUILayout.Label("Toolbar", EditorStyles.largeLabel);

		selectedColor = EditorGUILayout.ColorField("Paint Color", selectedColor);
		eraseColor = EditorGUILayout.ColorField("Erase Color", eraseColor);
		if(GUILayout.Button("Fill All"))
			for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				pixels[i, j] = selectedColor;
			}
		}

		GUILayout.FlexibleSpace();

		int oldWidth = width;
		int oldHeight = height;
		width = Mathf.Clamp(EditorGUILayout.IntField("Width", width), 2, 2048);
		height = Mathf.Clamp(EditorGUILayout.IntField("Height", height), 2, 2048);
		if(GUI.changed)
		{
			Color[,] oldPixels = new Color[oldWidth, oldHeight];
			for(int i = 0; i < oldWidth; i++)
			{
				for(int j = 0; j < oldHeight; j++)
				{
					oldPixels[i, j] = pixels[i, j];
				}
			}
			pixels = new Color[width, height];
			for(int i = 0; i < width; i++)
			{
				int copyWidth = i % oldWidth;
				for(int j = 0; j < height; j++)
				{
					int copyHeight = j % oldHeight;
					pixels[i, j] = oldPixels[copyWidth, copyHeight];
				}
			}

			if(editLive)
			{
				Texture2D texture = new Texture2D(width, height);
				texture.filterMode = textureTarget.sharedMaterial.mainTexture.filterMode;
				textureTarget.sharedMaterial.mainTexture = texture;
			}
		}

		GUILayout.FlexibleSpace();

		GUILayout.BeginHorizontal();
		textureTarget = EditorGUILayout.ObjectField("Output", textureTarget, typeof(Renderer), true) as Renderer;
		if(textureTarget != null && GUILayout.Button("X", GUILayout.Width(20)))
			textureTarget = null;
		GUILayout.EndHorizontal();

		var oldEnabled = GUI.enabled;

		GUI.enabled = textureTarget != null && textureTarget.sharedMaterial.mainTexture != null;
		if(GUILayout.Button("Load from Object"))
		{
			LoadTargetTexture();
		}

		GUILayout.BeginHorizontal();
		GUI.enabled = textureTarget != null && !editLive;
		if(GUILayout.Button("Save to Object"))
		{
			Texture2D texture = new Texture2D(width, height);
			texture.filterMode = FilterMode.Point;
			textureTarget.material = new Material(Shader.Find("Diffuse"));
			textureTarget.sharedMaterial.mainTexture = texture;

			SaveToTexture(texture);
		}

		GUI.enabled = textureTarget != null;
		editLive = GUILayout.Toggle(editLive, "Live", GUILayout.Width(50));
		if(editLive)
		{
			editLive = textureTarget != null && textureTarget.sharedMaterial.mainTexture != null;
			if(editLive)
			{
				SaveToTexture(textureTarget.sharedMaterial.mainTexture as Texture2D);
			}
		}
		GUILayout.EndHorizontal();

		GUI.enabled = oldEnabled;

		GUILayout.EndVertical();
	}

	void DoCanvas()
	{
		var evt = Event.current;
		
		var oldColor = GUI.color;
		GUILayout.BeginHorizontal();
		for(int i = 0; i < width; i++)
		{
			GUILayout.BeginVertical();
			for(int j = 0; j < height; j++)
			{
				GUI.color = pixels[i, j];
				var pixelRect = GUILayoutUtility.GetRect(GUIContent.none, "Box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
				GUI.DrawTexture(pixelRect, pixelBGTexture);
				if(pixelRect.Contains(evt.mousePosition))
				{
					if(evt.type == EventType.MouseDown)
					{
						if(evt.button == 0)
							pixels[i, j] = selectedColor;
						else if(evt.button == 1)
							pixels[i, j] = eraseColor;
						else if(evt.button == 2)
							pixels[i, j] = RandomColor();
						evt.Use();
					} else if(evt.type == EventType.MouseDrag)
					{
						if(evt.button == 0)
							pixels[i, j] = selectedColor;
						else if(evt.button == 1)
							pixels[i, j] = eraseColor;
						evt.Use();
					}
				}
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndHorizontal();
		GUI.color = oldColor;
	}

	void LoadTargetTexture()
	{
		Texture2D texture = textureTarget.sharedMaterial.mainTexture as Texture2D;
		height = texture.height;
		width = texture.width;
		
		pixels = new Color[width, height];
		for(int i = 0; i < width; i++)
			for(int j = 0; j < height; j++)
				pixels[i, j] = texture.GetPixel(i, j);
				//pixels[j + i * height] = texture.GetPixel(width - 1 - i, j);
	}

	void SaveToTexture(Texture2D texture)
	{
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				texture.SetPixel(i, j, pixels[width - 1 - i, j]);
				//var index = j + i * height;
				//texture.SetPixel(width - 1 - i, j, pixels[index]);
			}
		}
		texture.Apply();
	}
}

