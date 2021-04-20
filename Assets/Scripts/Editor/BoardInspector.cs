using Assets.Scripts;
using UnityEditor;
using UnityEngine;

namespace ScienceGameJam.UnityEditor
{
   
    [CustomEditor(typeof(Board))]
    public class BoardInspector : Editor
    {
        Board _target;

        public void OnEnable()
        {
            if(base.target != null)
                this._target = (Board)base.target;
        }
        

        public override void OnInspectorGUI()
        {
            if (GUI.Button(GUILayoutUtility.GetRect(0, int.MaxValue, 20, 20), "Collect all boxes"))
            {
                this._target.CollectAllBoxes();
            }
            if (GUI.Button(GUILayoutUtility.GetRect(0, int.MaxValue, 20, 20), "Collect all cards"))
            {
                this._target.CollectAllCards();
            }

            base.OnInspectorGUI();
        }
    }

}
