using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TrainStation : MonoBehaviour
{
    public List<LineRenderer> Outputs;
    public List<double> OutputDistribution;



    public void SendTrain()
    {
        var train = GameObject.Instantiate<Train>(Resources.Load<Train>("prefabs/train"));
        train.PlaceOnTracks(this.Outputs.First());
    }


}
