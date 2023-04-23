using System;
using System.Collections.Generic;
[Serializable]
public class Flavor 
{
    public int ID; // also known as itemID
    public float amount;

    // nonStatic get flavor strength ==> returns float, accepts flavor list => return (this amount / total amount in list [use sum function])

    public static List<Flavor> ComputeFlavor(Food foodA, Food foodB)
    {
        List<Flavor> biggerList = foodA.FlavorData.Count >= foodB.FlavorData.Count ? foodA .FlavorData : foodB.FlavorData;
        List<Flavor> mixList = foodA.FlavorData.Count < foodB.FlavorData.Count ? foodA.FlavorData : foodB.FlavorData;

        foreach (Flavor flavor in biggerList)
        {
            // If it is not already in mixList add
            // if it is in mixList increase the flavor
        }

        return mixList;
    }

    public static List<Flavor> ComputeFlavor(List<Flavor> flavorList)
    {
        List<Flavor> mixList = new List<Flavor>();

        foreach (Flavor flavor in flavorList)
        {
            // If it is not already in mixList add
            // if it is in mixList increase the flavor
        }

        return mixList;
    }

}
