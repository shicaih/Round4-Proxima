using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[System.Serializable]
public class RouteLevel
{
    public static int levelCount = 0;
    public Transform interacivePointsContainer;
    public Transform blockingAreasContainer;
    public Transform enterPoint, exitPoint;

    private Transform[] interactivePoints;
    public RectTransform[] blockingAreas;


    public RouteLevel()
    {
        levelCount += 1;
    }

    public void SetExitPoint()
    {
        int n = interacivePointsContainer.childCount;
        exitPoint = interacivePointsContainer.GetChild(n - 1).GetComponent<Transform>();
        enterPoint = interacivePointsContainer.GetChild(0).GetComponent<Transform>();
    }

    public void SetPointsAndAreas()
    {
        int m = interacivePointsContainer.childCount;
        int n = blockingAreasContainer.childCount;
        RouteRender routeRender = GameObject.Find("RouteRender").GetComponent<RouteRender>();

        interactivePoints = new Transform[m];
        blockingAreas = new RectTransform[n];
        for (int i = 0; i < m; i++)
        {
            interactivePoints[i] = interacivePointsContainer.GetChild(i);
            if (i != 0)
            {
                interactivePoints[i].GetComponent<InteractivePoint>().PointClick.AddListener(routeRender.ActivatePoint);
            }
            
        }
        for (int i = 0; i < n; i++)
        {
            blockingAreas[i] = blockingAreasContainer.GetChild(i).GetComponent<RectTransform>();
            Vector3[] corners = new Vector3[4];
            blockingAreas[i].GetComponent<RectTransform>().GetWorldCorners(corners);
            Debug.Log(blockingAreas[i].gameObject.name);
            foreach (Vector3 corner in corners)
            {
                Debug.Log(corner.ToString("F4"));
            }
        }

    }
}

public class RouteRender : MonoBehaviour
{
    public UnityEvent LevelEnd;
    public ScreenManager sm;


    [SerializeField]
    private RouteLevel[] levels;
    [SerializeField]
    private Transform levelObjContainer;
    [SerializeField]
    private Material[] mats;
    [SerializeField]
    private AudioClip[] audios;
    [SerializeField]
    private TextMeshProUGUI text;
    private AudioSource audioManager;
    private Transform[] levelObjs;

    private LineRenderer lineRend;
    private Animator ani;
    private InteractivePoint currentEnterPoint, currentExitPoint;
    private static RouteLevel currentLevel;
    private static Vector3[] areaCorners = new Vector3[4];
    private static Vector3[] pointsActivated, pointsToDisplay, finalPlayerPoints;

    public bool levelStart = true;
    private bool isInLevel = false;
    public int currentLevelIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (LevelEnd == null)
        {
            LevelEnd = new UnityEvent();
        }
        lineRend = GetComponent<LineRenderer>();
        ani = GetComponent<Animator>();

        IniLevel(levels);

