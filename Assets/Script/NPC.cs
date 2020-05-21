using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;


public class NPC : MonoBehaviour
{
    #region attributes_stealth

    [Header("Stealth")]
    [SerializeField]
    private float m_angleView = 120;
    [SerializeField]
    private float m_distanceView = 10;
    [SerializeField]
    private float m_secondWarningLimit = 1.5f;
    [SerializeField]
    private float m_secondAlertLimit = 3.0f;
    [SerializeField]
    private float m_enterInDetectionZone;
    [SerializeField]
    private float m_secondInDetectionZone;
    [SerializeField]
    private float m_exitInDetectionZone;
    [SerializeField]
    private float m_secondSinceExitInDetectionZone;
    [SerializeField]
    private bool m_isInWarning = false;
    [SerializeField]
    private bool m_isInAlert = false;
    [SerializeField]
    private bool m_debugRay = false;
    #endregion

    #region attributes_agenda
    [Header("Agenda")]
    [SerializeField]
    private bool m_autoSetRoute = false;
    [SerializeField]
    private Task[] m_taskVector;
    [SerializeField]
    private int m_currentTaskIndex;
    private UnityEngine.AI.NavMeshAgent m_agent;
    private int m_lengthTaskVector;
    //This attributes are necessary to handle loop into the day
    private Task m_taskBeginLoop;
    private Task m_taskEndLoop;
    private short m_isInloop=0;
    private TimeInGame m_timeAtEndOfOneTaskLoop;
    private bool m_actionIsDown = false;
    private Transform m_saveLastTransform;
    private Transform m_saveSecondToLastTransform;
    private Transform m_saveFirstTransform;
    #endregion

    #region attributes_pocket
    [SerializeField]
    private List<GameObject> m_inventoryGO;
    private List<Pair<Items, bool>> m_inventory = new List<Pair<Items, bool>>();
    #endregion


    #region attributes_knowledge
    [SerializeField]
    private List<GameObject> m_knowledgeGO;
    private List<Knowledge> m_knowledge = new List<Knowledge>();
    #endregion

    #region attributes_task
    [SerializeField]
    private bool m_stopClassicalSchedule = false;
    #endregion

    #region getter
    public float EnterInDetectionZone { get => m_enterInDetectionZone; set => m_enterInDetectionZone = value; }
    public float SecondInDetectionZone { get => m_secondInDetectionZone; set => m_secondInDetectionZone = value; }
    public float ExitInDetectionZone { get => m_exitInDetectionZone; set => m_exitInDetectionZone = value; }
    public bool StopClassicalSchedule { get => m_stopClassicalSchedule; set => m_stopClassicalSchedule = value; }
    #endregion

    void Start()
    {
        checkAndSetKnowledge();
        checkAndSetInventory();

        m_lengthTaskVector = m_taskVector.Length;

        if (m_autoSetRoute)
            setRoute();

        if (m_lengthTaskVector == 0)
        {
            Debug.LogWarning("No task have been set on " + gameObject.name);
            return;
        }

        m_agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        m_taskVector[0].m_index = 0;
        if (!m_taskVector[0].m_loop)
        {
            if(m_lengthTaskVector > 1)
                m_taskVector[0].m_durationTask = m_taskVector[1].m_timeOfBeginTask - m_taskVector[0].m_timeOfBeginTask;
            else
                m_taskVector[0].m_durationTask = new TimeInGame(0, 24, 0);
        }

        TimeInGame currentTime = new TimeInGame(0, GameManager.Instance.CurrentTimeInGame.HoursG, GameManager.Instance.CurrentTimeInGame.MinutesG);
        for (int i = 1; i < m_lengthTaskVector; i++)
        {
            //define index task
            m_taskVector[i].m_index = i;
            //define duration task
            if (!m_taskVector[i].m_loop)
            {
                if (i == m_taskVector.Length - 1)
                    m_taskVector[i].m_durationTask = new TimeInGame(0, 24, 00) - m_taskVector[i].m_timeOfBeginTask + m_taskVector[0].m_timeOfBeginTask;
                else
                    m_taskVector[i].m_durationTask = m_taskVector[i+1].m_timeOfBeginTask - m_taskVector[i].m_timeOfBeginTask;            
            }
            //define where the task index begin
            if (currentTime <= m_taskVector[i].m_timeOfBeginTask && currentTime >= m_taskVector[i-1].m_timeOfBeginTask)
            {
                m_currentTaskIndex = i-1;
            }
        }
        m_saveSecondToLastTransform = GameObject.Find("Game").GetComponent<Transform>();
        m_saveLastTransform = GameObject.Find("Player").GetComponent<Transform>();

        

        if (!checkCoherencyTaskVector())
            Debug.LogWarning("Task aren't well defined, there might be incoherencies");

        
    }

