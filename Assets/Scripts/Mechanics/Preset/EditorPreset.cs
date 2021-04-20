using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ScienceGameJam.Mechanics.Preset
{
    [CreateAssetMenu(fileName = "EditorPreset", menuName = "FeuFolie/Preset/EditorPreset")]
    public class EditorPreset : ScriptableObject
    {
        public float HandSnapDistance = 15f;
        public float HandBallSize = 10f;
        public float TrackBallSize = 10f;
        public Texture HandBoxTexture;
        public Texture HandCardTexture;
        public Texture HandErrorTexture;
    }
}
