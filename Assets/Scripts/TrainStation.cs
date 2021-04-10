using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TrainStation : MonoBehaviour
{
    public List<Tracks> Outputs;
    public List<Transform> OutputsVisuals;
    public List<Tracks> Inputs;
    public List<Transform> InputsVisuals;
    public List<double> OutputDistribution;
    public System.Random rand;


    public void Start()
    {
        this.rand = new System.Random();

        for (int i = 0; i < Outputs.Count() && i < this.OutputsVisuals.Count(); i++)
        {
            Outputs.ElementAt(i).line.SetPosition(0, this.OutputsVisuals.ElementAt(i).transform.position);
        }
        for (int i = 0; i < Inputs.Count() && i < this.InputsVisuals.Count(); i++)
        {
            var inputTrack = Inputs.ElementAt(i);
            inputTrack.line.SetPosition(inputTrack.line.positionCount - 1, this.InputsVisuals.ElementAt(i).transform.position);
            inputTrack.OnTrainArrivedAtEnd += TrainArrived;
        }
    }

    private void TrainArrived(Train Train, Tracks Tracks)
    {
        //TODO: register stats about arrived trains
        int indexOfTrack = this.Inputs.IndexOf(Tracks);
        if (this.Outputs != null && this.Outputs.Count() > indexOfTrack)
            Train.PlaceOnTracks(this.Outputs.ElementAt(indexOfTrack));
    }

    public void SendManyTrains(int HowMany)
    {
        StartCoroutine(this.sendManyTrains(100));
    }

    private IEnumerator sendManyTrains(int HowMany)
    {
        for (int i = 0; i < HowMany; i++)
        {
            this.SendTrain();
            yield return i / (float)HowMany;
        }
        yield break;
    }

    public void SendTrain()
    {
        var train = GameObject.Instantiate<Train>(Resources.Load<Train>("prefabs/train"));
        int trackID = this.DecideOutput();
        train.PlaceOnTracks(this.Outputs.ElementAt(trackID));
    }

    private int DecideOutput()
    {
        if (this.OutputDistribution == null || this.OutputDistribution.Count() <= 1)
            // if no distribution was set, no weights: return a random output
            return this.rand.Next(0, this.Outputs.Count());

        double total_weight = this.OutputDistribution.Sum();
        // if no weight was set, no weights: return a random output
        if (total_weight == 0)
            return this.rand.Next(0, this.Outputs.Count());

        var result = this.rand.NextDouble() * total_weight;
        
        int chosen_output = -1;
        while (result > 0)
        {
            chosen_output++;
            result -= this.OutputDistribution.ElementAt(chosen_output);
        }
        return chosen_output;
    }
}
