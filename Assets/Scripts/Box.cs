using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Box : TrainHandler
{
    public BoxSO BoxSO;
    public BoxSO PreviousBoxSO;

    [Header("Box elements")]
    public UnityEngine.UI.Text Label;
    /// <summary>
    /// List of space available for cards
    /// </summary>
    public CardSpace[,] CardSpaces;

    [Header("Cardspace autocreation elements")]
    public CardSpace CardSpacePrefab;
    public Transform CardSpaceContainer;

    [Header("Settings")]
    public double[] OutputDistribution;
    public WinningGare WinningGare;
    public string CardNameForPlayer = "";

    public event TrainHandlerTransitEventData.Event OnTrainTransited; // event

    public override TrainHandlerType TrainHandlerType
    {
        get
        {
            return TrainHandlerType.Box;
        }
    }

    public override void Start()
    {
        this.AreTracksAutoSnapped = true;

        if (this.BoxSO == null)
            throw new Exception("BoxSO is not defined for this Box");

        Board.Instance.RegisterBox(this);

        if (this.CardSpaces == null)
            RegenerateCardsspace();

        base.Start();
    }

    public void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (this.BoxSO != null && this.BoxSO != this.PreviousBoxSO)
            {
                float pixerperunit = Board.Instance.BoardCanvas.referencePixelsPerUnit;
                float canvascardheight = this.BoxSO.CardSpaceHeight * pixerperunit;
                float canvascardhlength = this.BoxSO.CardSpaceLength * pixerperunit;

                this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(canvascardhlength, canvascardheight);

                //hands init
                this.RegenerateHands();

                //cardspace init
                this.RegenerateCardsspace();

                this.PreviousBoxSO = this.BoxSO;
            }
        }
    }

    public Hand DecideOutput()
    {
        if (this.OutputDistribution == null || this.OutputDistribution.Count() <= 1)
            // if no distribution was set, no weights: return a random output
            return this.HandsRight.ElementAt(this.rand.Next(0, this.HandsRight.Count()));

        double total_weight = this.OutputDistribution.Sum();
        // if no weight was set, no weights: return a random output
        if (total_weight == 0)
            return this.HandsRight.ElementAt(this.rand.Next(0, this.HandsRight.Count()));

        var result = this.rand.NextDouble() * total_weight;

        int chosen_output = -1;
        while (result > 0)
        {
            chosen_output++;
            result -= this.OutputDistribution.ElementAt(chosen_output);
        }
        return this.HandsRight.ElementAt(chosen_output);
    }

    public override void RegenerateHands()
    {
        if (this.HandsContainer == null)
            return;

        float pixerperunit = Board.Instance.BoardCanvas.referencePixelsPerUnit;
        float canvascardheight = this.BoxSO.CardSpaceHeight * pixerperunit;
        float canvascardhlength = this.BoxSO.CardSpaceLength * pixerperunit;

#if UNITY_EDITOR
        foreach (Transform child in this.HandsContainer)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                GameObject.DestroyImmediate(child.gameObject);

            };
        }
#endif

        int handcount = Board.Instance.HandsCount;
        int nbhands = handcount;
        int handsoffset = (this.BoxSO.CardSpaceHeight - 1) * handcount;

        if (Board.Instance.IsStartStation(this))
        {
            this.HandsLeft = new Hand[nbhands];
            for (int j = 0; j < this.HandsLeft.Length; j++)
            {
                Hand newHand = GameObject.Instantiate<Hand>(this.HandPrefab, this.HandsContainer);
                newHand.transform.localPosition = new Vector3(0, canvascardheight - ((0.5f + j) * (canvascardheight / (float)(nbhands + handsoffset))), 0);
                newHand.transform.Rotate(new Vector3(0, 0, 180));
                newHand.Index = j;
                newHand.LeftHand = true;
                newHand.name = "LEFT HAND #" + j;
                newHand.RegisterTrainHandler(this);
                this.HandsLeft[j] = newHand;
            }
        }
        if (Board.Instance.IsEndStation(this))
        {
            this.HandsRight = new Hand[nbhands];
            for (int j = 0; j < this.HandsRight.Length; j++)
            {
                Hand newHand = GameObject.Instantiate<Hand>(this.HandPrefab, this.HandsContainer);
                newHand.transform.localPosition = new Vector3(canvascardhlength, canvascardheight - ((0.5f + j) * (canvascardheight / (float)(nbhands + handsoffset))), 0);
                newHand.Index = j;
                newHand.LeftHand = false;
                newHand.name = "RIGHT HAND #" + j;
                newHand.RegisterTrainHandler(this);
                this.HandsRight[j] = newHand;
            }
        }
    }

    public void RegenerateCardsspace(bool forceReset = false)
    {
        if (this.CardSpaceContainer == null)
            return;
        // deal with existing data
        var existing_cardspaces = this.CardSpaceContainer.GetComponentsInChildren<CardSpace>();
        if (!forceReset && existing_cardspaces.Count() > 0)
        {
            this.CardSpaces = new CardSpace[existing_cardspaces.Max(cs => cs.positionInBox.x) + 1, existing_cardspaces.Max(cs => cs.positionInBox.y) + 1];
            foreach (var existing_cardspace in existing_cardspaces)
            {
                existing_cardspace.Box = this;
                this.CardSpaces[existing_cardspace.positionInBox.x, existing_cardspace.positionInBox.y] = existing_cardspace;
            }
            return;
        }

        float pixerperunit = Board.Instance.BoardCanvas.referencePixelsPerUnit;
        float canvascardheight = this.BoxSO.CardSpaceHeight * pixerperunit;
        float canvascardhlength = this.BoxSO.CardSpaceLength * pixerperunit;

#if UNITY_EDITOR
        foreach (Transform child in this.CardSpaceContainer)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                GameObject.DestroyImmediate(child.gameObject);
            };
        }
