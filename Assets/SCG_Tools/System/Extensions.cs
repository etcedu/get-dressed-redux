using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

#region Pair
[Serializable]
public class Pair<T> {
	public T One, Two;
	public Pair(T one, T two) {
		One = one;
		Two = two;
	}
}

[Serializable]
public class Pair<T1, T2> {
	public T1 One;
	public T2 Two;
	public Pair(T1 one, T2 two) {
		One = one;
		Two = two;
	}
}
#endregion

#region IVector2
/// <summary>
/// A Vector2, but composed of integers rather than floats
/// </summary>
[Serializable]
public class IVector2 {
	public int x, y;
	public IVector2() { this.x=0; this.y=0; }
	public IVector2(int all) { this.x = all; this.y = all; }
	public IVector2(int x, int y) { this.x=x; this.y=y; }
	public static IVector2 zero {
		get{return new IVector2();}
	}
	
	public static IVector2 operator +(IVector2 one, IVector2 two) {
		return new IVector2(one.x + two.x, one.y + two.y);
	}
	public static IVector2 operator ++(IVector2 one) {
		return new IVector2(one.x + 1, one.y + 1);
	}
	public static IVector2 operator -(IVector2 one, IVector2 two) {
		return new IVector2(one.x - two.x, one.y - two.y);
	}
	public static IVector2 operator --(IVector2 one) {
		return new IVector2(one.x - 1, one.y - 1);
	}
	public static IVector2 operator *(IVector2 one, int two) {
		return new IVector2(one.x * two, one.y * two);
	}
	public static IVector2 operator /(IVector2 one, int two) {
		return new IVector2(one.x / two, one.y / two);
	}
	
	public static bool operator ==(IVector2 one, IVector2 two) {
		return one.x == two.x && one.y == two.y;
	}
	
	public static bool operator !=(IVector2 one, IVector2 two) {
		return one.x != two.x || one.y != two.y;
	}
	
	public override bool Equals(System.Object other) {
		if(other == null) return false;
		IVector2 oIV2 = other as IVector2;
		if((System.Object)oIV2 == null) return false;
		
		return this.x == oIV2.x && this.y == oIV2.y;
	}
	
	public override int GetHashCode() {
		int hash = 13;
		hash = (hash*7) + this.x.GetHashCode();
		hash = (hash*7) + this.y.GetHashCode();
		return hash;
	}
}
/// <summary>
/// IEqualityComparer for the IVector2 class
/// </summary>
class IVector2Comp : IEqualityComparer<IVector2> {
	public bool Equals(IVector2 one, IVector2 two) {
		return one == two;
	}
	
	public int GetHashCode(IVector2 v2)
	{
		int hCode = v2.x ^ v2.y;
		return hCode.GetHashCode();
	}
}

public static class IVector2Extensions {
	/// <summary>
	/// Change X value of Vector2 to the specified amount
	/// </summary>
	public static IVector2 WithX(this IVector2 v2, int x) {
		return new IVector2(x, v2.y);
	}
	/// <summary>
	/// Change Y value of Vector2 to the specified amount
	/// </summary>
	public static IVector2 WithY(this IVector2 v2, int y) {
		return new IVector2(v2.x, y);
	}
	
