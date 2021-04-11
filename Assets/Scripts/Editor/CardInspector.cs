using Assets.Scripts;
using UnityEditor;
using UnityEngine;

namespace ScienceGameJam.UnityEditor
{
   
    /*[CustomEditor(typeof(Card))]
    public class CardInspector : Editor
    {
        Card target;

        public void OnEnable()
        {
            if(base.target != null)
                this.target = (Card)base.target;
        }
        

        public override void OnInspectorGUI()
        {
            if (GUI.Button(GUILayoutUtility.GetRect(0, int.MaxValue, 20, 20), "Regenerate Hands"))
            {
                this.target.RegenerateHands();
            }

            base.OnInspectorGUI();
        }
    }*/

}
