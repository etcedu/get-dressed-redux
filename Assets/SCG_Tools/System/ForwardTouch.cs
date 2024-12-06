using UnityEngine;
using System.Collections;

public delegate void FT_Over(bool isOver, GameObject target);
public delegate void FT_Select(bool selected, GameObject target);
public delegate void FT_Target(GameObject target);
public delegate void FT_Drag(Vector2 delta, GameObject target);
public delegate void FT_Drag_Other(GameObject draggedObject, GameObject target);

/// <summary>
/// Forwards any of the touch functions that occur for this gameObject
/// </summary>
public class ForwardTouch : MonoBehaviour
{
	public GameObject target {
		set{_target = value;}
	}
	private GameObject _target;
    public FT_Over Hovered;
    public FT_Over Pressed;
	public FT_Select Selected;
    public FT_Target Clicked;
    public FT_Target DoubleClicked;
    public FT_Target DragStarted;
	public FT_Drag Dragged;
    public FT_Drag_Other DraggedOver;
    public FT_Drag_Other DraggedOut;
    public FT_Target DragEnded;

	void Awake() {
		if(GetComponent<Collider>() == null && GetComponent<Collider2D>() == null) {
			Debug.LogWarning("No collider attached to "+gameObject.name+". Cannot forward touches.");
			enabled = false;
		}
		_target = gameObject;
	}

	public void CodeHover(bool isOver)
	{
		OnHover(isOver);
	}
	void OnHover(bool isOver) {
		if(Hovered != null) Hovered(isOver, _target);
	}

	public void CodePress(bool isDown)
	{
		OnPress(isDown);
	}
	void OnPress(bool isDown) {
		if(Pressed != null) Pressed(isDown, _target);
	}

	public void CodeSelect(bool selected)
	{
		OnSelect(selected);
	}
	void OnSelect(bool selected) {
		if(Selected != null) Selected(selected, _target);
	}

	public void CodeClick()
	{
		OnClick();
	}
	void OnClick() {
		if(Clicked != null) Clicked(_target);
	}

	public void CodeDoubleClick()
	{
		OnDoubleClick();
	}
	void OnDoubleClick() {
		if(DoubleClicked != null) DoubleClicked(_target);
	}

	public void CodeDragStart()
	{
		OnDragStart();
	}
	void OnDragStart() {
		if(DragStarted != null) DragStarted(_target);
	}

	public void CodeDrag(Vector2 delta)
	{
		OnDrag(delta);
	}
	void OnDrag(Vector2 delta) {
		if(Dragged != null) Dragged(delta, _target);
	}

	public void CodeDragOver(GameObject draggedObject)
	{
		OnDragOver(draggedObject);
	}
	void OnDragOver(GameObject draggedObject) {
		if(DraggedOver != null) DraggedOver(draggedObject, _target);
	}

	public void CodeDragOut(GameObject draggedObject)
	{
		OnDragOut(draggedObject);
	}
	void OnDragOut(GameObject draggedObject) {
		if(DraggedOut != null) DraggedOut(draggedObject, _target);
	}

	public void CodeDragEnd()
	{
		OnDragEnd();
	}
	void OnDragEnd() {
		if(DragEnded != null) DragEnded(_target);
	}
}