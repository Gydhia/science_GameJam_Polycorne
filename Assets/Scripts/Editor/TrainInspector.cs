using Assets.Scripts;
using UnityEditor;
using UnityEngine;

namespace ScienceGameJam.UnityEditor
{
   
    [CustomEditor(typeof(Train))]
    public class TrainInspector : Editor
    {
        Train _target;

        public void OnEnable()
        {
            if(base.target != null)
                this._target = (Train)base.target;
        }
        

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("ID", this._target.GUID.ToString());

            base.OnInspectorGUI();
        }

        public void OnSceneGUI()
        {
            
        }
    }

}
