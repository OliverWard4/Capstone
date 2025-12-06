using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum ItemCost
{
    Pole = 2,
    Line = 1,
    Drone = 3,
    DestructableHazard = 1
};

// This script manages the interaction with various interactable objects in the game, such as poles and lines.
// It handles mouse input, line drawing, and collision detection with hazards.
public class InteractableManager : MonoBehaviour
{
    [Header("Required For basic Level Functions")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private GridStart gridStart;
    [SerializeField] private int NumGoals;
    [SerializeField] private int StartingTokens = 5;
    [SerializeField] private Canvas pauseMenu;
    [SerializeField] private PauseMenu pauseMenuScript;

    [Header("Line Settings")]
    [SerializeField] private float DefualtMaxLineLength = 5f;
    [SerializeField] private GameObject SpawnerPrefabOne;
    [SerializeField] private LayerMask InteractableLayer = 9;
    [SerializeField] private LayerMask Hazardlayer = 10;
    [SerializeField] private float width;
    [SerializeField] private Material lineMaterial;

    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI ResourceText;

    //private vars
    private int ResourceTokens = 0;
    GameObject ClickedObject;
    private LineRenderer ActiveLine = null;
    private pole ActivePole = null;
    private float LineDepth = .1f;
    private bool MouseDown;
    private pole poleStart;
    private bool CanPlace = true;
    private Vector3 ClickedObjectOrigin;
    private int goalsReached = 0;

    private Vector3 startingPosition; 

    private bool canPlay = true;

    void Start()
    {
        Cursor.visible = true;
        ResourceTokens = StartingTokens;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 mousePosition = MouseToWorldSpace();
        if (ResourceText != null) {ResourceText.text = "Resource Tokens : " + ResourceTokens;}

        // When mouse initially pressed
        if (Input.GetMouseButtonDown(0))
        {
            // did I click anything?
            Vector2 ray = mainCam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero, 60f);
            if (hit.collider != null)
            {
                
                // if so is it something I care about
                GameObject hitObject = hit.collider.gameObject;

                // print(hitObject.tag);
                if (hitObject.tag == "pole" && ResourceTokens >= (int)ItemCost.Line)
                {
                    ActivePole = hit.collider.gameObject.GetComponent<pole>();
                    ActiveLine = DrawLine(hit.collider.transform.position);
                    poleStart = hit.collider.gameObject.GetComponent<pole>();
                    ResourceTokens -= (int)ItemCost.Line;
                }
                else if (hitObject.tag == "poleSpawner" && ResourceTokens >= (int)ItemCost.Pole)
                {
                    startingPosition = hit.point;
                    ClickedObject = Instantiate(SpawnerPrefabOne, hit.collider.transform.position, hit.collider.transform.rotation);
                    ClickedObject.GetComponent<pole>().Destroyable = true;
                    ClickedObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = 12;
                    ResourceTokens -= (int)ItemCost.Pole;
                }
                else if (hitObject.tag == "Line")
                {
                    Line line = hitObject.GetComponent<Line>();
                    line.clearLine(line);
                    Destroy(line.gameObject);
                    ResourceTokens++;
                }
                else if (hitObject.tag == "Chainsaw")
                {
                    ClickedObject = hit.collider.gameObject;
                    ClickedObjectOrigin = hit.collider.gameObject.transform.position;
                }

            }
        }

        // if mouse is held down
        if (Input.GetMouseButton(0))
        {
            // do we have line being moved
            if (ActiveLine != null)
            {
                Vector3 poleStartlocation = this.poleStart.transform.position;

                // Sweeps along a line whether or not a given line is in a valid location
                float LineLength = (new Vector2(poleStartlocation.x, poleStartlocation.y) - new Vector2(mousePosition.x, mousePosition.y)).magnitude;
                Vector2 lineDirection = poleStartlocation - MouseToWorldSpace();
                CanPlace = LineHazardSweep(LineLength, lineDirection);

                if (LineLength < DefualtMaxLineLength && CanPlace)
                { // if in range
                    MoveLine(ActiveLine, 1, MouseToWorldSpace());
                }
                MouseDown = true;
            }
            else CanPlace = MouseHazardSweep();

            // updates the pole and when let go the else block will detach block
            if (ClickedObject != null )
            {
                ClickedObject.transform.position = MouseToWorldSpace();
                if (CanPlace)
                {
                    if (ClickedObject.tag == "pole")
                        ClickedObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
                    MouseDown = true;
                }
                else
                {
                    if (ClickedObject.tag == "pole")
                        ClickedObject.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                }
                
                

            }
        }
        else
        {
            // if mouse is no longer down we check to see if there is a pole to attach to if not delete the line
            if (MouseDown)
            {
                if (ActiveLine != null)
                {
                    ConnectLineToPole();
                    goalsReached = gridStart.CheckGrid(gridStart.FirstPole);
                    //print($"Connected Lines to end:  {goalsReached}");
                    if (goalsReached >= NumGoals)
                    {
                        pauseMenu.gameObject.SetActive(true);
                        pauseMenuScript.WinScreen();
                        canPlay = false;


                    }
                }
                else if (ClickedObject != null)
                {
                    if (ClickedObject.tag == "poleSpawner")
                    {
                        ClickedObject = null;
                        MouseDown = false;
                    }
                    else if (ClickedObject.tag == "Chainsaw")
                    {
                        // did I click anything?
                        LayerMask InteractAndHazardMask = LayerMask.GetMask("Interactables", "Hazard");

                        Vector2 ray2 = mainCam.ScreenToWorldPoint(Input.mousePosition);
                        RaycastHit2D hit2 = Physics2D.Raycast(ray2, Vector2.zero, 60f, InteractAndHazardMask);
                        Debug.DrawRay(mainCam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Color.magenta, 1f);
                        if (hit2.collider != null)
                        {
                            //print(hit2.collider.gameObject);
                            if (hit2.collider.gameObject.tag == "pole")
                            {
                                pole hitPole = hit2.collider.gameObject.GetComponent<pole>();
                                if (hitPole.Destroyable)
                                {
                                    foreach (LineRenderer i in hitPole.ConnectedLines)
                                    {
                                        Line line = i.GetComponent<Line>();
                                        line.clearLine(line);
                                        Destroy(i.gameObject);
                                        ResourceTokens += (int)ItemCost.Line;
                                    }
                                    Destroy(hit2.collider.gameObject);
                                    ResourceTokens += (int)ItemCost.Pole;
                                }
                            }
                            else if (hit2.collider.gameObject.tag == "Destructable Hazard")
                            {
                                if (ResourceTokens >= (int)ItemCost.DestructableHazard)
                                {
                                    Destroy(hit2.collider.gameObject);
                                    ResourceTokens -= (int)ItemCost.DestructableHazard;
                                }
                            }
                        }

                        ClickedObject.transform.position = ClickedObjectOrigin;
                        ClickedObjectOrigin = Vector3.zero;
                        ClickedObject = null;
                        MouseDown = false;
                    }
                    else if(ClickedObject.tag == "pole")
                    {
                        float distanceMoved = (ClickedObject.transform.position - startingPosition).magnitude;
                        if (CanPlace && distanceMoved > 1.5f)
                        {
                            ClickedObject = null;
                            MouseDown = false;
                        }
                        else
                        {
                            Destroy(ClickedObject.gameObject);
                            ResourceTokens += (int)ItemCost.Pole;
                        }
                    }
                    else
                    {
                        ClickedObject = null;
                        MouseDown = false;
                    }

                }
            }
        }
    }

