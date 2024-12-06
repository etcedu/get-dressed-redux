using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor (typeof(ArrayTest))]
//[CanEditMultipleObjects]
public class ArrayTestEditor : Editor 
{
	public void OnEnable()
	{ }

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		serializedObject.DrawArray("Int Array", "intArray", true);
		serializedObject.DrawArray("Object Array", "objArray", true);
		serializedObject.DrawArray("Float Array", "floatArray", true);
		//serializedObject.DrawArray<Clothing>("Clothing Array", "clothingArray");
		serializedObject.ApplyModifiedProperties();
	}
}
