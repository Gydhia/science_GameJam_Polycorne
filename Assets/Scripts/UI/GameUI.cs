using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Assets.Scripts.UI
{
    public class GameUI : MonoBehaviour
    {
        public PlayableDirector director;
        public GameObject EndPopup;
        public GameObject DialogPopup;

        public void StartTimeline()
        {
            director.Play();
        }

        public void OnClickCloseButton()
        {
            DialogPopup.SetActive(false);
            EndPopup.SetActive(false);
        }
    }
}