    // creates and initializes a LineRenderer
    // ***DOES NOT EQUATE CAMERA SPACE TO WORLD SPACE***
    public LineRenderer DrawLine(Vector3 startPoint, Vector3 endPoint)
    {
        // instantiates the line and its associated collider
        LineRenderer lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
        LineCollision lc = lineRenderer.gameObject.AddComponent<LineCollision>();
        PolygonCollider2D pc = lineRenderer.gameObject.AddComponent<PolygonCollider2D>();
        Line line = lineRenderer.gameObject.AddComponent<Line>();
        line.Start = ActivePole;


        lc.InitLineCollider(lineRenderer, pc, width);

        // instantiates the line's many properties
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.material = lineMaterial;
        lineRenderer.sortingOrder = 10;

        // For drawing line in the world space, provide the x,y,z values
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
        lineRenderer.gameObject.tag = "Line";

        return lineRenderer;
    }

    // Sets point x to position y of the line renderer z
    public LineRenderer DrawLine()
    {
        // A default line
        return DrawLine(new Vector3(0f, 0f, LineDepth), new Vector3(1f, 1f, LineDepth));
    }

    public LineRenderer DrawLine(Vector3 start)
    {
        // A default line
        return DrawLine(start, new Vector3(1f, 1f, LineDepth));
    }

