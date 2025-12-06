using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;

public class GridStart : MonoBehaviour
{
    // Start is called before the first frame update
    [field: SerializeField] public pole firstPole { get; set; }

    public pole FirstPole
    {
        get { return firstPole; }
        set { firstPole = value; }
    }

    public static List<pole> frontier;

    public int CheckGrid(pole root)
    {
        int output = CheckingGrid(root, new List<pole>() ) ;
        return output;
     }

    public int CheckingGrid(pole root, List<pole> visited)
    {
        if (root == null)
        {
            return 0;
        }

        int output = 0;

        if (root.isGoal)
        {
            output++;
        }

        foreach (LineRenderer i in root.ConnectedLines)
        {
            Line line = i.GetComponent<Line>();

            if (!visited.Contains(line.End))
            {
                visited.Add(line.End);
                output += CheckingGrid(line.End, visited);

            }
            if (!visited.Contains(line.Start))
            {
                visited.Add(line.Start);
                output += CheckingGrid(line.Start, visited);

            }

            //printList(GridStart.visited); 

        }
        // print(output);
        return output;
    }

    public void printList(List<pole> list)
    {
        foreach (pole p in list)
        {
            print(p.gameObject.name);
        }
    }

}
