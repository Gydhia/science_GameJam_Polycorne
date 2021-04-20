using Assets.Scripts;
using UnityEditor;
using UnityEngine;

namespace ScienceGameJam.UnityEditor
{
   
    [CustomEditor(typeof(Hand))]
    public class HandInspector : Editor
    {
        Hand _target;

        public void OnEnable()
        {
            if(base.target != null)
                this._target = (Hand)base.target;
        }
        

        public override void OnInspectorGUI()
        {
            if (GUI.Button(GUILayoutUtility.GetRect(0, int.MaxValue, 20, 20), "Snap track"))
            {
                this._target.SnapTrack();
            }

            base.OnInspectorGUI();
        }

        public void OnSceneGUI()
        {
            if(Event.current.type == EventType.Repaint)
            {
                Handles.color = Color.magenta;
                Handles.SphereHandleCap(0, this._target.transform.position, this._target.transform.rotation, 10f, EventType.Repaint);
            }
        }
    }

}