    void Update()
    {
        if (m_stopClassicalSchedule)
            m_agent.destination = transform.position;
        else
            setTask();
    }
    
    //Could be optimize
    public bool setRoute()
    {

        string myName = gameObject.name;
        int nbrTaskRoute = 0;
        foreach (GameObject taskRoute in GameObject.FindGameObjectsWithTag("Route"))
        {
            string nameTaskRoute = taskRoute.name;
            if (nameTaskRoute.Contains("R-"+myName))
            {
                //print(nameTaskRoute);
                nbrTaskRoute++;
                if (nbrTaskRoute > m_lengthTaskVector)
                {
                    Debug.LogError("nbrTaskRoute>= LengthTaskVector - Please change the size of TaskVector.");
                    return false;
                }
                    
                else
                {
                    string[] nameTaskRouteSplit = nameTaskRoute.Split('-');
                    if (nameTaskRouteSplit.Length < 6)
                    {
                        Debug.LogError("Couldn't finish auto set route, because task route name:" + nameTaskRoute + "isn't well defined");
                        return false;
                    }
                        
                    string actionStr = nameTaskRouteSplit[4];
                    string HoursStr = nameTaskRouteSplit[3];

                    m_taskVector[nbrTaskRoute - 1].m_timeOfBeginTask = TimeInGame.strInHoursToTimeInGame(HoursStr);
                    enumAction actionTaskRoute = (enumAction)System.Enum.Parse(typeof(enumAction), actionStr);  
                    m_taskVector[nbrTaskRoute - 1].m_actionTask = actionTaskRoute;
                    m_taskVector[nbrTaskRoute - 1].m_goal = taskRoute.GetComponent<Transform>();
                    bool loop;
                    if (!bool.TryParse(nameTaskRouteSplit[5], out loop))
                        Debug.LogError("Unable to convert string" + nameTaskRouteSplit[5] + "to bool");
                    m_taskVector[nbrTaskRoute - 1].m_loop = loop;
                    if(bool.Parse(nameTaskRouteSplit[5]) && nameTaskRouteSplit.Length==7)
                        m_taskVector[nbrTaskRoute - 1].m_durationTask = TimeInGame.strInHoursToTimeInGame(nameTaskRouteSplit[6]);

                }
            }
        }
        if (nbrTaskRoute == 0)
        {
            Debug.LogError("Didn't find any gameObject to define the route");
            return false;
        }
            

        return true;
    }

    public bool checkCoherencyTaskVector()
    {
        if (m_lengthTaskVector < 1)
        {
            Debug.LogError("Length task vector not possible");
            return false;
        }
            
        short nbrOfLoopBool = 0;
        for(int i=0;i< m_lengthTaskVector-1;i++)
        {
            if (!m_taskVector[i].checkCoherencyTask())
                return false;
            //If the time of the i-th task is > i+1-th task
            if (m_taskVector[i].m_timeOfBeginTask > m_taskVector[i + 1].m_timeOfBeginTask)
            {
                Debug.LogError("The time (" + m_taskVector[i].m_timeOfBeginTask.ToString() + ") of the " +
                    i.ToString() + "th task is greater than the time (" + m_taskVector[i + 1].m_timeOfBeginTask.ToString() + ") of the " +
                    (i + 1).ToString() + "th task.");
                return false;
            }
                
            //If the duration task is well parametrized
            if(!m_taskVector[i].m_loop && m_taskVector[i].m_durationTask!= m_taskVector[i+1].m_timeOfBeginTask - m_taskVector[i].m_timeOfBeginTask)
            {
                Debug.LogError("The time (" + m_taskVector[i].m_durationTask.ToString() + ") of the " +
                    i.ToString() + "th duration task is weird");
                return false;
            }
                
            if (m_taskVector[i].m_loop)
            {
                nbrOfLoopBool++;
                if (m_taskVector[i].m_durationTask == new TimeInGame(0, 0, 0))
                {
                    Debug.LogError("The duration task of the " + i.ToString() + "-th task is not set, " +
                        "yet it has been defined to loop task");
                    return false;
                }
                if(m_taskVector[i].m_durationTask == m_taskVector[i + 1].m_timeOfBeginTask - m_taskVector[i].m_timeOfBeginTask && nbrOfLoopBool%2==0)
                {
                    Debug.LogError("The duration task of the " + i.ToString() + "-th task is :"
                        + m_taskVector[i].m_durationTask.ToString() + "yet it has been defined to loop task." +
                        " But with this the loop won't buckle.");
                    return false;
                }
            }
        }
        //to handle the last task (which isn't handled in the loop)
        if(m_taskVector[m_lengthTaskVector-1].m_loop)
        {
            nbrOfLoopBool++;
            if (m_taskVector[m_lengthTaskVector - 1].m_durationTask == new TimeInGame(0, 0, 0))
            {
               Debug.LogError("The duration task of the las task is not set, " +
                    "yet it has been defined to loop task");
                return false;
            }
                
        }

        if(nbrOfLoopBool!=0)
        {
            if (nbrOfLoopBool % 2 != 0)
            {
                Debug.LogWarning(gameObject.name + " - They are " + nbrOfLoopBool.ToString() + " loop checked, there must be an even number." +
                    "A loop is open by the first task checked and close by the last task. checked");
                return false;
            }
        }
        return true;
    }