        levelObjs = new Transform[levelObjContainer.childCount];
        for (int i = 0; i < levelObjContainer.childCount; i++)
        {
            levelObjs[i] = levelObjContainer.GetChild(i);
            levelObjs[i].gameObject.SetActive(false);
        }
        audioManager = GetComponent<AudioSource>();

    }

    private void Update()
    {
        LevelProcess();
        if (isInLevel)
        {
            RenderRoute(pointsToDisplay);
        }
        else if (finalPlayerPoints != null)
        {
            RenderRoute(finalPlayerPoints);
        }
    }

    private void RenderRoute(Vector3[] points)
    {

        lineRend.positionCount = points.Length;
        lineRend.SetPositions(points);
        //GameObject wrongArea;
        //int n = pointsActivated.Length;
        //if (n >= 2 && IsAreasIntersecting(pointsActivated[n - 2], pointsActivated[n - 1], currentLevel.blockingAreas, out wrongArea))
        //{
        //    this.GetComponent<LineRenderer>().material = mats[1];
        //}
        //else
        //{
        //    this.GetComponent<LineRenderer>().material = mats[0];
        //}
    }

    public void ActivatePoint(InteractivePoint point)
    {
        int n = pointsActivated.Length;
        
        if (point.transform.position == pointsActivated[n - 1])
        {
            if (n > 1)
            {
                Array.Resize(ref pointsActivated, n - 1);
                Array.Resize(ref pointsToDisplay, n - 1);
                point.isActivated = false;
                point.isConnected = false;
            }
        }
        else
        {
            Array.Resize(ref pointsActivated, n + 1);
            Array.Resize(ref pointsToDisplay, n + 1);
            pointsActivated.SetValue(point.transform.position, n);
            pointsToDisplay.SetValue(posForDisplay(point.transform), n);
            point.isActivated = true;
            GameObject wrongArea;
            if (IsAreasIntersecting(pointsActivated[n - 1], pointsActivated[n], currentLevel.blockingAreas, out wrongArea))
            {
                ani.SetTrigger("Alert");
                Debug.Log(wrongArea.name);
                audioManager.PlayOneShot(audios[0]);
                StartCoroutine(Alert(n));
                point.isActivated = false;

            }
            else
            {
                point.isConnected = true;
            }
        }
    }
    IEnumerator Alert(int n)
    {
        yield return new WaitForSeconds(1.25f);
        ani.SetTrigger("Idle");
        Array.Resize(ref pointsActivated, n);
        Array.Resize(ref pointsToDisplay, n);
    }

    private void StartLevel()
    {
        levelObjs[currentLevelIndex].gameObject.SetActive(true);
        currentLevel = levels[currentLevelIndex];
        currentEnterPoint = currentLevel.enterPoint.GetComponent<InteractivePoint>();
        currentEnterPoint.isEnter = true;
        pointsActivated = new Vector3[] { currentEnterPoint.transform.position };
        pointsToDisplay = new Vector3[] { posForDisplay(currentEnterPoint.transform) };
        currentExitPoint = currentLevel.exitPoint.GetComponent<InteractivePoint>();
        isInLevel = true;
        levelStart = false;
    }
    private Vector3 posForDisplay(Transform trans)
    {
        Vector3 oriPos = trans.localPosition + Vector3.back * 0.1f;
        return trans.parent.TransformPoint(oriPos);

    }

    private void LevelProcess()
    {
        if (levelStart)
        {
            StartLevel();
        }

        if (isInLevel && currentExitPoint.isActivated && currentExitPoint.isConnected)
        {

            isInLevel = false;
            Debug.Log("Level Finished");
            sm.ReduceStamina();
            audioManager.PlayOneShot(audios[1]);
            finalPlayerPoints = new Vector3[pointsToDisplay.Length];
            Array.Copy(pointsToDisplay, finalPlayerPoints, pointsToDisplay.Length);
            Animator ani = text.GetComponent<Animator>();
            ani.SetTrigger("FadeIn");
            LevelEnd.Invoke();
        }
    }

    private void IniLevel(RouteLevel[] levels)
    {
        foreach (RouteLevel level in levels)
        {
            level.SetPointsAndAreas();
            level.SetExitPoint();
        }
    }

    private bool IsAreasIntersecting(Vector3 p1, Vector3 p2, RectTransform[] areas, out GameObject wrongArea)
    {
        foreach (RectTransform area in areas)
        {

            area.GetWorldCorners(areaCorners);
            if (IsIntersecting(p1, p2, areaCorners))
            {
                wrongArea = area.gameObject; ;
                return true;
            }
        }
        wrongArea = null;
        return false;
    }

    private bool IsIntersecting(Vector3 p1, Vector3 p2, Vector3[] _areaCorners)
    {
        Vector3 r1 = _areaCorners[0];
        Vector3 r2 = _areaCorners[1];
        Vector3 r3 = _areaCorners[2];
        Vector3 r4 = _areaCorners[3];


        return (IsLinesIntersecting2(p1, p2, r1, r2)
            || IsLinesIntersecting2(p1, p2, r2, r3)
            || IsLinesIntersecting2(p1, p2, r3, r4)
            || IsLinesIntersecting2(p1, p2, r4, r1)
            );
    }

    private static bool IsLinesIntersecting(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        bool isIntersecting = false;
        if (IsPointsOnDifSides(p1, p2, p3, p4) && IsPointsOnDifSides(p3, p4, p1, p2))
        {
            isIntersecting = true;
        }

        return isIntersecting;
    }

    bool IsLinesIntersecting2(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        bool isIntersecting = false;

        //3d -> 2d
        Vector2 l1_start = new Vector2(p1.x, p1.z);
        Vector2 l1_end = new Vector2(p2.x, p2.z);

        Vector2 l2_start = new Vector2(p3.x, p3.z);
        Vector2 l2_end = new Vector2(p4.x, p4.z);

        //Direction of the lines
        Vector2 l1_dir = (l1_end - l1_start).normalized;
        Vector2 l2_dir = (l2_end - l2_start).normalized;

        //If we know the direction we can get the normal vector to each line
        Vector2 l1_normal = new Vector2(-l1_dir.y, l1_dir.x);
        Vector2 l2_normal = new Vector2(-l2_dir.y, l2_dir.x);


        //Step 1: Rewrite the lines to a general form: Ax + By = k1 and Cx + Dy = k2
        //The normal vector is the A, B
        float A = l1_normal.x;
        float B = l1_normal.y;

        float C = l2_normal.x;
        float D = l2_normal.y;

        //To get k we just use one point on the line
        float k1 = (A * l1_start.x) + (B * l1_start.y);
        float k2 = (C * l2_start.x) + (D * l2_start.y);


        //Step 2: are the lines parallel? -> no solutions
        if (IsParallel(l1_normal, l2_normal))
        {
            Debug.Log("The lines are parallel so no solutions!");

            return isIntersecting;
        }
        //Step 3: are the lines the same line? -> infinite amount of solutions
        //Pick one point on each line and test if the vector between the points is orthogonal to one of the normals
        if (IsOrthogonal(l1_start - l2_start, l1_normal))
        {
            Debug.Log("Same line so infinite amount of solutions!");

            //Return false anyway
            return isIntersecting;
        }


        //Step 4: calculate the intersection point -> one solution
        float x_intersect = (D * k1 - B * k2) / (A * D - B * C);
        float y_intersect = (-C * k1 + A * k2) / (A * D - B * C);

        Vector2 intersectPoint = new Vector2(x_intersect, y_intersect);


        //Step 5: but we have line segments so we have to check if the intersection point is within the segment
        if (IsBetween(l1_start, l1_end, intersectPoint) && IsBetween(l2_start, l2_end, intersectPoint))
        {
            Debug.Log("We have an intersection point!");

            isIntersecting = true;
        }

        return isIntersecting;
    }

    //Are 2 vectors parallel?
    bool IsParallel(Vector2 v1, Vector2 v2)
    {
        //2 vectors are parallel if the angle between the vectors are 0 or 180 degrees
        if (Vector2.Angle(v1, v2) == 0f || Vector2.Angle(v1, v2) == 180f)
        {
            return true;
        }

        return false;
    }
    //Are 2 vectors orthogonal?
    bool IsOrthogonal(Vector2 v1, Vector2 v2)
    {
        //2 vectors are orthogonal is the dot product is 0
        //We have to check if close to 0 because of floating numbers
        if (Mathf.Abs(Vector2.Dot(v1, v2)) < 0.000001f)
        {
            return true;
        }

        return false;
    }

    //Is a point c between 2 other points a and b?
    bool IsBetween(Vector2 a, Vector2 b, Vector2 c)
    {
        bool isBetween = false;

        //Entire line segment
        Vector2 ab = b - a;
        //The intersection and the first point
        Vector2 ac = c - a;

        //Need to check 2 things: 
        //1. If the vectors are pointing in the same direction = if the dot product is positive
        //2. If the length of the vector between the intersection and the first point is smaller than the entire line
        if (Vector2.Dot(ab, ac) > 0f && ab.sqrMagnitude >= ac.sqrMagnitude)
        {
            isBetween = true;
        }

        return isBetween;
    }

    private static bool IsPointsOnDifSides(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        bool isOnDifSides = false;

        Vector3 lineDir = p2 - p1;
        Vector3 lineNormal = new Vector3(-lineDir.z, lineDir.y, lineDir.x);

        float dot1 = Vector3.Dot(lineNormal, p3 - p1);
        float dot2 = Vector3.Dot(lineNormal, p4 - p1);
        if (dot1 * dot2 < 0f)
        {
            isOnDifSides = true;
        }
        return isOnDifSides;
    }
}