	/// <summary>
	/// Adjust X value of Vector2 by the specified amount
	/// </summary>
	public static IVector2 AdjustX(this IVector2 v2, int x) {
		return new IVector2(v2.x + x, v2.y);
	}
	/// <summary>
	/// Adjust Y value of Vector2 by the specified amount
	/// </summary>
	public static IVector2 AdjustY(this IVector2 v2, int y) {
		return new IVector2(v2.x, v2.y + y);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(X,X,0)
	/// </summary>
	public static IVector3 MakeV3XX0(this IVector2 v2) {
		return new IVector3(v2.x, v2.x, 0);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(X,Y,0)
	/// </summary>
	public static IVector3 MakeV3XY0(this IVector2 v2) {
		return new IVector3(v2.x, v2.y, 0);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(X,0,X)
	/// </summary>
	public static IVector3 MakeV3X0X(this IVector2 v2) {
		return new IVector3(v2.x, 0, v2.x);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(X,0,Y)
	/// </summary>
	public static IVector3 MakeV3X0Y(this IVector2 v2) {
		return new IVector3(v2.x, 0, v2.y);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(0,X,X)
	/// </summary>
	public static IVector3 MakeV30XX(this IVector2 v2) {
		return new IVector3(0, v2.x, v2.x);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(0,X,Y)
	/// </summary>
	public static IVector3 MakeV30XY(this IVector2 v2) {
		return new IVector3(0, v2.x, v2.y);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(Y,Y,0)
	/// </summary>
	public static IVector3 MakeV3YY0(this IVector2 v2) {
		return new IVector3(v2.y, v2.y, 0);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(Y,X,0)
	/// </summary>
	public static IVector3 MakeV3YX0(this IVector2 v2) {
		return new IVector3(v2.y, v2.x, 0);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(Y,0,Y)
	/// </summary>
	public static IVector3 MakeV3Y0Y(this IVector2 v2) {
		return new IVector3(v2.y, 0, v2.y);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(Y,0,X)
	/// </summary>
	public static IVector3 MakeV3Y0X(this IVector2 v2) {
		return new IVector3(v2.y, 0, v2.x);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(0,Y,Y)
	/// </summary>
	public static IVector3 MakeV30YY(this IVector2 v2) {
		return new IVector3(0, v2.y, v2.y);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(0,Y,X)
	/// </summary>
	public static IVector3 MakeV30YX(this IVector2 v2) {
		return new IVector3(0, v2.y, v2.x);
	}
	
	
	/// <summary>
	/// Get magnitude between two vectors
	/// </summary>
	public static float magnitude(this IVector2 v2) {
		return Mathf.Sqrt(v2.x * v2.x + v2.y * v2.y);
	}
	
	
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(0,Y,X)
	/// </summary>
	public static Vector2 MakeVector2(this IVector2 v2) {
		return new Vector2(v2.x, v2.y);
	}
	
	
	
	public static string toString(this IVector2 v2) {
		return "("+v2.x+", "+v2.y+")";
	}
}
#endregion

#region IVector3

/// <summary>
/// A Vector3, but composed of integers rather than floats
/// </summary>
[Serializable]
public class IVector3 {
	public int x, y, z;
	public IVector3() { this.x=0; this.y=0; this.z=0; }
	public IVector3(int all) { this.x = all; this.y = all; this.z = all; }
	public IVector3(int x, int y) { this.x=x; this.y=y; this.z=0; }
	public IVector3(int x, int y, int z) { this.x=x; this.y=y; this.z=z; }
	public static IVector3 zero {
		get{return new IVector3();}
	}
	
	public static IVector3 operator +(IVector3 one, IVector3 two) {
		return new IVector3(one.x + two.x, one.y + two.y, one.z + two.z);
	}
	public static IVector3 operator ++(IVector3 one) {
		return new IVector3(one.x + 1, one.y + 1, one.z + 1);
	}
	public static IVector3 operator -(IVector3 one, IVector3 two) {
		return new IVector3(one.x - two.x, one.y - two.y, one.z - two.z);
	}
	public static IVector3 operator --(IVector3 one) {
		return new IVector3(one.x - 1, one.y - 1, one.z - 1);
	}
	public static IVector3 operator *(IVector3 one, int two) {
		return new IVector3(one.x * two, one.y * two, one.z * two);
	}
	public static IVector3 operator /(IVector3 one, int two) {
		return new IVector3(one.x / two, one.y / two, one.z / two);
	}
	
	public static bool operator ==(IVector3 one, IVector3 two) {
		return one.x == two.x && one.y == two.y && one.z == two.z;
	}
	
	public static bool operator !=(IVector3 one, IVector3 two) {
		return one.x != two.x || one.y != two.y || one.z != two.z;
	}
	
	public override bool Equals(System.Object other) {
		if(other == null) return false;
		IVector3 oIV3 = other as IVector3;
		if((System.Object)oIV3 == null) return false;
		
		return this.x == oIV3.x && this.y == oIV3.y && this.z == oIV3.z;
	}
	
	public override int GetHashCode() {
		int hash = 13;
		hash = (hash*7) + this.x.GetHashCode();
		hash = (hash*7) + this.y.GetHashCode();
		hash = (hash*7) + this.z.GetHashCode();
		return hash;
	}
}
/// <summary>
/// IEqualityComparer for the IVector3 class
/// </summary>
class IVector3Comp : IEqualityComparer<IVector3> {
	public bool Equals(IVector3 one, IVector3 two) {
		return one == two;
	}
	
	public int GetHashCode(IVector3 v3)
	{
		int hCode = v3.x ^ v3.y ^ v3.z;
		return hCode.GetHashCode();
	}
}

public static class IVector3Extensions {
	/// <summary>
	/// Change X value of Vector3 to the specified amount
	/// </summary>
	public static IVector3 WithX(this IVector3 v3, int x) {
		return new IVector3(x, v3.y, v3.z);
	}
	/// <summary>
	/// Change Y value of Vector3 to the specified amount
	/// </summary>
	public static IVector3 WithY(this IVector3 v3, int y) {
		return new IVector3(v3.x, y, v3.z);
	}
	/// <summary>
	/// Change Z value of Vector3 to the specified amount
	/// </summary>
	public static IVector3 WithZ(this IVector3 v3, int z) {
		return new IVector3(v3.x, v3.y, z);
	}
	
	/// <summary>
	/// Adjust X value of Vector3 by the specified amount
	/// </summary>
	public static IVector3 AdjustX(this IVector3 v3, int x) {
		return new IVector3(v3.x + x, v3.y, v3.z);
	}
	/// <summary>
	/// Adjust Y value of Vector3 by the specified amount
	/// </summary>
	public static IVector3 AdjustY(this IVector3 v3, int y) {
		return new IVector3(v3.x, v3.y + y, v3.z);
	}
	/// <summary>
	/// Adjust Z value of Vector3 by the specified amount
	/// </summary>
	public static IVector3 AdjustZ(this IVector3 v3, int z) {
		return new IVector3(v3.x, v3.y, v3.z + z);
	}
	
	/// <summary>
	/// Turn Vector3(X,Y,Z) into Vector2(X,X)
	/// </summary>
	public static IVector2 MakeV2XX(this IVector3 v3) {
		return new IVector2(v3.x, v3.x);
	}
	/// <summary>
	/// Turn Vector3(X,Y,Z) into Vector2(X,Y)
	/// </summary>
	public static IVector2 MakeV2XY(this IVector3 v3) {
		return new IVector2(v3.x, v3.y);
	}
	/// <summary>
	/// Turn Vector3(X,Y,Z) into Vector2(X,Z)
	/// </summary>
	public static IVector2 MakeV2XZ(this IVector3 v3) {
		return new IVector2(v3.x, v3.z);
	}
	/// <summary>
	/// Turn Vector3(X,Y,Z) into Vector2(Y,X)
	/// </summary>
	public static IVector2 MakeV2YX(this IVector3 v3) {
		return new IVector2(v3.y, v3.x);
	}
	/// <summary>
	/// Turn Vector3(X,Y,Z) into Vector2(Y,Y)
	/// </summary>
	public static IVector2 MakeV2YY(this IVector3 v3) {
		return new IVector2(v3.y, v3.y);
	}
	/// <summary>
	/// Turn Vector3(X,Y,Z) into Vector2(Y,Z)
	/// </summary>
	public static IVector2 MakeV2YZ(this IVector3 v3) {
		return new IVector2(v3.y, v3.z);
	}
	/// <summary>
	/// Turn Vector3(X,Y,Z) into Vector2(Z,X)
	/// </summary>
	public static IVector2 MakeV2ZX(this IVector3 v3) {
		return new IVector2(v3.z, v3.x);
	}
	/// <summary>
	/// Turn Vector3(X,Y,Z) into Vector2(Z,Y)
	/// </summary>
	public static IVector2 MakeV2ZY(this IVector3 v3) {
		return new IVector2(v3.z, v3.y);
	}
	/// <summary>
	/// Turn Vector3(X,Y,Z) into Vector2(Z,Z)
	/// </summary>
	public static IVector2 MakeV2ZZ(this IVector3 v3) {
		return new IVector2(v3.z, v3.z);
	}
	
	
	
	/// <summary>
	/// Get magnitude between two vectors
	/// </summary>
	public static float magnitude(this IVector3 v3) {
		return Mathf.Sqrt(v3.x * v3.x + v3.y * v3.y + v3.z * v3.z);
	}
	
	
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(0,Y,X)
	/// </summary>
	public static Vector3 MakeVector3(this IVector3 v3) {
		return new Vector3(v3.x, v3.y, v3.z);
	}
	
	
	public static string toString(this IVector3 v3) {
		return "("+v3.x+", "+v3.y+", "+v3.z+")";
	}
}
#endregion

#region Vector3

public static class Vector3Extensions {
	/// <summary>
	/// Change X value of Vector3 to the specified amount
	/// </summary>
	public static Vector3 WithX(this Vector3 v3, float x) {
		return new Vector3(x, v3.y, v3.z);
	}
	/// <summary>
	/// Change Y value of Vector3 to the specified amount
	/// </summary>
	public static Vector3 WithY(this Vector3 v3, float y) {
		return new Vector3(v3.x, y, v3.z);
	}
	/// <summary>
	/// Change Z value of Vector3 to the specified amount
	/// </summary>
	public static Vector3 WithZ(this Vector3 v3, float z) {
		return new Vector3(v3.x, v3.y, z);
	}
	
	/// <summary>
	/// Adjust X value of Vector3 by the specified amount
	/// </summary>
	public static Vector3 AdjustX(this Vector3 v3, float x) {
		return new Vector3(v3.x + x, v3.y, v3.z);
	}
	/// <summary>
	/// Adjust Y value of Vector3 by the specified amount
	/// </summary>
	public static Vector3 AdjustY(this Vector3 v3, float y) {
		return new Vector3(v3.x, v3.y + y, v3.z);
	}
	/// <summary>
	/// Adjust Z value of Vector3 by the specified amount
	/// </summary>
	public static Vector3 AdjustZ(this Vector3 v3, float z) {
		return new Vector3(v3.x, v3.y, v3.z + z);
	}	
		
	public static Vector3 AdjustRelative(this Vector3 v3, Vector3 adjustBy, Quaternion relativeTo) {
		return v3 + (relativeTo * adjustBy);
	}
	public static Vector3 AdjustXRelative(this Vector3 v3, float adjustBy, Quaternion relativeTo) {
		return v3 + (relativeTo * new Vector3(adjustBy,0,0));
	}
	public static Vector3 AdjustYRelative(this Vector3 v3, float adjustBy, Quaternion relativeTo) {
		return v3 + (relativeTo * new Vector3(0,adjustBy,0));
	}
	public static Vector3 AdjustZRelative(this Vector3 v3, float adjustBy, Quaternion relativeTo) {
		return v3 + (relativeTo * new Vector3(0,0,adjustBy));
	}
	
	/// <summary>
	/// Turn Vector3(X,Y,Z) into Vector2(X,X)
	/// </summary>
	public static Vector2 MakeV2XX(this Vector3 v3) {
		return new Vector2(v3.x, v3.x);
	}
	/// <summary>
	/// Turn Vector3(X,Y,Z) into Vector2(X,Y)
	/// </summary>
	public static Vector2 MakeV2XY(this Vector3 v3) {
		return new Vector2(v3.x, v3.y);
	}
	/// <summary>
	/// Turn Vector3(X,Y,Z) into Vector2(X,Z)
	/// </summary>
	public static Vector2 MakeV2XZ(this Vector3 v3) {
		return new Vector2(v3.x, v3.z);
	}
	/// <summary>
	/// Turn Vector3(X,Y,Z) into Vector2(Y,X)
	/// </summary>
	public static Vector2 MakeV2YX(this Vector3 v3) {
		return new Vector2(v3.y, v3.x);
	}
	/// <summary>
	/// Turn Vector3(X,Y,Z) into Vector2(Y,Y)
	/// </summary>
	public static Vector2 MakeV2YY(this Vector3 v3) {
		return new Vector2(v3.y, v3.y);
	}
	/// <summary>
	/// Turn Vector3(X,Y,Z) into Vector2(Y,Z)
	/// </summary>
	public static Vector2 MakeV2YZ(this Vector3 v3) {
		return new Vector2(v3.y, v3.z);
	}
	/// <summary>
	/// Turn Vector3(X,Y,Z) into Vector2(Z,X)
	/// </summary>
	public static Vector2 MakeV2ZX(this Vector3 v3) {
		return new Vector2(v3.z, v3.x);
	}
	/// <summary>
	/// Turn Vector3(X,Y,Z) into Vector2(Z,Y)
	/// </summary>
	public static Vector2 MakeV2ZY(this Vector3 v3) {
		return new Vector2(v3.z, v3.y);
	}
	/// <summary>
	/// Turn Vector3(X,Y,Z) into Vector2(Z,Z)
	/// </summary>
	public static Vector2 MakeV2ZZ(this Vector3 v3) {
		return new Vector2(v3.z, v3.z);
	}

	/// <summary>
	/// Return the largest value in this Vector3
	/// </summary>
	public static float Max(this Vector3 v3) {
		float max = Mathf.Max(v3.x, v3.y);
		max = Mathf.Max(max, v3.z);
		return max;
	}
	/// <summary>
	/// Return the smallest value in this Vector3
	/// </summary>
	public static float Min(this Vector3 v3) {
		float min = Mathf.Min(v3.x, v3.y);
		min = Mathf.Min(min, v3.z);
		return min;
	}

	/// <summary>
	/// Return the center of the array of Vector3s
	/// </summary>
	public static Vector3 Center(this Vector3[] v3s)
	{
		Vector3 center = Vector3.zero;
		int nonNullCount = 0;
		foreach(Vector3 v3 in v3s)
		{
			if(v3 != null)
			{
				center += v3;
				nonNullCount++;
			}
		}
		
		if(nonNullCount == 0)
			return Vector3.zero;
		
		return center / nonNullCount;
	}
}
#endregion

#region Vector2

public static class Vector2Extensions {
	/// <summary>
	/// Change X value of Vector2 to the specified amount
	/// </summary>
	public static Vector2 WithX(this Vector2 v2, float x) {
		return new Vector2(x, v2.y);
	}
	/// <summary>
	/// Change Y value of Vector2 to the specified amount
	/// </summary>
	public static Vector2 WithY(this Vector2 v2, float y) {
		return new Vector2(v2.x, y);
	}
	
	/// <summary>
	/// Adjust X value of Vector2 by the specified amount
	/// </summary>
	public static Vector2 AdjustX(this Vector2 v2, float x) {
		return new Vector2(v2.x + x, v2.y);
	}
	/// <summary>
	/// Adjust Y value of Vector2 by the specified amount
	/// </summary>
	public static Vector2 AdjustY(this Vector2 v2, float y) {
		return new Vector2(v2.x, v2.y + y);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(X,X,0)
	/// </summary>
	public static Vector3 MakeV3XX0(this Vector2 v2) {
		return new Vector3(v2.x, v2.x, 0);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(X,Y,0)
	/// </summary>
	public static Vector3 MakeV3XY0(this Vector2 v2) {
		return new Vector3(v2.x, v2.y, 0);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(X,0,X)
	/// </summary>
	public static Vector3 MakeV3X0X(this Vector2 v2) {
		return new Vector3(v2.x, 0, v2.x);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(X,0,Y)
	/// </summary>
	public static Vector3 MakeV3X0Y(this Vector2 v2) {
		return new Vector3(v2.x, 0, v2.y);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(0,X,X)
	/// </summary>
	public static Vector3 MakeV30XX(this Vector2 v2) {
		return new Vector3(0, v2.x, v2.x);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(0,X,Y)
	/// </summary>
	public static Vector3 MakeV30XY(this Vector2 v2) {
		return new Vector3(0, v2.x, v2.y);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(Y,Y,0)
	/// </summary>
	public static Vector3 MakeV3YY0(this Vector2 v2) {
		return new Vector3(v2.y, v2.y, 0);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(Y,X,0)
	/// </summary>
	public static Vector3 MakeV3YX0(this Vector2 v2) {
		return new Vector3(v2.y, v2.x, 0);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(Y,0,Y)
	/// </summary>
	public static Vector3 MakeV3Y0Y(this Vector2 v2) {
		return new Vector3(v2.y, 0, v2.y);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(Y,0,X)
	/// </summary>
	public static Vector3 MakeV3Y0X(this Vector2 v2) {
		return new Vector3(v2.y, 0, v2.x);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(0,Y,Y)
	/// </summary>
	public static Vector3 MakeV30YY(this Vector2 v2) {
		return new Vector3(0, v2.y, v2.y);
	}
	/// <summary>
	/// Turn Vector2(X,Y) into Vector3(0,Y,X)
	/// </summary>
	public static Vector3 MakeV30YX(this Vector2 v2) {
		return new Vector3(0, v2.y, v2.x);
	}
}
#endregion

#region List
public static class ListExtensions 
{
	/// <summary>
	/// Shuffle the specified list.
	/// </summary>
	public static void Shuffle<T>(this List<T> list)
	{
		int n = list.Count;
		while(n > 0)
		{
			int k = UnityEngine.Random.Range(0, n);
			n--;
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static List<T> AddAndReturn<T>(this List<T> list, T item)
	{
		list.Add(item);
		return list;
	}

	/// <summary>
	/// Move an item from one index to another
	/// </summary>
	public static void MoveItemIndex<T>(this List<T> list, int from, int to)
	{
		if(list == null || list.Count == 0 || from < 0 || from >= list.Count || to < 0 || to >= list.Count)
			return;

		T itemAtFrom = list[from];
		list.RemoveAt(from);

		list.Insert(to, itemAtFrom);
	}
}
#endregion

#region BitMask
public class BitMaskAttribute : PropertyAttribute
{
	public System.Type propType;
	public BitMaskAttribute(System.Type aType)
	{
		propType = aType;
	}
}
#endregion

#region GameObject

public static class GameObjectExtensions {
	/// <summary>
	/// Make this gameObject a child object of parent
	/// </summary>
	public static void MakeChildOf(this GameObject gO, GameObject parent) {
		gO.transform.MakeChildOf(parent.transform, false);
	}
	/// <summary>
	/// Make this gameObject a child object of parent
	/// </summary>
	public static void MakeChildOf(this GameObject gO, Transform parent) {
		gO.transform.MakeChildOf(parent, false);
	}
	/// <summary>
	/// Make this gameObject a child object of parent (specify isStatic)
	/// </summary>
	public static void MakeChildOf(this GameObject gO, GameObject parent, bool isStatic) {
		gO.transform.MakeChildOf(parent.transform, isStatic);
	}
	/// <summary>
	/// Make this gameObject a child object of parent (specify isStatic)
	/// </summary>
	public static void MakeChildOf(this GameObject gO, Transform parent, bool isStatic) {
		gO.transform.MakeChildOf(parent, isStatic);
	}
	/// <summary>
	/// Make this gameObject a child object of parent and set its localPosition
	/// </summary>
	public static void MakeChildOf(this GameObject gO, GameObject parent, Vector3 localPosition) {
		gO.transform.MakeChildOf(parent.transform, localPosition, false);
	}
	/// <summary>
	/// Make this gameObject a child object of parent and set its localPosition
	/// </summary>
	public static void MakeChildOf(this GameObject gO, Transform parent, Vector3 localPosition) {
		gO.transform.MakeChildOf(parent, localPosition, false);
	}
	/// <summary>
	/// Make this gameObject a child object of parent and set its localPosition (specify isStatic)
	/// </summary>
	public static void MakeChildOf(this GameObject gO, GameObject parent, Vector3 localPosition, bool isStatic) {
		gO.transform.MakeChildOf(parent.transform, localPosition, isStatic);
	}
	/// <summary>
	/// Make this gameObject a child object of parent and set its localPosition (specify isStatic)
	/// </summary>
	public static void MakeChildOf(this GameObject gO, Transform parent, Vector3 localPosition, bool isStatic) {
		gO.transform.MakeChildOf(parent, localPosition, isStatic);
	}
	/// <summary>
	/// Make this gameObject a child object of parent and set its localRotation
	/// </summary>
	public static void MakeChildOf(this GameObject gO, GameObject parent, Quaternion localRotation) {
		gO.transform.MakeChildOf(parent.transform, localRotation, false);
	}
	/// <summary>
	/// Make this gameObject a child object of parent and set its localRotation
	/// </summary>
	public static void MakeChildOf(this GameObject gO, Transform parent, Quaternion localRotation) {
		gO.transform.MakeChildOf(parent, localRotation, false);
	}
	/// <summary>
	/// Make this gameObject a child object of parent and set its localRotation (specify isStatic)
	/// </summary>
	public static void MakeChildOf(this GameObject gO, GameObject parent, Quaternion localRotation, bool isStatic) {
		gO.transform.MakeChildOf(parent.transform, localRotation, isStatic);
	}
	/// <summary>
	/// Make this gameObject a child object of parent and set its localRotation (specify isStatic)
	/// </summary>
	public static void MakeChildOf(this GameObject gO, Transform parent, Quaternion localRotation, bool isStatic) {
		gO.transform.MakeChildOf(parent, localRotation, isStatic);
	}
	/// <summary>
	/// Make this gameObject a child object of parent and set its localPosition and localRotation
	/// </summary>
	public static void MakeChildOf(this GameObject gO, GameObject parent, Vector3 localPosition, Quaternion localRotation) {
		gO.transform.MakeChildOf(parent.transform, localPosition, localRotation, false);
	}
	/// <summary>
	/// Make this gameObject a child object of parent and set its localPosition and localRotation
	/// </summary>
	public static void MakeChildOf(this GameObject gO, Transform parent, Vector3 localPosition, Quaternion localRotation) {
		gO.transform.MakeChildOf(parent, localPosition, localRotation, false);
	}
	/// <summary>
	/// Make this gameObject a child object of parent and set its localPosition and localRotation (specify isStatic)
	/// </summary>
	public static void MakeChildOf(this GameObject gO, GameObject parent, Vector3 localPosition, Quaternion localRotation, bool isStatic) {
		gO.transform.MakeChildOf(parent.transform, localPosition, localRotation, isStatic);
	}
	/// <summary>
	/// Make this gameObject a child object of parent and set its localPosition and localRotation (specify isStatic)
	/// </summary>
	public static void MakeChildOf(this GameObject gO, Transform parent, Vector3 localPosition, Quaternion localRotation, bool isStatic) {
		gO.transform.MakeChildOf(parent, localPosition, localRotation, isStatic);
	}
	
	/// <summary>
	/// Get an array of this gameObject's children
	/// </summary>
	public static Transform[] GetChildren(this GameObject gO) {
		return gO.transform.GetChildren();
	}
	/// <summary>
	/// Get an array of this gameObject's child gameObjects
	/// </summary>
	public static GameObject[] GetChildGameObjects(this GameObject gO) {
		return gO.transform.GetChildGameObjects();
	}
	/// <summary>
	/// Gets the specified child
	/// </summary>
	public static Transform GetChild(this GameObject gO, string Name) {
		return gO.transform.GetChild(Name);
	}
    /// <summary>
    /// Gets the specified child gameObject
    /// </summary>
    public static GameObject GetChildGameObject(this GameObject gO, string Name)
    {
        Transform t = gO.transform.GetChild(Name);
        if (t == null)
            return null;
        return t.gameObject;
    }
}
#endregion

#region Transform
public static class TransformExtensions {
	/// <summary>
	/// Make this transform a child object of parent
	/// </summary>
	public static void MakeChildOf(this Transform t, GameObject parent) {
		MakeChildOf(t, parent.transform, false);
	}
	/// <summary>
	/// Make this transform a child object of parent
	/// </summary>
	public static void MakeChildOf(this Transform t, Transform parent) {
		MakeChildOf(t, parent, false);
	}
	/// <summary>
	/// Make this transform a child object of parent (specify isStatic)
	/// </summary>
	public static void MakeChildOf(this Transform t, GameObject parent, bool isStatic) {
		MakeChildOf(t, parent.transform, isStatic);
	}
	/// <summary>
	/// Make this transform a child object of parent (specify isStatic)
	/// </summary>
	public static void MakeChildOf(this Transform t, Transform parent, bool isStatic) {
		t.parent = parent;
		t.gameObject.isStatic = isStatic;
	}
	/// <summary>
	/// Make this transform a child object of parent and set its localPosition
	/// </summary>
	public static void MakeChildOf(this Transform t, GameObject parent, Vector3 localPosition) {
		MakeChildOf(t, parent.transform, localPosition, false);
	}
	/// <summary>
	/// Make this transform a child object of parent and set its localPosition
	/// </summary>
	public static void MakeChildOf(this Transform t, Transform parent, Vector3 localPosition) {
		MakeChildOf(t, parent, localPosition, false);
	}
	/// <summary>
	/// Make this transform a child object of parent and set its localPosition (specify isStatic)
	/// </summary>
	public static void MakeChildOf(this Transform t, GameObject parent, Vector3 localPosition, bool isStatic) {
		MakeChildOf(t, parent.transform, localPosition, isStatic);
	}
	/// <summary>
	/// Make this transform a child object of parent and set its localPosition (specify isStatic)
	/// </summary>
	public static void MakeChildOf(this Transform t, Transform parent, Vector3 localPosition, bool isStatic) {
		t.parent = parent;
		t.localPosition = localPosition;
		t.gameObject.isStatic = isStatic;
	}
	/// <summary>
	/// Make this transform a child object of parent and set its localRotation
	/// </summary>
	public static void MakeChildOf(this Transform t, GameObject parent, Quaternion localRotation) {
		MakeChildOf(t, parent.transform, localRotation, false);
	}
	/// <summary>
	/// Make this transform a child object of parent and set its localRotation
	/// </summary>
	public static void MakeChildOf(this Transform t, Transform parent, Quaternion localRotation) {
		MakeChildOf(t, parent, localRotation, false);
	}
	/// <summary>
	/// Make this transform a child object of parent and set its localRotation (specify isStatic)
	/// </summary>
	public static void MakeChildOf(this Transform t, GameObject parent, Quaternion localRotation, bool isStatic) {
		MakeChildOf(t, parent.transform, localRotation, isStatic);
	}
	/// <summary>
	/// Make this transform a child object of parent and set its localRotation (specify isStatic)
	/// </summary>
	public static void MakeChildOf(this Transform t, Transform parent, Quaternion localRotation, bool isStatic) {
		t.parent = parent;
		t.localRotation = localRotation;
		t.gameObject.isStatic = isStatic;
	}
	/// <summary>
	/// Make this transform a child object of parent and set its localPosition and localRotation
	/// </summary>
	public static void MakeChildOf(this Transform t, GameObject parent, Vector3 localPosition, Quaternion localRotation) {
		MakeChildOf(t, parent.transform, localPosition, localRotation, false);
	}
	/// <summary>
	/// Make this transform a child object of parent and set its localPosition and localRotation
	/// </summary>
	public static void MakeChildOf(this Transform t, Transform parent, Vector3 localPosition, Quaternion localRotation) {
		MakeChildOf(t, parent, localPosition, localRotation, false);
	}
	/// <summary>
	/// Make this transform a child object of parent and set its localPosition and localRotation (specify isStatic)
	/// </summary>
	public static void MakeChildOf(this Transform t, GameObject parent, Vector3 localPosition, Quaternion localRotation, bool isStatic) {
		MakeChildOf(t, parent.transform, localPosition, localRotation, isStatic);
	}
	/// <summary>
	/// Make this transform a child object of parent and set its localPosition and localRotation (specify isStatic)
	/// </summary>
	public static void MakeChildOf(this Transform t, Transform parent, Vector3 localPosition, Quaternion localRotation, bool isStatic) {
		t.parent = parent;
		t.localPosition = localPosition;
		t.localRotation = localRotation;
		t.gameObject.isStatic = isStatic;
	}


	/// <summary>
	/// Get an array of this transform's children
	/// </summary>
	public static Transform[] GetChildren(this Transform t) {
		Transform[] retArray = new Transform[t.childCount];
		for(int i=0; i<t.childCount; ++i)
			retArray[i] = t.GetChild(i);
		return retArray;
	}
	/// <summary>
	/// Get an array of this transform's child gameObjects
	/// </summary>
	public static GameObject[] GetChildGameObjects(this Transform t) {
		GameObject[] retArray = new GameObject[t.childCount];
		for(int i=0; i<t.childCount; ++i) {
			retArray[i] = t.GetChild(i).gameObject;
		}
		return retArray;
	}
	/// <summary>
	/// Gets the specified child
	/// </summary>
	public static Transform GetChild(this Transform t, string Name) {
		for(int i=0; i<t.childCount; ++i)
			if(t.GetChild(i).name == Name) return t.GetChild(i);
		return null;
	}


	public static void Copy(this Transform t, Transform other) {
		Transform parent = t.parent;
		t.parent = other.parent;

		t.position = other.position;
		t.localRotation = other.localRotation;
		t.localScale = other.localScale;

		t.parent = parent;
	}

	/// <summary>
	/// Returns the center position of the Transforms in the array
	/// </summary>
	public static Vector3 Center(this Transform[] ts)
	{
		Vector3 center = Vector3.zero;
		int nonNullCount = 0;
		foreach(Transform t in ts)
		{
			if(t != null)
			{
				center += t.position;
				nonNullCount++;
			}
		}

		if(nonNullCount == 0)
			return Vector3.zero;

		return center / nonNullCount;
	}
}
#endregion

#region Color
public static class ColorExtensions {
	public static Color WithR(this Color c, float r) {
		return new Color(r, c.g, c.b, c.a);
	}
    public static void SetR(this Color c, float r)
    {
        c = c.WithR(r);
    }
	public static Color WithG(this Color c, float g) {
		return new Color(c.r, g, c.b, c.a);
	}
    public static void SetG(this Color c, float g)
    {
        c = c.WithG(g);
    }
	public static Color WithB(this Color c, float b) {
		return new Color(c.r, c.g, b, c.a);
	}
    public static void SetB(this Color c, float b)
    {
        c = c.WithB(b);
    }
	public static Color WithA(this Color c, float a) {
		return new Color(c.r, c.g, c.b, a);
	}
    public static void SetA(this Color c, float a)
    {
        c = c.WithA(a);
    }
	public static Color MultR(this Color c, float m) {
		return new Color(c.r*m, c.g, c.b, c.a);
	}
	public static Color MultG(this Color c, float m) {
		return new Color(c.r, c.g*m, c.b, c.a);
	}
	public static Color MultB(this Color c, float m) {
		return new Color(c.r, c.g, c.b*m, c.a);
	}
	public static Color MultA(this Color c, float m) {
		return new Color(c.r, c.g, c.b, c.a*m);
	}
	public static Color MultRGB(this Color c, float m) {
		return new Color(c.r*m, c.g*m, c.b*m, c.a);
	}
	public static Color MultRGBA(this Color c, float m) {
		return new Color(c.r*m, c.g*m, c.b*m, c.a*m);
	}
	public static string ToHexStringRGBA(this Color color) {
		Color32 _color = color;
		return _color.r.ToString("X2") + _color.g.ToString("X2") + _color.b.ToString("X2") + _color.a.ToString("X2");
	}
	public static Color HexStringRGBA(string hex) {
		byte r, g, b, a;
		if(!byte.TryParse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber, null, out r))
		{
			Debug.LogError("Hex could not get r value");
			return Color.white;
		}
		if(!byte.TryParse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber, null, out g))
		{
			Debug.LogError("Hex could not get g value");
			return new Color(r, 255, 255, 255);
		}
		if(!byte.TryParse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber, null, out b))
		{
			Debug.LogError("Hex could not get b value");
			return new Color(r, g, 255, 255);
		}
		if(!byte.TryParse(hex.Substring(6,2), System.Globalization.NumberStyles.HexNumber, null, out a))
		{
			Debug.LogError("Hex could not get a value");
			return new Color(r, g, b, 255);
		}
		return new Color(r, g, b, a);
	}
	public static bool TryParseHexStringRGBA(string hex, out Color32 color) {
		byte r, g, b, a;
		if(!byte.TryParse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber, null, out r))
		{
			Debug.LogError("Hex could not get r value");
			color = Color.white;
			return false;
		}
		if(!byte.TryParse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber, null, out g))
		{
			Debug.LogError("Hex could not get g value");
			color = new Color(r, 255, 255, 255);
			return false;
		}
		if(!byte.TryParse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber, null, out b))
		{
			Debug.LogError("Hex could not get b value");
			color = new Color(r, g, 255, 255);
			return false;
		}
		if(!byte.TryParse(hex.Substring(6,2), System.Globalization.NumberStyles.HexNumber, null, out a))
		{
			Debug.LogError("Hex could not get a value");
			color = new Color(r, g, b, 255);
			return false;
		}
		color = new Color32(r, g, b, a);
		return true;
	}
}
#endregion

#region Array
public static class ArrayExtensions
{
	public static string[] Trim(this string[] array)
	{
		for(int i = 0; i < array.Length; i++)
			array[i] = array[i].Trim();
		return array;
	}

	public static bool ContainsAll(this string[] array, string[] comparison)
	{
		for(int i = 0; i < comparison.Length; i++)
			if(!array.Contains(comparison[i]))
				return false;
		return true;
	}
}
#endregion

#region KeyValuePair
public static class KeyValuePairExtensions
{
	public static KeyValuePair<object, object> Cast<K, V>(this KeyValuePair<K, V> kvp)
	{
		return new KeyValuePair<object, object>(kvp.Key, kvp.Value);
	}
	
	public static KeyValuePair<T, V> CastFrom<T, V>(System.Object obj)
	{
		return (KeyValuePair<T, V>) obj;
	}

	public static KeyValuePair<object, object> CastFrom(System.Object obj)
	{
		var type = obj.GetType();
		if(type.IsGenericType)
		{
			if(type == typeof(KeyValuePair<,>))
			{
				var key = type.GetProperty("Key");
				var value = type.GetProperty("Value");
				var keyObj = key.GetValue(obj, null);
				var valueObj = value.GetValue(obj, null);
				return new KeyValuePair<object, object>(keyObj, valueObj);
			}
		}
		throw new ArgumentException(" ### -> public static KeyValuePair< object, object > CastFrom(Object obj) " +
			": Error : obj argument must be KeyValuePair<,>");
	}
}
#endregion

#region Object
public static class ObjectExtensions
{
	public static IDictionary<string, object> ToDictionary(this object source)
	{
		return source.ToDictionary<object>();
	}

	public static IDictionary<string, T> ToDictionary<T>(this object source)
	{
		if(source == null)
			throw new ArgumentNullException();

		var dictionary = new Dictionary<string, T>();
		foreach(PropertyDescriptor property in TypeDescriptor.GetProperties(source))
			AddPropertyToDictionary<T>(property, source, dictionary);
		return dictionary;
	}

	private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, Dictionary<string, T> dictionary)
	{
		object value = property.GetValue(source);
		if(IsOfType<T>(value))
			dictionary.Add(property.Name, (T)value);
	}

	private static bool IsOfType<T>(object value)
	{
		return value is T;
	}

	private static void ThrowExceptionWhenSourceArgumentIsNull()
	{
		throw new ArgumentNullException("source", "Unable to convert object to a dictionary. The source object is null.");
	}
}
#endregion

#region String
public static class StringExtensions
{
	public static byte[] GetBytes(this string str)
	{
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		return bytes;
	}

	public static string GetString(this byte[] bytes)
	{
		string s = "";
		char[] chars = new char[sizeof(char)];
		for(int i=0; i<bytes.Length; i += sizeof(char))
		{
			System.Buffer.BlockCopy(bytes, i, chars, 0, sizeof(char));
			s += chars[0];
		}
		return s;
	}
}
#endregion


#region Task
/// <summary>
/// Task and TaskManager ported from Crucible with workaround for tasks persisting scene changes
/// </summary>
public class Task {
	public bool Running {
		get{return _task.Running;}
	}
	public bool Paused {
		get{return _task.Paused;}
	}
	
	public delegate void FinishedHandler(bool manual);
	public event FinishedHandler Finished;
	TaskManager.TaskState _task;
	
	public Task(IEnumerator coroutine, FinishedHandler onFinish, bool ignoreSceneChange, bool autoStart) {
		Finished += onFinish;
		_task = TaskManager.CreateTask(coroutine, ignoreSceneChange);
		_task.Finished += TaskFinished;
		if(autoStart) Start();
	}
	public Task(IEnumerator coroutine, System.Action onFinish, bool ignoreSceneChange, bool autoStart) : this(coroutine, (unused)=>onFinish(), ignoreSceneChange, autoStart){}
	public Task(IEnumerator coroutine, bool ignoreSceneChange, bool autoStart) : this(coroutine, (FinishedHandler)null, ignoreSceneChange, autoStart){}
	public Task(IEnumerator coroutine, bool ignoreSceneChange) : this(coroutine, (FinishedHandler)null, ignoreSceneChange, true){}
	
	public Task(IEnumerator coroutine, FinishedHandler onFinish) : this(coroutine, onFinish, false, true){}
	public Task(IEnumerator coroutine, System.Action onFinish) : this(coroutine, (unused)=>onFinish(), false, true){}
	public Task(IEnumerator coroutine) : this(coroutine, false, true){}
	
	public void Start() {
		_task.Start();
	}
	public void Stop() {
		_task.Stop();
	}
	public void Pause() {
		_task.Pause();
	}
	public void Unpause() {
		_task.Unpause();
	}
	
	void TaskFinished(bool manual) {
		FinishedHandler handler = Finished;
		if(handler != null) handler(manual);
	}
	
	public static Task DeferredCall(float delay, bool realTime, bool fixedTime, System.Action action) {
		return new Task(deferCall(delay, realTime, fixedTime, action));
	}
	public static Task DeferredCall(float delay, bool realTime, System.Action action) {
		return DeferredCall(delay, realTime, false, action);
	}
	public static Task DeferredCall(float delay, System.Action action) {
		return DeferredCall(delay, false, false, action);
	}
	static IEnumerator deferCall(float delay, bool realTime, bool fixedTime, System.Action action) {
		if(realTime) {
			yield return new WaitForSeconds(delay);
		} else {
			float elapsedTime = 0f;
			if(fixedTime) {
				while(elapsedTime < delay) {
					elapsedTime += Time.fixedDeltaTime;
					yield return new WaitForFixedUpdate();
				}
			} else {
				while(elapsedTime < delay) {
					elapsedTime += Time.deltaTime;
					yield return null;
				}
			}
		}
		action();
	}
	
	class TaskManager : MonoBehaviour {
		public class TaskState {
			public bool Running {
				get{return _running;}
			}
			public bool Paused {
				get{return _paused;}
			}
			public delegate void FinishedHandler(bool manual);
			public event FinishedHandler Finished;
			
			IEnumerator _coroutine;
			bool _running, _paused, _stopped, _ignoreSceneChange;
			
			public TaskState(IEnumerator coroutine) {
				new TaskState(coroutine, false);
			}
			public TaskState(IEnumerator coroutine, bool ignoreSceneChange) {
				_coroutine = coroutine;
				_ignoreSceneChange = ignoreSceneChange;
			}
			
			public void Pause() {
				_paused = true;
			}
			public void Unpause() {
				_paused = false;
			}
			
			public void Start() {
				_running = true;
				if(_ignoreSceneChange) _immortalSingleton.StartCoroutine(CallWrapper());
				else _singleton.StartCoroutine(CallWrapper());
			}
			public void Stop() {
				_stopped = true;
				_running = false;
			}
			
			IEnumerator CallWrapper() {
				yield return null;
				IEnumerator e = _coroutine;
				while(_running) {
					if(_paused) {
						yield return null;
					} else {
						if(e != null && e.MoveNext()) {
							yield return e.Current;
						} else {
							_running = false;
						}
					}
				}
				FinishedHandler handler = Finished;
				if(handler != null) handler(_stopped);
			}
		}
		
		static TaskManager _singleton, _immortalSingleton;
		
		public static TaskState CreateTask(IEnumerator coroutine) {
			return CreateTask(coroutine, false);
		}
		public static TaskState CreateTask(IEnumerator coroutine, bool ignoreSceneChange) {
			if(!ignoreSceneChange && _singleton == null) {
				GameObject gO = new GameObject("TaskManager");
				_singleton = gO.AddComponent<TaskManager>();
			} else if(ignoreSceneChange && _immortalSingleton == null) {
				GameObject gO = new GameObject("ImmortalTaskManager");
				DontDestroyOnLoad(gO);
				_immortalSingleton = gO.AddComponent<TaskManager>();
			}
			return new TaskState(coroutine, ignoreSceneChange);
		}
	}
}
#endregion