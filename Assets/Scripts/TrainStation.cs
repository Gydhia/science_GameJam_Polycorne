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
    public System.Random rand;


    public void Start()
    {
        this.rand = new System.Random();
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
