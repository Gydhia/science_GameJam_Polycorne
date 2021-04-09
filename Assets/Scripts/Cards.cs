using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cards : MonoBehaviour
{
    public void connectLine(Vector3 start, Vector3 end)
    {
        GameObject line = new GameObject();
        line.transform.position = start;
        line.AddComponent<LineRenderer>();

        LineRenderer line_renderer = line.GetComponent<LineRenderer>();

        line_renderer.SetPosition(0, start);
        line_renderer.SetPosition(1, end);
    }
}
