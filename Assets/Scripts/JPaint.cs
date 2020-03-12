using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JLibrary.Paint
{
    public class JPaint : MonoBehaviour
    {
        [SerializeField] private Vector2Int _textureSize;
        [SerializeField] private SpriteRenderer[] _brushList;
        [SerializeField] private string[] _paintTag = null;

        public RenderTexture RendererTexture { get; private set; }

        private Transform _jCamera, _jPlane, _jPaintObjs;
        private Camera _camera;
        private PaintPooling _paintPooling;
        private Material _paintMaterial;

        private const string _paintMaterialName = "PaintMaterial";
        private const int _totalPaintObj = 1000;
        private void Awake()
        {
            Initialize();
            CreateRendererTexture();
            CreateMaterial();
            CreatePaintPooling();
        }

        private void Initialize()
        {
            _paintPooling = new PaintPooling();

            _jCamera = FindChild("JCamera");
            _jPlane = FindChild("JPlane");
            _jPaintObjs = FindChild("JPaintObj");

            _camera = _jCamera.GetComponent<Camera>();
        }
        private Transform FindChild(string childName) => transform.Find(childName);

        private void CreateRendererTexture()
        {
            int depth = 16;

            SizeControl();
            RendererTexture = new RenderTexture(_textureSize.x, _textureSize.y, depth,
                RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);

            RendererTexture.Create();
        }
        private void SizeControl()
        {
            int x = _textureSize.x <= 0 ? 10 : _textureSize.x;
            int y = _textureSize.y <= 0 ? 10 : _textureSize.y;
            _textureSize.Set(x, y);
        }
        private void CreateMaterial()
        {
            if (_paintMaterial == null)
            {
                _paintMaterial = new Material(Shader.Find("Unlit/Texture"));
                _paintMaterial.name = _paintMaterialName;
            }

            _camera.targetTexture = RendererTexture;
            _jPlane.GetComponent<Renderer>().material = _paintMaterial;
        }
        private void CreatePaintPooling()
        {
            foreach (var item in _brushList)
                if (item == null)
                    Debug.LogError("No SpriteRenderer Added");

            for (int i = 0; i < _totalPaintObj; i++)
            {
                int tempBrush = 0;
                Transform paintobj = CreatePaintObj(_brushList[tempBrush].gameObject);
                paintobj.SetParent(_jPaintObjs);
                paintobj.name = i.ToString();
                _paintPooling.Add(paintobj);
            }
        }
        public Transform CreatePaintObj(GameObject obj) => Instantiate(obj, Vector3.zero, Quaternion.identity).transform;

        public void Paint(RaycastHit hit, int whichBrush,Color whichColor,float brushSize)
        {
            if (hit.collider != null && TagControl(hit))
            {
                if (FindUVPosition(hit, out Vector3 pos))
                {
                 
                    (PaintPooling.Pooling p, bool saveControl) = _paintPooling.BringGameObj();
                    SetBrush(p.ObjSpriteRenderer, whichBrush);
                    SetColor(p.ObjSpriteRenderer, whichColor);
                    SetBrushSize(p.PointObj, brushSize);
                    SetPaintPos(p.PointObj.transform, pos);
                    if (saveControl)
                        StartCoroutine(SaveTexture());
                }
            }
        }

        private void SetBrushSize(GameObject obj, float size)
        {
            size = Mathf.Clamp(size,0,float.MaxValue);
            obj.transform.localScale = new Vector3(size, size, 1);
        }

        private void SetColor(SpriteRenderer spr, Color whichColor) => spr.color = whichColor;
        private void SetBrush(SpriteRenderer spr, int whichBrush)
        {
            if (whichBrush > _brushList.Length)
                spr.sprite = _brushList[0].sprite;
            else
                spr.sprite = _brushList[whichBrush].sprite;
        }
        private bool TagControl(RaycastHit hit)
        {
            foreach (var item in _paintTag)
                if (hit.collider.tag == item)
                    return true;

            return false;
        }
        private bool FindUVPosition(RaycastHit hit, out Vector3 pos)
        {
            MeshCollider mc = hit.collider as MeshCollider;

            if (mc == null || mc.sharedMesh == null)
            {
                pos = Vector3.zero;
                return false;
            }

            Vector2 pixel = hit.textureCoord;
            pos = new Vector3(pixel.x - _camera.orthographicSize, pixel.y - _camera.orthographicSize, 0);
            return true;
        }
        private void SetPaintPos(Transform obj, Vector3 vec)
        {
            obj.localPosition = vec;
        }
        public IEnumerator SaveTexture()
        {
            RenderTexture rt = RendererTexture;
            RenderTexture.active = rt;

            Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();

            // RenderTexture.active = null;
            _paintMaterial.mainTexture = tex;

            _paintPooling.DisableSprite();
            yield return null;
        }
    }
}