    public void MoveLine(LineRenderer line, int point, Vector3 Position)
    {
        line.SetPosition(point, new Vector3(Position.x, Position.y, 0f));
        line.gameObject.GetComponent<LineCollision>().UpdateCollisionPoints();
    }

    public bool LineHazardSweep(float LineLength, Vector2 lineDirection, LayerMask ActiveLayer)
    {
        Vector3 poleStartlocation = this.poleStart.transform.position;

        Vector2 ray = new Vector2(poleStartlocation.x, poleStartlocation.y);
        RaycastHit2D HazardSweep = Physics2D.Raycast(ray, -lineDirection, LineLength, Hazardlayer);
        if (HazardSweep.collider != null)
        {
            if (HazardSweep.collider.gameObject.tag == "Hazard" || HazardSweep.collider.gameObject.tag == "Destructable Hazard")
            {
                CanPlace = false;
            }
            else CanPlace = true;
        }
        else CanPlace = true;

        return CanPlace;
    }

    public bool LineHazardSweep(float LineLength, Vector2 lineDirection)
    {
        return LineHazardSweep(LineLength, lineDirection, Hazardlayer);
    }

    private Vector3 MouseToWorldSpace()
    {
        Vector3 mouse = mainCam.ScreenPointToRay(Input.mousePosition).origin;
        return new Vector3(mouse.x, mouse.y, LineDepth);
    }

    private bool MouseHazardSweep()
    {
        // raycast from the mouse 
        Vector2 ray = mainCam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero, 60f);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.tag == "Hazard" || hit.collider.gameObject.tag == "Unstable Ground" || hit.collider.gameObject.tag == "Destructable Hazard")
            {
                CanPlace = false;
            }
            else CanPlace = true;
        }
        else CanPlace = true;

        return CanPlace;
    }

    public void AddResourceTokens(int AmountTokensAdded)
    {
        ResourceTokens += AmountTokensAdded;
    }

    public void PlayerLost()
    {
        ResourceTokens = 0;
        canPlay = false;
        pauseMenu.gameObject.SetActive(true);
        pauseMenuScript.LoseScreen();
    }

    //At this point the line is now being dropped so we start now have to check
    //whether that line has been made, manage resource tokens and clean up left over lines
    private void ConnectLineToPole()
    {
        Vector3 poleStartlocation = this.poleStart.transform.position;

        Vector2 ray = mainCam.ScreenToWorldPoint(Input.mousePosition);
        float LineLength = (poleStartlocation - MouseToWorldSpace()).magnitude;
        RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero, 60f, InteractableLayer);

        if (hit.collider != null && LineLength < DefualtMaxLineLength && CanPlace)
        {
            if (hit.collider.gameObject.tag == "pole" && hit.collider.transform.position != poleStartlocation)
            {
                pole otherPole = hit.collider.gameObject.GetComponent<pole>();
                Line ActiveLineComp = ActiveLine.GetComponent<Line>();
                MoveLine(ActiveLine, 1, hit.collider.transform.position);
                ActiveLineComp.End = otherPole;
                bool LineExists = false;
                foreach (LineRenderer i in otherPole.ConnectedLines)
                    LineExists |= i.GetComponent<Line>().CompareLine(ActiveLineComp);

                if (!LineExists)
                {
                    ActivePole.ConnectedLines.Add(ActiveLine);
                    otherPole.ConnectedLines.Add(ActiveLine);
                }
                else
                {
                    Destroy(ActiveLine.transform.root.gameObject);
                    ResourceTokens++;
                }
            }
            else
            {
                Destroy(ActiveLine.transform.root.gameObject);
                ResourceTokens++;
            }
        }
        else
        {
            Destroy(ActiveLine.transform.root.gameObject);
            ResourceTokens++;
        }
        ActivePole = null;
        ActiveLine = null;
        MouseDown = false;
    }

    public bool LevelComplete()
    {
        return NumGoals <= goalsReached;
    }

    public bool CanPlay()
    {
        return canPlay; 
    }

}