    public void checkAndSetInventory()
    {
        //all the gameobjects in the list must have a daughter of the Items class
        if (m_inventoryGO != null)
        {
            foreach (GameObject itemGO in m_inventoryGO)
            {
                if (itemGO.TryGetComponent(out Items item))
                {
                    Pair<Items, bool> itemHave =new Pair<Items,bool>(item, true);
                    m_inventory.Add(itemHave);
                }
                else
                    Debug.LogWarning("In Knowledge there is an invalid gameobject which dont have any Knowledge script attached");
            }
        }
    }

    public void checkAndSetKnowledge()
    {
        //all the gameobjects in the list must have a daughter of the Knowledge class
        if (m_knowledgeGO != null)
        {
            foreach (GameObject knowledgeGO in m_knowledgeGO)
            {
                if (knowledgeGO.TryGetComponent(out Knowledge knowledge))
                    m_knowledge.Add(knowledge);
                else
                    Debug.LogWarning("In Knowledge there is an invalid gameobject which dont have any Knowledge script attached");
            }
        }
    }

    public void setTask()
    {
        m_lengthTaskVector = m_taskVector.Length;

        /*Using CurrentTimeInGame cause trouble with comparaison.
        *Indeed, the purpose of this, is to have a loop of task 
        * to do each day with a model of one day, therefore 
        * we have to "make a modulo 1 day" of the time.*/
        TimeInGame currentTime = new TimeInGame(0, GameManager.Instance.CurrentTimeInGame.HoursG, GameManager.Instance.CurrentTimeInGame.MinutesG);
        if(m_lengthTaskVector == 0)
        {
            Debug.LogWarning("No task have been set");
            return;
        }

        if (m_lengthTaskVector == 1)
        {
            executeAction(m_taskVector[m_currentTaskIndex].m_actionTask, m_taskVector[m_currentTaskIndex].m_goal);
            return;
        }

        if (m_isInloop!=2)
        {
            if (m_taskVector[m_currentTaskIndex].m_loop == true && m_isInloop != 1)
            {
                m_isInloop = 1;
                m_taskBeginLoop = m_taskVector[m_currentTaskIndex];
            }

                
                
            //If it's the last action
            if (m_currentTaskIndex + 1 == m_lengthTaskVector)
            {
                if (m_taskVector[m_currentTaskIndex].m_loop == true && m_taskVector[m_currentTaskIndex] != m_taskBeginLoop && m_isInloop == 1)
                {
                    m_taskEndLoop = m_taskVector[m_currentTaskIndex];
                    m_timeAtEndOfOneTaskLoop = currentTime;
                    m_isInloop = 2;
                }
                /*We check if we are not between the task 0 and 1 
                 * we can access to TaskVector[1] because we have check above
                 * that the vector have more than one element. 
                 */
                if (!(currentTime > m_taskVector[0].m_timeOfBeginTask && currentTime < m_taskVector[1].m_timeOfBeginTask))
                {
                    if (m_actionIsDown)
                        return;
                    else
                        executeAction(m_taskVector[m_currentTaskIndex].m_actionTask, m_taskVector[m_currentTaskIndex].m_goal);
                }
                else
                {
                    m_currentTaskIndex = 0;
                    m_actionIsDown = false; 
                }
            }
            //Check if we are out of range
            else if (m_currentTaskIndex + 1 > m_lengthTaskVector)
            {
                print("Error - Index of the current task out of range");
                m_currentTaskIndex = 0;
            }
            //For a "normal" Task
            else
            {

                //TaskVector[CurrentTaskIndex + 2] could be at most the last task (see check above)
                if (m_taskVector[m_currentTaskIndex].m_loop == true && m_taskVector[m_currentTaskIndex] != m_taskBeginLoop && m_isInloop==1)
                {
                    m_taskEndLoop = m_taskVector[m_currentTaskIndex];
                    m_timeAtEndOfOneTaskLoop = currentTime;
                    m_isInloop = 2;
                    print("TaskEndLoop=" + m_currentTaskIndex);
                }
                    
                //while we are between two task A and B execute the task A
                if (currentTime <= m_taskVector[m_currentTaskIndex + 1].m_timeOfBeginTask)
                {
                    if (m_actionIsDown)
                        return;
                    else
                        executeAction(m_taskVector[m_currentTaskIndex].m_actionTask, m_taskVector[m_currentTaskIndex].m_goal);
                }
                //else add one to the index of the current task to execute B, and so on...
                else
                { 
                    m_currentTaskIndex++;
                    m_actionIsDown = false;
                }
            }
        }
        else
        {
            //#################     check end loop     ##############
            //case to end the loop if the end task of the loop is not the last task
            if (m_taskEndLoop != m_taskVector[m_lengthTaskVector -1])
            {
                
                if (currentTime >= m_taskVector[m_taskEndLoop.m_index + 1].m_timeOfBeginTask)
                {
                    m_isInloop = 0;
                    m_currentTaskIndex = m_taskEndLoop.m_index + 1;
                }
            }
            //In the case of the end task of the loop is the last task, is an infinite loop


            /*  If we are in last task of the loop we check that we have passed 
             *  the duration of the last task then we go to the first task of 
             *  the loop. Else we execute this action
             * */
            if (m_currentTaskIndex== m_taskEndLoop.m_index)
            {
                if (currentTime - m_timeAtEndOfOneTaskLoop > m_taskEndLoop.m_durationTask)//(currentTime - TaskEndLoop.m_timeTask > TaskEndLoop.m_durationTask)
                {
                    m_actionIsDown = false;
                    m_currentTaskIndex = m_taskBeginLoop.m_index;
                    m_timeAtEndOfOneTaskLoop = currentTime;
                }
                else
                {
                    if (m_actionIsDown)
                        return;
                    else
                        executeAction(m_taskVector[m_currentTaskIndex].m_actionTask, m_taskVector[m_currentTaskIndex].m_goal);
                }
            }
            //If we are in another task of the loop
            else
            {
                /*If we have passed the duration of the current task, then we update 
                 * the m_timeAtEndOfOneTaskLoop and the currentTaskIndex
                 * */
                if(currentTime - m_timeAtEndOfOneTaskLoop > m_taskVector[m_currentTaskIndex + 1].m_durationTask)
                {
                    m_actionIsDown = false;
                    m_currentTaskIndex++;
                    m_timeAtEndOfOneTaskLoop = currentTime;
                }
                else
                {
                    if (m_actionIsDown)
                        return;
                    else
                        executeAction(m_taskVector[m_currentTaskIndex].m_actionTask, m_taskVector[m_currentTaskIndex].m_goal);
                }
            }
        }
    }

