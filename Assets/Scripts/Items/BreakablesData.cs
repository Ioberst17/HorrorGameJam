using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BreakablesData
{
    public class Breakable
    {
        public int heartsAmount;
        public int ammoAmount;

        public Breakable(int hearts, int ammo)
        {
            this.heartsAmount = hearts; this.ammoAmount = ammo;
        }
    }

    public static Dictionary<string, Breakable> breakablesData = new Dictionary<string, Breakable>
    {
        {"Crate", new Breakable(2, 100)}
    };


}
