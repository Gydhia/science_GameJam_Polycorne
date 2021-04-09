using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameCamera : MonoBehaviour
{

    private Canvas GameCanvas;
    private Camera Camera;

    public void Start() {
        this.Camera = Camera.main;
        this.GameCanvas = FindObjectOfType<Canvas>();
    }
    public void Update()
    {
        this.Camera.orthographicSize = this.GameCanvas.transform.position.y;

    }
}