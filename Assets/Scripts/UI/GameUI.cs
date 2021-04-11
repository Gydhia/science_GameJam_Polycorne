using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Assets.Scripts.UI
{
    public class GameUI : MonoBehaviour
    {
        public PlayableDirector director;

        public void StartTimeline()
        {
            director.Play();
        }
    }
}
