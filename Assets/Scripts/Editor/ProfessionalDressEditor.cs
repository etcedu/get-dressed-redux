using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor (typeof(ProfessionalDress))]
//[CanEditMultipleObjects]
public class ProfessionalDressEditor : Editor 
{
	// Comma Separated Value file, containing data for the clothing items
	[SerializeField]
	static TextAsset ClothingFile;
	// Comma Separated Value file, containing data for the clothing items
	[SerializeField]
	static TextAsset FeedbackFile;
	
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		GUILayout.Space(10);
		CustomEditorTools.SetLabelWidth(50);
		ClothingFile = (TextAsset)EditorGUILayout.ObjectField("Data", ClothingFile, typeof(TextAsset), false);
		EditorGUILayout.BeginHorizontal();
		{
			EditorGUI.BeginDisabledGroup(ClothingFile == null);
			{
				GUILayout.FlexibleSpace();
				if(GUILayout.Button("Import", GUILayout.Width(75)))
				{
					foreach(Object o in serializedObject.targetObjects)
						(o as ProfessionalDress).ParseClothes(ClothingFile, false);
				}
				if(GUILayout.Button("Clear and Import", GUILayout.Width(120)))
				{
					foreach(Object o in serializedObject.targetObjects)
						(o as ProfessionalDress).ParseClothes(ClothingFile, true);
				}
				GUILayout.FlexibleSpace();
			} EditorGUI.EndDisabledGroup();
		} EditorGUILayout.EndHorizontal();
		
		if(CustomEditorTools.DrawHeader("Clothes", serializedObject.ToString() + "_Clothes"))
		{
			CustomEditorTools.BeginContents();
			{
				DrawHairSetup();
				DrawClothingSetup(serializedObject.FindProperty("top"), "Top", "topTrack");
				DrawClothingSetup(serializedObject.FindProperty("bottom"), "Bottom", "bottomTrack");
				DrawClothingSetup(serializedObject.FindProperty("shoes"), "Shoes", "shoesTrack");
				serializedObject.DrawArray("Other", "other", true);
			} CustomEditorTools.EndContents();
		}
		GUILayout.Space(10);
		
		serializedObject.DrawProperty("Pos Tier", "positionTier");
		
