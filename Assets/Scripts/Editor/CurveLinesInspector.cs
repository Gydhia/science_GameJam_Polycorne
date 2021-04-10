using Assets.Scripts;
using UnityEditor;
using UnityEngine;

namespace ScienceGameJam.UnityEditor
{

    [CustomEditor(typeof(Track))]
    public class CurveLinesInspector : Editor
    {
        Track target;

        public void OnEnable()
        {
            if (base.target != null)
                this.target = (Track)base.target;
        }


        public override void OnInspectorGUI()
        {
            if (GUI.Button(GUILayoutUtility.GetRect(0, int.MaxValue, 20, 20), "Curve Lines")) {
                this.target.CurveLineWithBezier();
            }

            base.OnInspectorGUI();
        }
    }

}
