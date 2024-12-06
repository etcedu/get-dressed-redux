/*using UnityEngine;
using UnityEditor;

public class TexturePostProcessor : AssetPostprocessor
{
	void OnPostprocessTexture(Texture2D texture)
	{
		TextureImporter importer = assetImporter as TextureImporter;
		importer.textureType = TextureImporterType.Advanced;
		importer.npotScale = TextureImporterNPOTScale.ToNearest;
		importer.generateCubemap = TextureImporterGenerateCubemap.None;
		importer.grayscaleToAlpha = false;
		importer.alphaIsTransparency = true;
		importer.spriteImportMode = SpriteImportMode.Single;
		importer.spritePixelsPerUnit = 100;
		importer.mipmapEnabled = false;
		importer.wrapMode = TextureWrapMode.Clamp;
		importer.filterMode = FilterMode.Bilinear;
		importer.anisoLevel = 16;
		
		Object asset = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Texture2D));
		if (asset)
		{
			EditorUtility.SetDirty(asset);
		}
		else
		{
			texture.alphaIsTransparency = true;
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.filterMode = FilterMode.Bilinear;
			texture.anisoLevel = 16;        
		} 
	}
}*/