    public void walk(Transform goal)
    {
        ////////////////// Check coherency ////////////////////
        if (!m_actionIsDown)
        {
            int currentMinutes = (int)GameManager.Instance.CurrentTimeInGame.MinutesG;
            //each second save the position
            if (currentMinutes % 100 == 0)
                m_saveLastTransform = gameObject.transform;
            else if (currentMinutes % 10 == 75)
                m_saveSecondToLastTransform = gameObject.transform;
            
            //check if the last position saved and the current position isn't away...
            if(currentMinutes % 10 == 50)
            {
                if (Vector3.Distance(m_saveLastTransform.position, m_saveSecondToLastTransform.position) < 0.5f)
                {
                    //...then it end the animation
                    GetComponent<Animator>().SetBool("IsWalking", false);
                    m_actionIsDown = true;
                    Debug.LogError("The PNJ didn't move a lot since the last second, indeed the distance " +
                        "is :" + Vector3.Distance(m_saveLastTransform.position, gameObject.transform.position).ToString() +
                        " therefore we stop it.");
                    return;
                }
            }


            //check if the current position is incoherent...
            if (gameObject.transform.position.x > 1000f || gameObject.transform.position.y > 1000f || gameObject.transform.position.z > 1000f)
            {
                //...then it end the animation...
                GetComponent<Animator>().SetBool("IsWalking", false);
                m_actionIsDown = true;
                Debug.LogError("The PNJ is anormaly far away, indeed his position is:" + gameObject.transform.position.ToString() +
                    " therefore we stop it and set a new position at" + m_saveFirstTransform.position);
                //...moreover we set is position to the first position save and we set the task.
                gameObject.transform.position = m_saveFirstTransform.position;
                m_currentTaskIndex = 0;
                return;
            }
        }
        

        ////////////////// If it's ok, walk ////////////////////
        const float epsilon = 2f;
        //check first if we are near the goal, then it stop the walk and the action is down, else it continue to walk
        if (Utilities.isCloseEpsilonVec3(GetComponent<Transform>().position, goal.position, epsilon))
        {
            GetComponent<Animator>().SetBool("IsWalking", false);
            m_actionIsDown = true;
        }
        else
            GetComponent<Animator>().SetBool("IsWalking", true);
        
        if(m_stopClassicalSchedule == false)
            m_agent.destination = goal.position;
        else
            m_agent.destination = transform.position;

    }

