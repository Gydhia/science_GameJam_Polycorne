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
                serializedObject.Update();
                this._target.SnapTrack();
                serializedObject.ApplyModifiedProperties();
                serializedObject.SetIsDifferentCacheDirty();
            }

            base.OnInspectorGUI();
        }
    }

}
