using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class LineCollision : MonoBehaviour
{
    private LineRenderer lr;
    private PolygonCollider2D pc;
    private float width;
    private List<Vector2> CollisionPoints;

    // Update is called once per frame
    public void UpdateCollisionPoints()
    {
        CollisionPoints = UpdateColliderPoints();
        pc.SetPath(0, CollisionPoints.ConvertAll(p => (Vector2)transform.InverseTransformPoint(p)));
    }

    public LineCollision(LineRenderer lineRenderer, float width)
    {
        lr = lineRenderer;
        this.width = width;
    }

    public void InitLineCollider(LineRenderer lr, PolygonCollider2D pc, float width)
    {
        this.lr = lr;
        this.pc = pc;
        this.width = width;
    }

    private List<Vector2> UpdateColliderPoints()
    {
        Vector3[] positions = new Vector3[lr.positionCount];

        for (int i = 0; i < lr.positionCount; i++)
        {
            positions[i] = lr.GetPosition(i);
        }

        //The following code was inspired by Blankdev
        //https://www.youtube.com/watch?v=BfP0KyOxVWs

        // m = (y2 - y1 / (x2 - 21))
        float m = (positions[1].y - positions[0].y) / (positions[1].x - positions[0].x);

        float dx = (width / 2f) * (m / Mathf.Pow(m * m + 1, .5f));
        float dy = (width / 2f) * (1 / MathF.Pow(1 + m * m, .5f));

        Vector3[] offsets = new Vector3[2];
        offsets[0] = new Vector3(-dx, dy);
        offsets[1] = new Vector3(dx, -dy);

        List<Vector2> colliderPoints = new List<Vector2>
        {
            positions[0] + offsets[0],
            positions[1] + offsets[0],
            positions[1] + offsets[1],
            positions[0] + offsets[1]
        };

        colliderPoints = SlideCollisionPoints(colliderPoints); 
        return colliderPoints;
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.magenta;
    //     if (CollisionPoints != null) CollisionPoints.ForEach(p => Gizmos.DrawSphere(p, .025f));
    // }

    private List<Vector2> SlideCollisionPoints(List<Vector2> points, float distancetoSlide = .25f)
    {

        //if we calculate the direction of the resultant vectors
        //we can then multiply this by the distance we eant to move along the vector
        Vector2 v = points[1] - points[0];
        Vector2 u = v / v.magnitude;
        points[0] += (distancetoSlide * u);
        points[1] -= (distancetoSlide * u);

        v = points[2] - points[3];
        u = v / v.magnitude;
        points[2] -= (distancetoSlide * u);
        points[3] += (distancetoSlide * u);
        return points; 
    }
}
