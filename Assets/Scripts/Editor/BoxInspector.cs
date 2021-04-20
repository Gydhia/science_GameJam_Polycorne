using Assets.Scripts;
using UnityEditor;
using UnityEngine;

namespace ScienceGameJam.UnityEditor
{
   
    [CustomEditor(typeof(Box))]
    public class BoxInspector : Editor
    {
        Box _target;

        public void OnEnable()
        {
            if(base.target != null)
                this._target = (Box)base.target;
        }
        

        public override void OnInspectorGUI()
        {
            if (GUI.Button(GUILayoutUtility.GetRect(0, int.MaxValue, 20, 20), "Generate Hands Owner"))
            {
                this._target.GenerateHandsOwner();
                foreach (Hand hand in this._target.AllHands)
                {
                    SerializedObject handSerializedObject = new SerializedObject(hand);
                    handSerializedObject.Update();
                }
                foreach (Track track in this._target.Tracks)
                {
                    SerializedObject trackSerializedObject = new SerializedObject(track);
                    trackSerializedObject.Update();
                }

                EditorUtility.SetDirty(this._target.gameObject);
            }
            if (GUI.Button(GUILayoutUtility.GetRect(0, int.MaxValue, 20, 20), "Regenerate Cards Space"))
            {
                this._target.RegenerateCardsspace(true);
            }

            if (GUI.Button(GUILayoutUtility.GetRect(0, int.MaxValue, 20, 20), "Regenerate Hands"))
            {
                this._target.RegenerateHands();
            }

            base.OnInspectorGUI();
        }
    }

}
