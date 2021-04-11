﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class InputController : MonoBehaviour
    {
        public Camera Camera;
        public float ScrollSpeed;

        public delegate void MouseClick();
        public event MouseClick OnMouseButtonDown;

        public static InputController Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }


        void Update()
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                Camera.transform.Translate(new Vector3(this.ScrollSpeed * Time.deltaTime, 0, 0));
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Camera.transform.Translate(new Vector3(-this.ScrollSpeed * Time.deltaTime, 0, 0));
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                Camera.transform.Translate(new Vector3(0, -this.ScrollSpeed * Time.deltaTime, 0));
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                Camera.transform.Translate(new Vector3(0, this.ScrollSpeed * Time.deltaTime, 0));
            }
            if (Input.GetMouseButtonDown(0))
            {
                FireMouseClick();
            }
        }

        public void FireMouseClick()
        {
            if(OnMouseButtonDown != null) {
                OnMouseButtonDown.Invoke();
            }
        }
    }
}