    public bool playerIsSee()
    {

        Vector3 positionAI = gameObject.transform.position;
        Vector2 positionAI2D = new Vector2(positionAI.x, positionAI.z);

        Vector3 playerPosition = GameObject.Find("Player").GetComponent<Transform>().position;

        Vector3 vecAiToPlayer = playerPosition - positionAI;
        Vector2 vecAiToPlayer2D = new Vector2(vecAiToPlayer.x, vecAiToPlayer.z);

        Vector3 vecInFrontOfEye = transform.Find("FrontPoint").position - positionAI;
        Vector2 vecInFrontOfEye2D = new Vector2(vecInFrontOfEye.x, vecInFrontOfEye.z);

        float angle = Vector2.Angle(vecInFrontOfEye2D, vecAiToPlayer2D);
        float distance = Vector3.Distance(positionAI, playerPosition);

        if (angle <= m_angleView / 2 && distance < m_distanceView)
        {
            RaycastHit hit; ;
            if (Physics.Raycast(positionAI, vecAiToPlayer, out hit))
                if (hit.collider.gameObject.name == "Player")
                    return true;
        }
        return false;
    }

    public void debugSeeRayAi()
    {
        Vector3 positionAI = gameObject.transform.position;
        Vector2 positionAI2D = new Vector2(positionAI.x, positionAI.z);
        Vector3 inFrontOfEye = transform.Find("FrontPoint").position;
        Vector2 inFrontOfEye2D = new Vector2(inFrontOfEye.x, inFrontOfEye.z);
        Vector3 positionEye = new Vector3(positionAI.x, inFrontOfEye.y - 0.5f, positionAI.z);

        int nbrOfRayToSend = 1000;
        for (float theta = -m_angleView/2; theta<m_angleView/2;theta+=m_angleView/ nbrOfRayToSend)
        {
            Vector2 rotation2D = Utilities.rotate( positionAI2D, inFrontOfEye2D, theta, m_distanceView);
            //rotation2D *= 2;
            Vector3 rotation = new Vector3(rotation2D.x, inFrontOfEye.y - 0.5f, rotation2D.y);
            
            Debug.DrawLine(positionEye, rotation, Color.yellow, 0.01f);
        }
    }

    public void watch()
    {
        bool isPlayerSee = playerIsSee();

        if (isPlayerSee)
        {
            print("Je suis dedans");

            //GetComponent<Animator>().SetBool("IsWatching", true);
            if (SecondInDetectionZone >= m_secondWarningLimit)
            {
                m_isInWarning = true;
                transform.Find("CanvasAI/WarningAndAlert").GetComponent<Text>().text = "!";
            }

            if (SecondInDetectionZone >= m_secondAlertLimit)
            {
                m_isInAlert = true;
                transform.Find("CanvasAI/WarningAndAlert").GetComponent<Text>().color = new Color(1f, 0.1f, 0.1f);
                transform.Find("CanvasAI/WarningAndAlert").GetComponent<Text>().text = "!";
                GetComponent<Animator>().SetBool("GrabRifle", true);
                GameManager.Instance.gameOver();
            }
        }

        if (GameManager.Instance.DebugIsEnabled)
            if (m_debugRay)
                debugSeeRayAi();

    }

