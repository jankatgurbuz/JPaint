#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class JPaintEditor : EditorWindow
{
    private const string jPaintName = "JPaint";
    private const string jCanvasCamera = "JCamera";
    private const string jCanvasPlane = "JPlane";

    [MenuItem("JLibrary/JPaint")]
    public static void Paint()
    {
        if (FindJPaint == null)
        {
            GameObject main = CreateObj(jPaintName);

            GameObject camera = CreateObj(jCanvasCamera);
            Camera c = camera.AddComponent<Camera>();
            c.clearFlags = CameraClearFlags.Depth;
            c.orthographic = true;
            c.farClipPlane = 5;
            c.orthographicSize = 0.5f;
            SetParent(main,camera);

            GameObject plane = CreateObjPrimitive(jCanvasPlane, PrimitiveType.Quad);
            plane.transform.position = new Vector3(0, 0, 5);
            SetParent(main, plane);

        }

    }
    private static GameObject CreateObj(string objName) => new GameObject(objName);
    public static void SetParent(GameObject parent, GameObject obj) => obj.transform.SetParent(parent.transform);
    private static GameObject FindJPaint { get { return GameObject.Find(jPaintName); } }
    private static GameObject CreateObjPrimitive(string objName, PrimitiveType p)
    {
        GameObject primitive = GameObject.CreatePrimitive(p);
        primitive.name = objName;
        return primitive;
    }
   

    



}
#endif