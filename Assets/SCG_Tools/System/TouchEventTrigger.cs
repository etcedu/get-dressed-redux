using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Executes events when touch actions are performed
/// </summary>
public class TouchEventTrigger : MonoBehaviour
{
	public GameObject target;
	public List<EventDelegate> HoveredEvents;
	public List<EventDelegate> PressedEvents;
	public List<EventDelegate> SelectedEvents;
	public List<EventDelegate> ClickedEvents;
	public List<EventDelegate> DoubleClickedEvents;
	public List<EventDelegate> DragStartedEvents;
	public List<EventDelegate> DraggedEvents;
	public List<EventDelegate> DraggedOverEvents;
	public List<EventDelegate> DraggedOutEvents;
	public List<EventDelegate> DragEndedEvents;
	
	void Awake() {
		if(GetComponent<Collider>() == null && GetComponent<Collider2D>() == null) {
			Debug.LogWarning("No collider attached to "+gameObject.name+". Cannot detect touches.");
			enabled = false;
		}
		if(target == null)
			target = gameObject;
	}
	
	public void CodeHover(bool isOver)
	{
		OnHover(isOver);
	}
	void OnHover(bool isOver) {
		if(HoveredEvents != null) EventDelegate.Execute(HoveredEvents);
	}
	
	public void CodePress(bool isDown)
	{
		OnPress(isDown);
	}
	void OnPress(bool isDown) {
		if(PressedEvents != null) EventDelegate.Execute(PressedEvents);
	}
	
	public void CodeSelect(bool selected)
	{
		OnSelect(selected);
	}
	void OnSelect(bool selected) {
		if(SelectedEvents != null) EventDelegate.Execute(SelectedEvents);
	}
	
	public void CodeClick()
	{
		OnClick();
	}
	void OnClick() {
		Debug.Log("here");
		if(ClickedEvents != null) EventDelegate.Execute(ClickedEvents);
	}
	
	public void CodeDoubleClick()
	{
		OnDoubleClick();
	}
	void OnDoubleClick() {
		if(DoubleClickedEvents != null) EventDelegate.Execute(DoubleClickedEvents);
	}
	
	public void CodeDragStart()
	{
		OnDragStart();
	}
	void OnDragStart() {
		if(DragStartedEvents != null) EventDelegate.Execute(DragStartedEvents);
	}
	
	public void CodeDrag(Vector2 delta)
	{
		OnDrag(delta);
	}
	void OnDrag(Vector2 delta) {
		if(DraggedEvents != null) EventDelegate.Execute(DraggedEvents);
	}
	
	public void CodeDragOver(GameObject draggedObject)
	{
		OnDragOver(draggedObject);
	}
	void OnDragOver(GameObject draggedObject) {
		if(DraggedOverEvents != null) EventDelegate.Execute(DraggedOverEvents);
	}
	
	public void CodeDragOut(GameObject draggedObject)
	{
		OnDragOut(draggedObject);
	}
	void OnDragOut(GameObject draggedObject) {
		if(DraggedOutEvents != null) EventDelegate.Execute(DraggedOutEvents);
	}
	
	public void CodeDragEnd()
	{
		OnDragEnd();
	}
	void OnDragEnd() {
		if(DragEndedEvents != null) EventDelegate.Execute(DragEndedEvents);
	}
}