using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ResultsPanel : MonoBehaviour
{
    public RectTransform[] columns;
    public UnityEngine.UI.Text[] labels;
    public UnityEngine.UI.Text globalscore;
    public TextMesh globalscoremesh;
    public string score_suffix = " pts";
    public void Start()
    {
    }

    internal void Refresh(int[] trainArrivals, int expectedTotal)
    {

        int total_arrivals = 0;
        if(trainArrivals != null)
            total_arrivals = trainArrivals.Sum();

        if (total_arrivals > 0)
        {
            for (int i = 0; i < trainArrivals.Length && i < this.columns.Count() && i < this.labels.Count(); i++)
            {
                this.columns[i].sizeDelta = new Vector2(this.columns[i].sizeDelta.x, trainArrivals[i]);
                this.labels[i].text = (100f * trainArrivals[i] / (float)expectedTotal).ToString("0") + "%";
            }
        }
        else
        {
            for (int i = 0; i < this.columns.Length && i < this.columns.Count() && i < this.labels.Count(); i++)
            {
                this.columns[i].sizeDelta = new Vector2(this.columns[i].sizeDelta.x, 0);
                this.labels[i].text = "";
            }
        }
        string score = "";
        if (total_arrivals > 0)
            score = total_arrivals.ToString("0") + this.score_suffix;
        if (this.globalscoremesh != null)
            globalscoremesh.text = score;
        if (this.globalscore != null)
            globalscore.text = score;
    }
}
