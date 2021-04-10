using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class MainMenu : MonoBehaviour
    {
        public Button PlayButton;
        public Button CreditsButton;
        public Button ExitButton;

        void OnClickPlayButton()
        {
            SceneManager.LoadScene("SceneDuJeu");
        }

        public void OnClickExitButton()
        {
            print("ouiiiiiiii");
            Application.Quit();
        }
    }
}
