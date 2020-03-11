using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JLibrary.Paint;
public class PaintController : MonoBehaviour
{
    private JPaint _jpaint;
    private void Start()
    {
        _jpaint = GameObject.Find("JPaint").GetComponent<JPaint>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.DrawLine(ray.origin, hit.point);

                hit.collider.GetComponent<Renderer>().material.mainTexture = _jpaint.RendererTexture;
                _jpaint.Paint(hit, 0,Color.blue);
            }
        }

    }
}