#endif

        this.CardSpaces = new CardSpace[this.BoxSO.CardSpaceLength, this.BoxSO.CardSpaceHeight];
        /*for (int i = 0; i < this.BoxSO.CardSpaceLength; i++)
        {
            for (int j = 0; j < this.BoxSO.CardSpaceHeight; j++)
            {
                this.CardSpaces[i, j] = new CardSpace();
                this.CardSpaces[i, j].Card = new Card(this.BoxSO.Cards[i, j]);
            }
        }*/

        for (int k = 0; k < this.BoxSO.CardSpaceLength; k++)
        {
            for (int l = 0; l < this.BoxSO.CardSpaceHeight; l++)
            {
                CardSpace cardspace = GameObject.Instantiate<CardSpace>(this.CardSpacePrefab, this.CardSpaceContainer);
                float x = (k * (canvascardhlength / (float)this.BoxSO.CardSpaceLength)) + (canvascardhlength * 0.5f / (float)this.BoxSO.CardSpaceLength);
                float y = ((this.BoxSO.CardSpaceHeight - l - 1) * (canvascardheight / (float)this.BoxSO.CardSpaceHeight)) + (canvascardheight * 0.5f / (float)this.BoxSO.CardSpaceHeight);
                cardspace.transform.localPosition = new Vector3(x, y, 0);
                cardspace.Box = this;
                cardspace.positionInBox = new Vector2Int(k, l);
                this.CardSpaces[k, l] = cardspace;

            }
        }
    }

    public int GetCardSpaceColumn(CardSpace cardSpace)
    {
        int column = -1;

        for(int i = 0; i <  this.CardSpaces.GetLength(0); i++)
        {
            for (int j = 0; j < this.CardSpaces.GetLength(1); j++)
            {
                if (cardSpace == this.CardSpaces[i, j])
                {
                    column = i;
                }
            }
        }

        return column;
    }

    public int GetCardSpaceRow(CardSpace cardSpace)
    {
        int row = -1;

        for (int i = 0; i < this.CardSpaces.GetLength(0); i++)
        {
            for (int j = 0; j < this.CardSpaces.GetLength(1); j++)
            {
                if (cardSpace == this.CardSpaces[i, j])
                {
                    row = j;
                }
            }
        }

        return row;
    }

    public void FireTrainExitCard(Train train, Card card, Hand viaHand)
    {
        if (this.CardSpaces != null &&
            this.CardSpaces.Length > 0 &&
            this.CardSpaces[0, 0] != null &&
            this.CardSpaces[0, 0].Card != null)
        {
            if (card.CardSpace == null)
                Debug.LogError("Train " + train.name + " is coming from card " + card.name + " which do not have a cardspace");

            // choose which card to use if overlap.
            int y = 0;

            int cardSpaceColumn = this.GetCardSpaceColumn(card.CardSpace);
            int cardSpaceRow = this.GetCardSpaceRow(card.CardSpace);

            if (cardSpaceColumn == -1)
                Debug.LogError("Ounch, the cardspace " + card.CardSpace.name + "is not a member of the box " + this.name);

            if (viaHand.LeftHand)
            {
                // check the cardspace on the left

                if(cardSpaceColumn == 0)
                {
                    // no more card space on the left : exit the box via the correct hand!
                    Hand toHand = this.HandsLeft[viaHand.Index];

                    this.FireTrainExited(train, toHand);
                }
                else
                {
                    // there is another cardspace : switch to the card located there
                    Card toCard = this.CardSpaces[(cardSpaceColumn - 1), cardSpaceRow].Card;
                    Hand toHand = toCard.HandsRight[viaHand.Index];
                    this._fireTrainTransited(train, viaHand, toHand);
                }
                /*if (this.CardSpaces.GetLength(1) > 1)
                {
                    y = -1;
                    float total_weight = 0;
                    for (int i = 0; i < this.CardSpaces.GetLength(1); i++)
                        total_weight += this.CardSpaces[0, i].OverlapWeight;
                    double result = this.rand.NextDouble() * total_weight;
                    while (result > 0)
                    {
                        y++;
                        result -= this.CardSpaces[0, y].OverlapWeight;
                    }
                }
                if (this.CardSpaces[0, y].Card != null)
                    train.PlaceOnHand(this.CardSpaces[0, y].Card.HandsLeft[viaHand.Index]);*/
            }
            else
            {
                // check the cardspace on the right

                if (cardSpaceColumn == (this.CardSpaces.GetLength(0) - 1))
                {
                    // no more card space on the right : exit the box !
                    Hand toHand = this.HandsRight[viaHand.Index];

                    this.FireTrainExited(train, toHand);
                }
                else
                {
                    // there is another cardspace : switch to the card located there
                    Card toCard = this.CardSpaces[(cardSpaceColumn + 1), cardSpaceRow].Card;
                    Hand toHand = toCard.HandsLeft[viaHand.Index];
                    this._fireTrainTransited(train, viaHand, toHand);
                }
            }
        }
        else
        {
            // HERE IS A FAKE BEHAVIOUR:
            // it should fail, but let's connect across the BOX for now
            //Hand exit;
            //if (Hand.LeftHand)
            //    exit = this.HandsRight.First(h => h.Index == Hand.Index);
            //else
            //    exit = this.HandsLeft.First(h => h.Index == Hand.Index);
            //Train.PlaceOnHand(exit);
        }
    }

    protected override bool _fireTrainEntered(Train train, Hand viaHand)
    {
        bool isTrainAllowedToEnter = false;

        if (Board.Instance.IsFailStation(this))
        {
            train.Animator.SetBool("die", true);
            //train.speedDecreaseOverTimeValue = 5;
        }
        if (Board.Instance.IsEndStation(this))
        {
            Board.Instance.RegisterTrainArrival(train, viaHand, this);
            if (SoundController.Instance != null)
                SoundController.Instance.PlaySound(SoundController.SoundNames.WhispArrival);
        }
        /*else if (Board.Instance.StartStation == this)
        {
            return;
        }*/

        if (this.CardSpaces != null &&
            this.CardSpaces.Length > 0 &&
            this.CardSpaces[0, 0] != null &&
            this.CardSpaces[0, 0].Card != null)
        {
            if (viaHand.LeftHand)
            {
                Card toCard = this.CardSpaces[0, 0].Card;
                Hand toHand = toCard.HandsLeft[viaHand.Index];
                train.PlaceOnHand(toHand);
            }
            else
            {
                Card toCard = this.CardSpaces[(this.CardSpaces.GetLength(0) - 1), 0].Card;
                Hand toHand = toCard.HandsRight[viaHand.Index];
                train.PlaceOnHand(toHand);
            }

            isTrainAllowedToEnter = true;
        }
        else
        {
            // HERE IS A FAKE BEHAVIOUR:
            // it should fail, but let's connect across the BOX for now
            //Hand exit;
            //if (Hand.LeftHand)
            //    exit = this.HandsRight.First(h => h.Index == Hand.Index);
            //else
            //    exit = this.HandsLeft.First(h => h.Index == Hand.Index);
            //Train.PlaceOnHand(exit);
        }

        return isTrainAllowedToEnter;
    }

    protected override void _fireTrainExited(Train train, Hand toHand)
    {
        train.PlaceOnHand(toHand);
    }

    protected void _fireTrainTransited(Train train, Hand fromHand, Hand toHand)
    {
        train.PlaceOnHand(toHand);

        if (this.OnTrainTransited != null)
            this.OnTrainTransited(new TrainHandlerTransitEventData(train, this, fromHand, toHand));
    }
}
