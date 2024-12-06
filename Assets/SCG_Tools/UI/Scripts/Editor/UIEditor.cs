using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

public class UIEditor : Editor {
	[MenuItem ("GameObject/UI/HUD Camera")]
	private static void CreateHUDCamera() {
		// Get layer to set Camera to
		int uiLayer = LayerMask.NameToLayer("UI");

		// Create camera, position it in the scene, and set it to the UI layer
		GameObject uiCameraObj = new GameObject("HUD Camera", new System.Type[]{typeof(Camera)});
		uiCameraObj.transform.position = SceneViewSpawnPoint();
		uiCameraObj.layer = uiLayer;

		// Modify the camera settings for HUD
		Camera uiCamera = uiCameraObj.GetComponent<Camera>();
		uiCamera.orthographic = true;
		uiCamera.clearFlags = CameraClearFlags.Depth;
		uiCamera.nearClipPlane = -1;
		uiCamera.farClipPlane = 1;
		uiCamera.orthographicSize = 1;
		if(Camera.main != null)
		   uiCamera.depth = Camera.main.depth + 1;
		else
			uiCamera.depth = 0;
		uiCamera.cullingMask = (1 << uiLayer);

		// Create HUD Canvas inside HUD Camera
		GameObject uiCanvasObj = new GameObject("HUD Canvas", new System.Type[]{typeof(Canvas), typeof(GraphicRaycaster)});
		uiCanvasObj.transform.SetParent(uiCameraObj.transform);
		uiCanvasObj.layer = uiLayer;

		Canvas uiCanvas = uiCanvasObj.GetComponent<Canvas>();
		uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
		uiCanvas.worldCamera = uiCamera;
		uiCanvas.planeDistance = 0;

		Selection.activeObject = uiCanvas;
	}



	public static Vector3 SceneViewSpawnPoint() {
		if(SceneView.currentDrawingSceneView == null) {
			return Vector3.zero;
		}

		Selection.activeObject = SceneView.currentDrawingSceneView;
		Camera sceneCam = SceneView.currentDrawingSceneView.camera;
		Vector3 spawnPos;

		if(SceneView.currentDrawingSceneView.in2DMode)
			spawnPos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, -sceneCam.transform.position.z));
		else {		
			float checkDist = Vector3.Distance(SceneView.currentDrawingSceneView.pivot, sceneCam.transform.position);
			RaycastHit rayInfo = new RaycastHit();
			if( Physics.Raycast(sceneCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out rayInfo, checkDist))
				spawnPos = rayInfo.point;
			   else
				spawnPos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, checkDist));
		}

		return spawnPos;
	}
}
