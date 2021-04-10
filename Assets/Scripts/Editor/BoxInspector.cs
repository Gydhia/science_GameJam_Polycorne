using Assets.Scripts;
using UnityEditor;
using UnityEngine;

namespace ScienceGameJam.UnityEditor
{
   
    [CustomEditor(typeof(Box))]
    public class BoxInspector : Editor
    {
        Box target;

        public void OnEnable()
        {
            if(base.target != null)
                this.target = (Box)base.target;
        }
        

        public override void OnInspectorGUI()
        {
            if (GUI.Button(GUILayoutUtility.GetRect(0, int.MaxValue, 20, 20), "Regenerate Cards Space"))
            {
                this.target.RegenerateCardsspace();
            }

            if (GUI.Button(GUILayoutUtility.GetRect(0, int.MaxValue, 20, 20), "Regenerate Hands"))
            {
                this.target.RegenerateHands();
            }

            base.OnInspectorGUI();
        }
    }

}