		if(CustomEditorTools.DrawHeader("Feedback", serializedObject.ToString() + "_Feedback"))
		{
			CustomEditorTools.BeginContents();
			{
				FeedbackFile = (TextAsset)EditorGUILayout.ObjectField("Data", FeedbackFile, typeof(TextAsset), false);
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUI.BeginDisabledGroup(FeedbackFile == null);
					{
						GUILayout.FlexibleSpace();
						if(GUILayout.Button("Import Male", GUILayout.Width(110)))
						{
							foreach(Object o in serializedObject.targetObjects)
								(o as ProfessionalDress).ParseFeedback(FeedbackFile, true, false);
							FeedbackFile = null;
						}
						if(GUILayout.Button("Import Female", GUILayout.Width(110)))
						{
							foreach(Object o in serializedObject.targetObjects)
								(o as ProfessionalDress).ParseFeedback(FeedbackFile, false, false);
							FeedbackFile = null;
						}
						GUILayout.FlexibleSpace();
					} EditorGUI.EndDisabledGroup();
					if(GUILayout.Button("Clear All", GUILayout.Width(90)))
					{
						foreach(Object o in serializedObject.targetObjects)
						{
							(o as ProfessionalDress).ClearFeedback();
						}
					}
				} EditorGUILayout.EndHorizontal();
				CustomEditorTools.SetLabelWidth(70);
				serializedObject.DrawArray("Informal Interview", "informalFB");
				serializedObject.DrawArray("Casual Interview", "casualFB");
				serializedObject.DrawArray("Business Casual Interview", "busCasFB");
				serializedObject.DrawArray("Business Professional Interview", "busProfFB");
			} CustomEditorTools.EndContents();
		}

		GUILayout.Space(10);
		
		if(CustomEditorTools.DrawHeader("UI Elements", serializedObject.ToString() + "_UIElements"))
		{
			CustomEditorTools.BeginContents();
			{
				if(CustomEditorTools.DrawHeader("Grids", serializedObject.ToString() + "_Grids"))
				{
					CustomEditorTools.BeginContents();
					{
						CustomEditorTools.StartIndent(10, 70);
						GUILayout.BeginVertical();
						serializedObject.DrawProperty("Starter", "startingGrid");
						GUILayout.Space(5);
						serializedObject.DrawProperty("Face", "faceGrid");
						serializedObject.DrawProperty("Top", "topGrid");
						serializedObject.DrawProperty("Bottom", "bottomGrid");
						serializedObject.DrawProperty("Shoes", "shoesGrid");
						serializedObject.DrawProperty("Other", "otherGrid");
						GUILayout.EndVertical();
						CustomEditorTools.EndIndent();
					}CustomEditorTools.EndContents();
				}
				
				GUILayout.Space(5);
				
				if(CustomEditorTools.DrawHeader("Toggles", serializedObject.ToString() + "_Toggles"))
				{
					CustomEditorTools.BeginContents();
					{
						CustomEditorTools.StartIndent(10, 70);
						GUILayout.BeginVertical();
						serializedObject.DrawProperty("Face", "faceToggle");
						serializedObject.DrawProperty("Top", "topToggle");
						serializedObject.DrawProperty("Bottom", "bottomToggle");
						serializedObject.DrawProperty("Shoes", "shoesToggle");
						serializedObject.DrawProperty("Other", "otherToggle");
						GUILayout.EndVertical();
						CustomEditorTools.EndIndent();
					} CustomEditorTools.EndContents();
				}
				
				GUILayout.Space(5);
				
				if(CustomEditorTools.DrawHeader("Buttons", serializedObject.ToString() + "_Buttons"))
				{
					CustomEditorTools.BeginContents();
					{
						CustomEditorTools.StartIndent(10, 120);
						GUILayout.BeginVertical();
						serializedObject.DrawProperty("Previous", "previous");
						serializedObject.DrawProperty("Next", "next");
						serializedObject.DrawProperty("Zoom", "zoomToggle");
						serializedObject.DrawProperty("Base Clothes Button", "baseClothesToggle");
						GUILayout.EndVertical();
						CustomEditorTools.EndIndent();
					} CustomEditorTools.EndContents();
				}
				
				GUILayout.Space(5);
				
				if(CustomEditorTools.DrawHeader("Labels", serializedObject.ToString() + "_Labels"))
				{
					CustomEditorTools.BeginContents();
					{
						CustomEditorTools.StartIndent(10, 110);
						GUILayout.BeginVertical();
						serializedObject.DrawProperty("Clothes Name", "clothingLabel");
						serializedObject.DrawProperty("Face Score", "faceScore");
						serializedObject.DrawProperty("Face Bonus", "faceBonus");
						serializedObject.DrawProperty("Face Feedback", "faceFeedback");
						serializedObject.DrawProperty("Top Score", "topScore");
						serializedObject.DrawProperty("Top Bonus", "topBonus");
						serializedObject.DrawProperty("Top Feedback", "topFeedback");
						serializedObject.DrawProperty("Bottom Score", "bottomScore");
						serializedObject.DrawProperty("Bottom Bonus", "bottomBonus");
						serializedObject.DrawProperty("Bottom Feedback", "bottomFeedback");
						serializedObject.DrawProperty("Shoes Score", "shoesScore");
						serializedObject.DrawProperty("Shoes Bonus", "shoesBonus");
						serializedObject.DrawProperty("Shoes Feedback", "shoesFeedback");
						serializedObject.DrawProperty("Item Score", "otherScore");
						serializedObject.DrawProperty("Item Bonus", "otherBonus");
						serializedObject.DrawProperty("Item Feedback", "otherFeedback");
						serializedObject.DrawProperty("Final Score", "endScoreBar");
						serializedObject.DrawProperty("Company", "companyLabel");
						serializedObject.DrawProperty("Position", "positionLabel");
						GUILayout.EndVertical();
						CustomEditorTools.EndIndent();
					} CustomEditorTools.EndContents();
				}
			}CustomEditorTools.EndContents();
		}

		GUILayout.Space(5);

		ProfessionalDress pD = serializedObject.targetObject as ProfessionalDress;
		NGUIEditorTools.DrawEvents("Evaluate Events", pD, pD.evaluateEvents);

		GUILayout.Space(5);
		
		CustomEditorTools.SetLabelWidth(150);
		serializedObject.DrawProperty("Clothing Anim Prob", "clothingAnimationProbability");
		CustomEditorTools.SetLabelWidth(120);
		serializedObject.DrawProperty("Animator", "characterAnimator");
		serializedObject.DrawProperty("Male Controller", "maleController");
		serializedObject.DrawProperty("Female Controller", "femaleController");
		GUILayout.Space(10);
		serializedObject.DrawProperty("Gender", "gender");

		serializedObject.DrawArray("Male Setup", "maleSetup", true);
		/*if(GUILayout.Button("Copy To Female"))
			(serializedObject.targetObject as ProfessionalDress).femaleSetup = 
				(serializedObject.targetObject as ProfessionalDress).maleSetup;*/
		serializedObject.DrawArray("Female Setup", "femaleSetup", true);
		/*if(GUILayout.Button("Copy To Male"))
			(serializedObject.targetObject as ProfessionalDress).maleSetup = 
				(serializedObject.targetObject as ProfessionalDress).femaleSetup;*/
		
		serializedObject.ApplyModifiedProperties();
	}

	void DrawHairSetup()
	{
		GUILayout.BeginVertical();
		GUILayout.Space(5);
		if(CustomEditorTools.DrawHeader("Face", serializedObject.ToString() + "_Face"))
		{
			float oldLabelWidth = CustomEditorTools.GetLabelWidth();
			CustomEditorTools.SetLabelWidth(20);
			CustomEditorTools.BeginContents();
			{
				SerializedProperty hair = serializedObject.FindProperty("hair");
				serializedObject.DrawArray(hair, "Hair", "clothing", "Face");
				serializedObject.DrawArray(hair, "Hair Renderers", "renderers", "Face");
				GUILayout.Space(5);
				SerializedProperty facialHair = serializedObject.FindProperty("facialHair");
				serializedObject.DrawArray(facialHair, "Facial Hair", "clothing", "Face");
				serializedObject.DrawArray(facialHair, "Facial Hair Renderers", "renderers", "Face");
				CustomEditorTools.SetLabelWidth(80);
				serializedObject.DrawProperty("Tracker", "faceTrack");
				//serializedObject.DrawRelativeProperty("Renderer", )
			}CustomEditorTools.EndContents();
			CustomEditorTools.SetLabelWidth(oldLabelWidth);
		}
		GUILayout.EndVertical();
	}

	void DrawClothingSetup(SerializedProperty clothing, string title, string trackName)
	{
		GUILayout.BeginVertical();
		GUILayout.Space(5);
		if(CustomEditorTools.DrawHeader(title, serializedObject.ToString() + "_" + title))
		{
			float oldLabelWidth = CustomEditorTools.GetLabelWidth();
			CustomEditorTools.SetLabelWidth(20);
			CustomEditorTools.BeginContents();
			{
				serializedObject.DrawArray(clothing, "Clothing", "clothing", title);
				serializedObject.DrawArray(clothing, "Renderers", "renderers", title);
				CustomEditorTools.SetLabelWidth(80);
				serializedObject.DrawProperty("Tracker", trackName);
			}CustomEditorTools.EndContents();
			CustomEditorTools.SetLabelWidth(oldLabelWidth);
		}
		GUILayout.EndVertical();
	}
}
