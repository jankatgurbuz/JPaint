using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JLibrary.Paint
{
    public class PaintPooling
    {
        public class Pooling
        {
            public GameObject PointObj;
            public SpriteRenderer ObjSpriteRenderer;
        }

        private int _counter = 0;
        private List<Pooling> _pooling;

        public PaintPooling()
        {
            _pooling = new List<Pooling>();
        }
        public void Add(Transform obj)
        {
            SpriteRenderer spr = obj.GetComponent<SpriteRenderer>();
            spr.enabled = false;
            Pooling p = new Pooling { PointObj = obj.gameObject, ObjSpriteRenderer = spr };
            _pooling.Add(p);
        }
        public (Pooling,bool) BringGameObj()
        {
            Pooling obj = _pooling[_counter++];
            obj.ObjSpriteRenderer.enabled = true;
            return (obj,CheckCounter());
        }
        private bool CheckCounter() 
        {
            if (_counter == _pooling.Count)
            {
                _counter = 0;
                return true;
            } 
            return false;
        }

        public void DisableSprite() 
        {
            foreach (var item in _pooling)
                item.ObjSpriteRenderer.enabled = false;
        }
    }
}

