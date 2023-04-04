using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NarrativeItems
{
    public int id;
    public string name;
    public string stat;
    public string modifier;
    public float amount;
    public string audioOnAcquisition;
    public string description;
    public Sprite sprite;

    public NarrativeItems()
    {

    }

    public NarrativeItems(NarrativeItems input)
    {
        this.id = input.id;
        this.name = input.name;
        this.stat = input.stat;
        this.modifier = input.modifier;
        this.amount = input.amount;
        this.audioOnAcquisition = input.audioOnAcquisition;
        this.description = input.description;
        this.sprite = Resources.Load<Sprite>("Sprites/" + name);
    }
}