    public bool executeAction(enumAction actionToExecute, Transform goal)
    {
        if (Time.frameCount % 600 == 0)
            GetComponent<Animator>().SetInteger("RandomInt", Random.Range(0, 10));
        else
            GetComponent<Animator>().SetInteger("RandomInt", 0);


        switch (actionToExecute)
        {
            case enumAction.WALK:
                //print("I'm walking");
                walk(goal);
                break;
            case enumAction.WATCH:
                watch();
                break;
            case enumAction.SLEEP:
                GetComponent<Animator>().SetBool("IsSleeping", true);
                break;
            case enumAction.SIT:
                GetComponent<Animator>().SetBool("IsSitting", true);
                break;
            case enumAction.WALK_AND_WATCH:
                //print("I'm walking and watching");
                walk(goal);
                watch();
                break;
        }

        return true;
    }

    public string inventoryToString()
    {
        string itemsInInventoryStr = "";
        if (m_inventory != null)
        {
            Debug.Log(gameObject.name + " inventory contains :\n");
            foreach (Pair<Items, bool> itemsHave in m_inventory)
            {
                Debug.Log("Name : " + itemsHave.first.Name + ", id :" + itemsHave.first.Id + ", Is in pocket : " + itemsHave.second.ToString() + "\n");
            }
                
        }
        else
        {
            itemsInInventoryStr += "Inventory is empty\n";
        }
        return itemsInInventoryStr;
    }

    public string knowledgeToString()
    {
        string knowledgeString = "";
        if (m_knowledge != null)
        {
            Debug.Log(gameObject.name + " know :\n");
            foreach (Knowledge knowledge in m_knowledge)
            {
                Debug.Log(knowledge.Name + "\n");
            }
        }
        else
        {
            knowledgeString += "Doesnt have any knowledge\n";
        }
        return knowledgeString;
    }
    
    public void tryToOpenTheDoor(Door door)
    {  
        if(door.IsOpen == false)
        {
            StopClassicalSchedule = true;
            if (door.tryToOpenOrCloseDoor())
                StartCoroutine(waitDoorOpen(door));
            //if the door is lock
            else
            {
                if (m_knowledge != null)
                    foreach (Knowledge knowledge in m_knowledge)
                        if (knowledge is Password)
                        {
                            Password password = (Password)knowledge;
                            if (door.tryPassword(password.getPassword))
                                if (door.tryToOpenOrCloseDoor())
                                    StartCoroutine(waitDoorOpen(door));
                        }
                if (m_inventory != null)
                    foreach (Pair<Items,bool> item in m_inventory)
                        if (item.first is Key && item.second)
                        {
                            Key key = (Key)item.first;
                            if (key.DoorAssociated == door)
                                if (door.tryToOpenOrCloseDoor())
                                    StartCoroutine(waitDoorOpen(door));
                        }
            }
        }
    }

    public void tryToCloseTheDoor(Door door)
    {
        door.tryToOpenOrCloseDoor();
    }

    private IEnumerator waitDoorOpen(Door door)
    {
        while (door.IsOpen == false)
            yield return new WaitForSeconds(0.1f);
        StopClassicalSchedule = false;
    }
}




public enum enumAction
{
    IDLE,
    WALK,
    WATCH,
    WALK_AND_WATCH,
    SLEEP,
    SIT
}

[System.Serializable]
public class Task
{
    public bool m_loop=true;
    public TimeInGame m_timeOfBeginTask=new TimeInGame { };
    public enumAction m_actionTask;
    public int m_index;

    public Transform m_goal;
    //In case of a loop the task at the end of the loop don't have the same duration
    public TimeInGame m_durationTask = new TimeInGame { };


    public bool checkCoherencyTask()
    {
        if ((m_actionTask == enumAction.WALK || m_actionTask == enumAction.WALK_AND_WATCH) && m_goal == null)
        {
            Debug.LogError("The " + m_index + "-th task is walk or walk and watch, but the goal is not defined");
            return false;
        }
        return true;
    }
}
