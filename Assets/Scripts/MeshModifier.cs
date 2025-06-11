using UnityEngine;

[ExecuteInEditMode]
public class MeshModifier : MonoBehaviour
{
    public bool _destroy;

    private Mesh mesh;
    [SerializeField] Vector3[] verts;
    [SerializeField] GameObject[] handles;

    private Vector3 vertPos;
    private const string TAG_HANDLE = "editMesh";


    [ContextMenu("Create Handles")]
    void CreateHandles()
    {
        if (handles != null && handles.Length > 0)
        {
            for (int i = handles.Length-1; i >= 0; i--)
            {
                DestroyImmediate(handles[i]);
            }
        }

        handles = new GameObject[verts.Length];
        for (int i = 0; i < verts.Length; i++)
        {
            vertPos = transform.TransformPoint(verts[i]);
            GameObject handle = new GameObject(TAG_HANDLE);
            handle.transform.position = vertPos;
            handle.transform.parent = transform;
            handle.tag = TAG_HANDLE;
            handle.name = $"Handle{i}";
            //handle.AddComponent<EditMeshGizmo>()._parent = this;
            handles[i] = handle;
        }
    }

    private void OnEnable()
    {
        mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        verts = mesh.vertices;      
    }

    // Update is called once per frame
    void Update()
    {

        if (_destroy)
        {
            _destroy = false;
            DestroyImmediate(this);
            return;
        }

        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = handles[i].transform.localPosition;
        }

        mesh.vertices = verts;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
