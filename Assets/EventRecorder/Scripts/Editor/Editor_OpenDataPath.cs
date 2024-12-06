using UnityEditor;
using UnityEngine;

namespace SimcoachGames
{
    public class Editor_OpenDataPath : MonoBehaviour
    {
        [MenuItem("File/Open Persistent Data Path")]
        public static void OpenPersistentDataPath()
        {
            Application.OpenURL(Application.persistentDataPath);
        }
    }
}
