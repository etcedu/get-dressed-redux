using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
[RequireComponent (typeof(PolygonCollider2D))]
public class PolyCollider2DAdjuster : MonoBehaviour
{
	[SerializeField]
	List<GameObjectOffset> points = new List<GameObjectOffset>();
	[HideInInspector]
	[SerializeField]
	PolygonCollider2D poly2D;

	
#if UNITY_EDITOR
	void OnEnable()
	{
		if(poly2D == null)
		{
			poly2D = GetComponent<PolygonCollider2D>();
			foreach(Vector2 point in poly2D.points)
				points.Add(new GameObjectOffset(transform, point));
		}
	}
#endif
	
	// Update is called once per frame
	void LateUpdate()
	{
#if UNITY_EDITOR
		if(!Application.isPlaying)
		{
			if(poly2D.points.Length < points.Count)
			{
				foreach(GameObjectOffset point in points)
				{
					if(!poly2D.points.Contains(point.Key))
						points.Remove(point);
				}
			} else if(poly2D.points.Length > points.Count)
			{
				bool addedPoint = false;
				for(int i = 0; i < poly2D.points.Length-1; i++)
				{
					if(points[i].Key != poly2D.points[i])
					{
						points.Insert(i, new GameObjectOffset(transform, poly2D.points[i]));
						addedPoint = true;
						break;
					}
				} if(!addedPoint)
					points.Add(new GameObjectOffset(transform, poly2D.points.Last()));
			}

			for(int i = 0; i < points.Count; i++)
				poly2D.points[i] = points[i].EditorUpdate(poly2D.points[i]);
		}
#endif
		poly2D.SetPath(0, UpdatedPoints());
	}

	Vector2[] UpdatedPoints()
	{
		Vector2[] pts = new Vector2[points.Count];
		for(int i = 0; i < points.Count; i++)
			pts[i] = points[i].ActivePosition;
		return pts;
	}

	[System.Serializable]
	class GameObjectOffset
	{
		public GameObject target;
		public Vector2 offset;
		[HideInInspector]
		[SerializeField]
		Transform _holder;

		public Vector2 ActivePosition
		{
			get{ return (Vector2)_holder.InverseTransformPoint(target.transform.position) + offset; }
		}

#if UNITY_EDITOR
		[HideInInspector]
		[SerializeField]
		GameObject _oldTarget;
		[HideInInspector]
		[SerializeField]
		Vector2 _oldOffset;
		[HideInInspector]
		[SerializeField]
		Vector2 _pointKey;

		public GameObjectOffset(Transform holder, Vector2 key)
		{
			target = holder.gameObject;
			offset = key;
			_holder = holder;

			_oldTarget = _holder.gameObject;
			_oldOffset = key;
			_pointKey = key;
		}

		public Vector2 EditorUpdate(Vector2 key)
		{
			if(target != _oldTarget)
			{
				if(target == null)
					target = _holder.gameObject;

				offset += (Vector2)_holder.InverseTransformPoint(_oldTarget.transform.position) - (Vector2)_holder.InverseTransformPoint(target.transform.position);
						
				_oldTarget = target;
				key = (Vector2)_holder.InverseTransformPoint(target.transform.position) + offset;
			}
			if(offset != _oldOffset)
			{
				key += offset - _oldOffset;
				Debug.Log(offset - _oldOffset);
				_oldOffset = offset;
			}

			_pointKey = key;
			return key;
		}

		public Vector2 Key
		{
			get{ return _pointKey; }
		}
#endif
	}
}

