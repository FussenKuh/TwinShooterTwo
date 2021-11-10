using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpawner : MonoBehaviour
{
    [SerializeField]
    bool spawnNow = false;
    [SerializeField]
    bool drawDebugGrid = false;

    [Header("Size Configuration ----------")]
    [SerializeField]
    int worldWidth = 100;
    [SerializeField]
    int worldHeight = 500;
    [SerializeField]
    int minObjectSize = 5;
    [SerializeField]
    int maxObjectSize = 10;
    [SerializeField]
    [Range(0, 1)]
    float objectPercentDesired = 0.15f;
    [SerializeField]
    [ReadOnly]
    [Range(0, 1)]
    float objectPercentActual = 0.0f;
    [SerializeField]
    [ReadOnly]
    Rect worldSize;
    [SerializeField]
    [ReadOnly]
    float worldArea = 0;
    [SerializeField]
    [ReadOnly]
    float objectArea = 0;

    [Header("Prefab References ----------")]
    [SerializeField]
    GameObject objectPrefab;
    [SerializeField]
    GameObject wallPrefab;
    [SerializeField]
    GameObject startPrefab;
    [SerializeField]
    GameObject goalPrefab;

    [Header("Object Configuration ----------")]
    [SerializeField]
    bool allowObjectRotation = false;
    [SerializeField]
    bool randomObjectColors = false;
    [SerializeField]
    List<Color> objectColors = new List<Color>() { Color.white };

    [Header("Border Wall Configuration ----------")]
    [SerializeField]
    Color worldBorderColor = Color.black;
    [SerializeField]
    float borderWallThickness = 2.0f;

    [Header("Start/Goal Configuration ----------")]
    [SerializeField]
    [Range(0, 1)]
    float distancePercent = 0.6f;
    [SerializeField]
    [ReadOnly]
    public float minDistanceBetweenStartAndGoal = 0.0f;
    [SerializeField]
    [ReadOnly]
    public float actualDistanceBetweenStartAndGoal = 0.0f;


    [Header("Debug Information ----------")]
    [SerializeField]
    [ReadOnly]
    bool startLocationSpawned = false;
    [SerializeField]
    [ReadOnly]
    bool goalLocationSpawned = false;
    [SerializeField]
    [ReadOnly]
    bool objectsSpawned = false;
    [SerializeField]
    [ReadOnly]
    bool worldBoarderSpawned = false;
    [ReadOnly]
    bool spawning = false;


    public Vector3 Center { get { return transform.position; } }
    public Vector2 PlayAreaDimensions { get { return new Vector2(worldWidth, worldHeight); } }
    public Vector2 WorldDimensions { get { return new Vector2(worldWidth + borderWallThickness/2, worldHeight + borderWallThickness/2); } }
    public GameObject StartPoint { get; set; }
    public GameObject GoalPoint { get; set; }
    public Transform worldHolder;


    //public Grid<bool> walkableGrid;
    public Grid<GridObject> grid;

    public event System.EventHandler<LevelReadyArgs> OnLevelReadyEvent;

    public class LevelReadyArgs : System.EventArgs
    {
        public bool Error { get; set; }
        public string ErrorMessage { get; set; }
        public WorldSpawner World { get; set; }
        public float SpawnTime { get; set; }
    }

    [System.Serializable]
    public class WorldSettings
    {
        #region World Preferences


        /// <summary>
        /// Width of the world
        /// </summary>
        [Tooltip("Width of the world")]
        public int worldWidth = 100;//500;

        /// <summary>
        /// Height of the world
        /// </summary>
        [Tooltip("Height of the world")]
        public int worldHeight = 100;//800;

        /// <summary>
        /// Color of the world's border walls
        /// </summary>
        [Tooltip("Color of the world's border walls")]
        public Color worldBorderColor = Color.black;

        /// <summary>
        /// Thickness of the world's border walls
        /// </summary>
        [Tooltip("Thickness of the world's border walls")]
        public float borderWallThickness = 5.0f;
        #endregion

        #region Object Preferences

        /// <summary>
        /// Minimum size of an object in the world
        /// </summary>
        [Tooltip("Minimum size of an object in the world")]
        public int minObjectSize = 5;

        /// <summary>
        /// Maximum size of an object in the world
        /// </summary>
        [Tooltip("Maximum size of an object in the world")]
        public int maxObjectSize = 50;

        /// <summary>
        /// Desired percent of the world that should be covered in objects (0.0 - 1.0)
        /// </summary>
        [Tooltip("Desired percent of the world that should be covered in objects (0.0 - 1.0)")]
        [Range(0, 1)]
        public float objectPercentDesired = 0.5f; //0.12f;

        /// <summary>
        /// Can the objects be rotated when spawned?
        /// </summary>
        [Tooltip("Can the objects be rotated when spawned?")]
        public bool allowObjectRotation = false;

        /// <summary>
        /// Should each object be given a random color?
        /// </summary>
        [Tooltip("Should each object be given a random color?")]
        public bool randomObjectColors = false;

        /// <summary>
        /// If not random colors, what are the possible colors for the objects. (The objects will be given a random color from this list)
        /// </summary>
        [Tooltip("If not random colors, what are the possible colors for the objects. (The objects will be given a random color from this list)")]
        public List<Color> objectColors = new List<Color>() { Color.white };
        #endregion


        /// <summary>
        /// Based on the hypotenuse length of the world, the Start and Goal locations should be at least this percentage distance of the hypotenuse apart from each other (0.0 - 1.0)
        /// </summary>
        [Tooltip("Based on the hypotenuse length of the world, the Start and Goal locations should be at least this percentage distance of the hypotenuse apart from each other (0.0 - 1.0)")]
        [Range(0, 1)]
        public float distancePercent = 0.6f;

        public override string ToString()
        {
            
            return "\n" +
                "Width:\t\t" + worldWidth.ToString() + "\n" +
                "Height:\t\t" + worldHeight.ToString() + "\n" +
                "allowObjectRotation: " + allowObjectRotation.ToString() + "\n";
        }
    }


    /// <summary>
    /// Responsible for spawning a boarder wall around the play area
    /// </summary>
    /// <param name="wallPrefab">The prefab for the wall</param>
    /// <param name="worldContainer">The parent container for the wall gameobjects </param>
    /// <param name="worldDims">The dimensions of the world in World Units</param>
    /// <param name="origin">The centerpoint of the world</param>
    /// <param name="boarderColor">The color for the boarder walls</param>
    bool SpawnWorldWalls(GameObject wallPrefab, Transform worldContainer, Rect worldDims, Vector3 origin, Color boarderColor)
    {
        GameObject tmp;
        SpriteRenderer sr;

        float wallWidth = borderWallThickness;

        // Left Wall
        tmp = GameObject.Instantiate(wallPrefab, worldContainer);
        tmp.name = "Left Wall";
        tmp.transform.localScale = new Vector3(wallWidth, worldDims.height, 1);
        tmp.transform.position = new Vector3((-worldDims.width / 2) - (wallWidth/2), 0, 0) + origin;
        sr = tmp.GetComponent<SpriteRenderer>();
        sr.color = boarderColor;
        // Right Wall
        tmp = GameObject.Instantiate(wallPrefab, worldContainer);
        tmp.name = "Right Wall";
        tmp.transform.localScale = new Vector3(wallWidth, worldDims.height, 1);
        tmp.transform.position = new Vector3((worldDims.width / 2) + (wallWidth / 2), 0, 0) + origin;
        sr = tmp.GetComponent<SpriteRenderer>();
        sr.color = boarderColor;
        // Top Wall
        tmp = GameObject.Instantiate(wallPrefab, worldContainer);
        tmp.name = "Top Wall";
        tmp.transform.localScale = new Vector3(worldDims.width + (wallWidth * 2), wallWidth, 1);
        tmp.transform.position = new Vector3(0, (worldDims.height / 2) + (wallWidth / 2), 0) + origin;
        sr = tmp.GetComponent<SpriteRenderer>();
        sr.color = boarderColor;
        // Bottom Wall
        tmp = GameObject.Instantiate(wallPrefab, worldContainer);
        tmp.name = "Bottom Wall";
        tmp.transform.localScale = new Vector3(worldDims.width + (wallWidth * 2), wallWidth, 1);
        tmp.transform.position = new Vector3(0, (-worldDims.height / 2) - (wallWidth / 2), 0) + origin;
        sr = tmp.GetComponent<SpriteRenderer>();
        sr.color = boarderColor;

        return true;
    }


    bool SpawnStartAndGoalPoints(GameObject startPrefab, GameObject goalPrefab, Transform worldContainer, Rect worldDims, Vector3 origin)
    {
        float originalDesiredDistancePercent = distancePercent;

        // Find a position for the start point
        StartPoint = GameObject.Instantiate(startPrefab);

        Vector3 pos = new Vector3((int)Random.Range((-worldDims.width / 2) + (startPrefab.transform.localScale.x / 2), (worldDims.width / 2) - (startPrefab.transform.localScale.x / 2)),
                                             (int)Random.Range((-worldDims.height / 2) + (startPrefab.transform.localScale.y / 2), (worldDims.height / 2) - (startPrefab.transform.localScale.y / 2)), 0) + origin;
        if ((StartPoint.transform.localScale.x % 2 == 0 && worldDims.width % 2 != 0) || (StartPoint.transform.localScale.x % 2 != 0 && worldDims.width % 2 == 0))
        {
            if (pos.x + 0.5f + (startPrefab.transform.localScale.x / 2) <= origin.x + (worldDims.x / 2))
            {
                pos.x += 0.5f;
            }
            else
            {
                pos.x -= 0.5f;
            }
        }
        if ((StartPoint.transform.localScale.y % 2 == 0 && worldDims.height % 2 != 0) || (StartPoint.transform.localScale.y % 2 != 0 && worldDims.height % 2 == 0))
        {
            if (pos.x + 0.5f + (startPrefab.transform.localScale.x / 2) <= origin.x + (worldDims.x / 2))
            {
                pos.x += 0.5f;
            }
            else
            {
                pos.x -= 0.5f;
            }
        }

        StartPoint.transform.position = pos;
        StartPoint.name = "Start Point";
        StartPoint.transform.parent = worldContainer;
        startLocationSpawned = true;

        int attempts = 0;

        // Find a position for the goal point
        Vector3 proposedGoalPos;
        do
        {
            attempts++;
            if (attempts == 1000)
            {
                attempts = 0;
                distancePercent -= 0.05f;
                Debug.LogWarning("Desired distance between start and goal couldn't be met. Reducing distance.");
            }
            minDistanceBetweenStartAndGoal = Mathf.Sqrt(((worldDims.height * worldDims.height) + (worldDims.width * worldDims.width))) * distancePercent;

            proposedGoalPos = new Vector3((int)Random.Range((-worldDims.width / 2) + (goalPrefab.transform.localScale.x / 2), (worldDims.width / 2) - (goalPrefab.transform.localScale.x / 2)),
                                             (int)Random.Range((-worldDims.height / 2) + (goalPrefab.transform.localScale.y / 2), (worldDims.height / 2) - (goalPrefab.transform.localScale.y / 2)), 0) + origin;
        } while (Vector3.Distance(StartPoint.transform.position, proposedGoalPos) < minDistanceBetweenStartAndGoal);

        GoalPoint = GameObject.Instantiate(goalPrefab);
        if ((GoalPoint.transform.localScale.x % 2 == 0 && worldDims.width % 2 != 0) || (GoalPoint.transform.localScale.x % 2 != 0 && worldDims.width % 2 == 0))
        {
            if (proposedGoalPos.x + 0.5f + (startPrefab.transform.localScale.x / 2) <= origin.x + (worldDims.x / 2))
            {
                proposedGoalPos.x += 0.5f;
            }
            else
            {
                proposedGoalPos.x -= 0.5f;
            }
        }
        if ((GoalPoint.transform.localScale.y % 2 == 0 && worldDims.height % 2 != 0) || (GoalPoint.transform.localScale.y % 2 != 0 && worldDims.height % 2 == 0))
        {
            if (proposedGoalPos.y + 0.5f + (startPrefab.transform.localScale.y / 2) <= origin.y + (worldDims.y / 2))
            {
                proposedGoalPos.y += 0.5f;
            }
            else
            {
                proposedGoalPos.y -= 0.5f;
            }
        }

        GoalPoint.transform.position = proposedGoalPos;
        GoalPoint.name = "Goal Point";
        GoalPoint.transform.parent = worldContainer;
        goalLocationSpawned = true;

        actualDistanceBetweenStartAndGoal = Vector3.Distance(StartPoint.transform.position, GoalPoint.transform.position);

        distancePercent = originalDesiredDistancePercent;

        return startLocationSpawned && goalLocationSpawned;
    }

    /// <summary>
    /// Responsible for spawning obstacles around the play area
    /// </summary>
    /// <param name="objPrefab">The prefab for the objectes</param>
    /// <param name="worldContainer">The parent container for the wall gameobjects</param>
    /// <param name="worldDims">The dimensions of the world in World Units</param>
    /// <param name="origin">The centerpoint of the world</param>
    /// <returns></returns>
    IEnumerator SpawnObjects(GameObject objPrefab, Transform worldContainer, Rect worldDims, Vector3 origin)
    {
        int nameIndex = 0;
        int objectSpawnAttempts = 0;
        // Try to spawn enough objects to reach the desired square-footage of spawned objects. 
        // Note, if we fail more than 1000 times, we assume we can't meet the desired square-footage.
        // We'll just give up and move on
        while (objectPercentActual < objectPercentDesired && objectSpawnAttempts <= 1000)
        {
            GameObject tmp;
            SpriteRenderer sr;
            BoxCollider2D bc;
            ContactFilter2D filter = new ContactFilter2D();
            filter.NoFilter();

            filter.SetLayerMask(LayerMask.GetMask("Default"));

            // Generate a *potential* object
            tmp = GameObject.Instantiate(objPrefab);
            //tmp.transform.localScale = new Vector2(Random.Range(minObjectSize, maxObjectSize), Random.Range(minObjectSize, maxObjectSize));

            sr = tmp.GetComponent<SpriteRenderer>();
            sr.size = new Vector2(Random.Range(minObjectSize, maxObjectSize), Random.Range(minObjectSize, maxObjectSize));

            bc = tmp.GetComponent<BoxCollider2D>();
            bc.size = sr.size;

            Vector3 pos = new Vector3((int)Random.Range(-worldDims.width / 2, worldDims.width / 2), (int)Random.Range(-worldDims.height / 2, worldDims.height / 2), 0);
            if ((/*tmp.transform.localScale.x*/ sr.size.x % 2 == 0 && worldDims.width % 2 != 0) || (/*tmp.transform.localScale.x*/ sr.size.x % 2 != 0 && worldDims.width % 2 == 0))
            {
                if (pos.x + 0.5f + (/*tmp.transform.localScale.x*/ sr.size.x / 2) <= origin.x + (worldDims.x / 2))
                {
                    pos.x += 0.5f;
                }
                else
                {
                    pos.x -= 0.5f;
                }
            }
            if ((/*tmp.transform.localScale.y*/ sr.size.y % 2 == 0 && worldDims.height % 2 != 0) || (/*tmp.transform.localScale.y*/ sr.size.y % 2 != 0 && worldDims.height % 2 == 0))
            {
                if (pos.y + 0.5f + (/*tmp.transform.localScale.y*/ sr.size.y / 2) <= origin.y + (worldDims.y / 2))
                {
                    pos.y += 0.5f;
                }
                else
                {
                    pos.y -= 0.5f;
                }
            }

            tmp.transform.position = pos + origin;
            if (allowObjectRotation)
            {
                float rotation = Random.Range(0, 360);
                rotation = (int)(rotation / 10.0f);
                rotation = rotation * 10.0f;

                tmp.transform.Rotate(Vector3.forward, rotation);
            }
            yield return new WaitForFixedUpdate(); // Wait for the next fixed update to ensure that our new *potential* object is properly loaded into the physics system

            // Check to see if our *potential* object is colliding with anything already in the world
            Collider2D[] hits = new Collider2D[1];
            if (Physics2D.OverlapCollider(tmp.GetComponent<Collider2D>(), filter, hits) != 0)
            {
                // If the object is colliding with something already in the world, destroy it. Our attempt has failed.
                objectSpawnAttempts++;
                Destroy(tmp);
            }
            else
            {
                Debug.Log("Object Size: " + sr.size);
                // The object has found a proper spot in the world! Finalize the object
                nameIndex++;
                tmp.transform.parent = worldContainer;
                tmp.name = "Object " + nameIndex.ToString("D3");
                //sr = tmp.GetComponent<SpriteRenderer>();

                if (randomObjectColors)
                {
                    sr.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                }
                else
                {
                    int index = Random.Range(0, objectColors.Count);
                    Debug.Log("ObjectColors Count: " + objectColors.Count);
                    Debug.Log("Index: " + index);
                    sr.color = objectColors[index];
                }

                objectArea += sr.size.x * sr.size.y; /*(tmp.transform.localScale.x * tmp.transform.localScale.y);*/
                objectPercentActual = objectArea / worldArea;
            }
            yield return new WaitForFixedUpdate();
        }

        if (objectSpawnAttempts >= 1000)
        {
            Debug.LogWarning("Couldn't spawn as many objects as desired. Desired: " + (objectPercentDesired * 100) + "% -- Acutal: " + (objectPercentActual * 100) + "%");
        }
        objectPercentActual = objectArea / worldArea;

        objectsSpawned = true;
        yield return new WaitForFixedUpdate();
    }

    public void SpawnWorld(WorldSettings settings)
    {
        if (spawning)
        {
            Debug.LogWarning("WorldSpawner: Received a spawn request while attempting to spawn a world. Ignoring request.");
            spawnNow = false;

            LevelReadyArgs retVal = new LevelReadyArgs();
            retVal.World = null;
            retVal.SpawnTime = 0f;
            retVal.Error = true;
            retVal.ErrorMessage = "Spawn requested while the world was already spawning. Ignoring request.";
            OnLevelReadyEvent?.Invoke(this, retVal);
            return;
        }

        allowObjectRotation = settings.allowObjectRotation;
        // Set the border wall thickness and make sure it's greater than 1
        borderWallThickness = settings.borderWallThickness;
        borderWallThickness = Mathf.Max(1.0f, borderWallThickness);
        // Set the distance percentage and make sure it's between 0 and 1
        distancePercent = settings.distancePercent;
        distancePercent = Mathf.Max(0.0f, distancePercent);
        distancePercent = Mathf.Min(1.0f, distancePercent);
        // Set the max and min object sizes and make sure they are larger than 1 and make sure min <= max (otherwise, swap them)
        maxObjectSize = settings.maxObjectSize;
        maxObjectSize = Mathf.Max(1, maxObjectSize);
        minObjectSize = settings.minObjectSize;
        minObjectSize = Mathf.Max(1, minObjectSize);
        if (minObjectSize > maxObjectSize)
        {
            int tmp = minObjectSize;
            minObjectSize = maxObjectSize;
            maxObjectSize = tmp;
        }
        // Set the max and min world sizes and make sure they are larger than 1 and make sure min <= max (otherwise, swap them)
        worldHeight = settings.worldHeight;
        worldHeight = Mathf.Max(1, worldHeight);
        worldWidth = settings.worldWidth;
        worldWidth = Mathf.Max(1, worldWidth);

        if (settings.objectColors != null && settings.objectColors.Count > 0)
        {
            objectColors = settings.objectColors;
        }
        // Set the object percentage and make sure it's between 0 and 1
        objectPercentDesired = settings.objectPercentDesired;
        objectPercentDesired = Mathf.Max(0.0f, objectPercentDesired);
        objectPercentDesired = Mathf.Min(1.0f, objectPercentDesired);
        randomObjectColors = settings.randomObjectColors;
        if (settings.worldBorderColor != null)
        {
            worldBorderColor = settings.worldBorderColor;
        }

        

        spawnNow = true;
    }


    void ConfigureGrid()
    {

        grid = new Grid<GridObject>(worldWidth, worldHeight, 1f, transform.position - new Vector3(worldWidth / 2f, worldHeight / 2f, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g,x,y));
        for (int h = grid.GetHeight() - 1; h >= 0; h--)
        {
            for (int w = 0; w < grid.GetWidth(); w++)
            {

                if (Physics2D.OverlapBox(grid.GetWorldCenterPosition(w, h), new Vector2(0.4f, 0.4f), 0, LayerMask.GetMask("Default")) == null)
                {
                    //Debug.Log(grid.GetWorldCenterPosition(w, h) + " -- Nothing Hit");
                    // No Object at this position. Mark the grid as 'true' for passible 
                    GridObject go = grid.GetGridObject(w, h);
                    go.Walkable = true;
                }
                else
                {
                    //Debug.Log(grid.GetWorldCenterPosition(w, h) + " -- Hit");
                    // We hit an object at this position. Mark the grid as 'false' for not passible
                    GridObject go = grid.GetGridObject(w, h);
                    go.Walkable = false;
                }
            }
        }


        // Spew some ascii art of the level layout to the debug console
        string resultsString = "Width: " + worldWidth + " Height: " + worldHeight + "\n";
        for (int h = grid.GetHeight() - 1; h >= 0; h--)
        {
            for (int w = 0; w < grid.GetWidth(); w++)
            {
                resultsString += grid.GetGridObject(w, h).AsciiMapCharacter;
            }
            resultsString += "\n";
        }

        Debug.Log(resultsString);



        //walkableGrid = new Grid<bool>(worldWidth, worldHeight, 1f, transform.position - new Vector3(worldWidth/2f, worldHeight/2f, 0)/*new Vector3(-(worldWidth / 2), -(worldHeight / 2), 0)*/, (Grid<bool> g, int x, int y) => true);

        //for (int h = walkableGrid.GetHeight()-1; h >= 0; h--)
        //{
        //    for (int w = 0; w < walkableGrid.GetWidth(); w++) 
        //    {

        //        if (Physics2D.OverlapBox(walkableGrid.GetWorldCenterPosition(w, h), new Vector2(0.4f, 0.4f), 0, LayerMask.GetMask("Default")) == null)
        //        {
        //            //Debug.Log(grid.GetWorldCenterPosition(w, h) + " -- Nothing Hit");
        //            // No Object at this position. Mark the grid as 'true' for passible 
        //            walkableGrid.SetGridObject(w, h, true);
        //        }
        //        else
        //        {
        //            //Debug.Log(grid.GetWorldCenterPosition(w, h) + " -- Hit");
        //            // We hit an object at this position. Mark the grid as 'false' for not passible
        //            walkableGrid.SetGridObject(w, h, false);
        //        }
        //    }
        //}


        //// Spew some ascii art of the level layout to the debug console
        //string walkingResultsString = "Width: " + worldWidth + " Height: " + worldHeight + "\n";
        ////for (int h = grid.GetHeight() - 1; h >= 0; h--)
        ////{
        ////    for (int w = 0; w < grid.GetWidth(); w++)
        ////    {
        ////        if (grid.GetGridObject(w, h))
        ////        {
        ////            resultsString += "_";
        ////        }
        ////        else
        ////        {
        ////            resultsString += "X";
        ////        }
        ////    }
        ////    resultsString += "\n";
        ////}

        //Debug.Log(walkingResultsString);

    }

    IEnumerator SpawnNow()
    {
        transform.position = new Vector3(worldWidth / 2f, worldHeight / 2f, 0);

        float startTime = Time.unscaledTime;

        Time.timeScale = 100;

        // Cleanup the old world and generate the specs for the new one
        if (worldHolder != null)
        {
            Destroy(worldHolder.gameObject);
        }
        worldHolder = new GameObject("World Holder").transform;
        worldHolder.transform.position = Vector3.zero;
        worldHolder.parent = gameObject.transform;
        worldSize = new Rect(0, 0, worldWidth, worldHeight);
        //worldSize = new Rect(0, 0, Random.Range(minSize, maxSize), Random.Range(minSize, maxSize));
        worldArea = worldSize.height * worldSize.width;
        objectArea = 0;
        objectPercentActual = 0;

        startLocationSpawned = false;
        goalLocationSpawned = false;
        objectsSpawned = false;
        worldBoarderSpawned = false;

        // Spawn the boundaries of the world
        worldBoarderSpawned = SpawnWorldWalls(wallPrefab, worldHolder, worldSize, gameObject.transform.position, worldBorderColor);
        yield return new WaitUntil(() => worldBoarderSpawned);

        // Spawn Starting Point and Goal
        SpawnStartAndGoalPoints(startPrefab, goalPrefab, worldHolder, worldSize, gameObject.transform.position);
        yield return new WaitUntil(() => (startLocationSpawned && goalLocationSpawned));

        // Spawn the objects (obstacles)
        StartCoroutine(SpawnObjects(objectPrefab,worldHolder,worldSize,gameObject.transform.position));
        yield return new WaitUntil(() => objectsSpawned);

        ConfigureGrid();

        Time.timeScale = 1;

        LevelReadyArgs retVal = new LevelReadyArgs();
        retVal.World = gameObject.GetComponent<WorldSpawner>();
        retVal.SpawnTime = Time.unscaledTime - startTime;
        retVal.Error = false;
        retVal.ErrorMessage = "";
        OnLevelReadyEvent?.Invoke(this, retVal);
        Debug.Log("Level Ready: " + retVal.SpawnTime.ToString("N2") + "s");
        spawning = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        if (startPrefab == null)
        {
            startPrefab = new GameObject();
            Debug.LogWarning("WorldSpawner - startPrefab not set");
        }

        if (goalPrefab == null)
        {
            goalPrefab = new GameObject();
            Debug.LogWarning("WorldSpawner - goalPrefab not set");
        }

        if (objectPrefab == null)
        {
            objectPrefab = new GameObject();
            Debug.LogWarning("WorldSpawner - objectPrefab not set");
        }

        if (wallPrefab == null)
        {
            wallPrefab = new GameObject();
            Debug.LogWarning("WorldSpawner - wallPrefab not set");
        }

    }

    // Update is called once per frame
    void Update()
    {
        // If the world is already spawning and we're asked to spawn, throw out the spawn request
        if (spawning)
        {
            Debug.LogWarning("WorldSpawner: Received a spawn request while attempting to spawn a world. Ignoring request.");
            spawnNow = false;
        }

        if (spawnNow)
        {
            spawnNow = false;

            StartCoroutine(SpawnNow());
        }

        if (drawDebugGrid)
        {
            //            walkableGrid.DrawDebug(1);
            grid.DrawDebug(1);
        }

    }


    //List<Vector2> FreeSquares(Vector2 origin, Vector2 dim)
    //{
    //    List<Vector2> retVal = new List<Vector2>();

    //    for (int y = (int)(origin.y - (dim.y/2)); y > (int)(origin.y + (dim.y / 2)); y++)
    //    {
    //        for (int x = (int)(origin.x - (dim.x / 2)); y > (int)(origin.x + (dim.x / 2)); x++)
    //        {
    //            grid.GetGridObject()
    //        }

    //    }


    //    return retVal;
    //}
}
