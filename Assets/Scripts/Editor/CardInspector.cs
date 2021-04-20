using Assets.Scripts;
using UnityEditor;
using UnityEngine;

namespace ScienceGameJam.UnityEditor
{

    [CustomEditor(typeof(Card))]
    public class CardInspector : Editor
    {
        Card _target;

        public void OnEnable()
        {
            if(base.target != null)
                this._target = (Card)base.target;
        }
        

        public override void OnInspectorGUI()
        {
            if (GUI.Button(GUILayoutUtility.GetRect(0, int.MaxValue, 20, 20), "Regenerate Hands"))
            {
                this._target.RegenerateHands();
            }
            if (GUI.Button(GUILayoutUtility.GetRect(0, int.MaxValue, 20, 20), "Connect tracks"))
            {
                this._target.GenerateConnectedTracks();
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
            if (GUI.Button(GUILayoutUtility.GetRect(0, int.MaxValue, 20, 20), "Snap tracks"))
            {
                this._target.SnapTracks(false);
            }

            base.OnInspectorGUI();
        }
    }

}
