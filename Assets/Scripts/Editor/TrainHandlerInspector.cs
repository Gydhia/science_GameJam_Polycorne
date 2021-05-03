using Assets.Scripts;
using UnityEditor;
using UnityEngine;

namespace ScienceGameJam.UnityEditor
{

    [CustomEditor(typeof(TrainHandler), true)]
    public class TrainHandlerInspector : Editor
    {
        TrainHandler _target;

        public void OnEnable()
        {
            if (base.target != null)
                this._target = (TrainHandler)base.target;
        }


        public override void OnInspectorGUI()
        {
            if (GUI.Button(GUILayoutUtility.GetRect(0, int.MaxValue, 20, 20), "Regenerate Hands"))
            {
                this._target.RegenerateHands();
            }

            base.OnInspectorGUI();
        }
    }

}
