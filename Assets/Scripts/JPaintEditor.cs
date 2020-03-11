#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class JPaintEditor : EditorWindow
{
    private const string _jPaintName = "JPaint";
    private const string _jCanvasCamera = "JCamera";
    private const string _jCanvasPlane = "JPlane";
    private const string _jPaintObj = "JPaintObj";

    private static RenderTexture _renderTexture = null;

    [MenuItem("JLibrary/JPaint")]
    public static void Paint()
    {
        if (FindJPaint == null)
        {
            GameObject main = CreateObj(_jPaintName);
            main.AddComponent<JLibrary.Paint.JPaint>();

            GameObject camera = CreateObj(_jCanvasCamera);
            SetCamera(camera);
            SetParent(main, camera);

            GameObject plane = CreateObjPrimitive(_jCanvasPlane, PrimitiveType.Quad);
            plane.transform.position = new Vector3(0, 0, 5);
            SetParent(main, plane);

            GameObject paintObjs = CreateObj(_jPaintObj);
            SetParent(main, paintObjs);

        }

    }
    private static GameObject CreateObj(string objName) => new GameObject(objName);
    public static void SetParent(GameObject parent, GameObject obj) => obj.transform.SetParent(parent.transform);
    private static GameObject FindJPaint { get { return GameObject.Find(_jPaintName); } }
    private static GameObject CreateObjPrimitive(string objName, PrimitiveType p)
    {
        GameObject primitive = GameObject.CreatePrimitive(p);
        primitive.name = objName;
        return primitive;
    }
    private static void SetCamera(GameObject camera) 
    {
        Camera c = camera.AddComponent<Camera>();
        c.clearFlags = CameraClearFlags.Depth;
        c.orthographic = true;
        c.farClipPlane = 5;
        c.orthographicSize = 0.5f;
        c.targetTexture = RenderTexture();
    }
    private static RenderTexture RenderTexture()
    {
        if (_renderTexture==null)
        {
            _renderTexture = new RenderTexture(1024, 1024, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            _renderTexture.Create();
        }
        return _renderTexture;
    }
}
#endif