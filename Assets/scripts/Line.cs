using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Line : MonoBehaviour
{
    private pole start { get; set; }
    private pole end { get; set; }

    public pole Start
    {
        get { return start; }
        set { start = value; }
    }

    public pole End
    {
        get { return end; }
        set { end = value; }
    }

    public bool CompareLine(Line other)
    {
        // Debug.Log($"my start {this.start.transform.position} ");
        // print($"their start{other.start.transform.position}\n"); 
        // print($"my end {this.end.transform.position}\n"); 
        // print($" their end{other.end.transform.position}");  
        return (this.start.transform.position == other.start.transform.position &&
                this.end.transform.position == other.end.transform.position) ||
                (this.start.transform.position == other.end.transform.position &&
                this.end.transform.position == other.start.transform.position);

    }
    public Line() { }

    public Line(pole Start)
    {
        this.start = start;
    }

    public Line(pole start, pole end)
    {
        this.start = start;
        this.end = end;
    }

    public void clearLine(Line line)
    {

        LineRenderer lr = line.GetComponent<LineRenderer>(); 
        start.ConnectedLines.Remove(lr);
        end.ConnectedLines.Remove(lr);      
    }
}
