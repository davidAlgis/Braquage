using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    // Start is called before the first frame update
    private float m_initialRotation;
    private float m_actualRotation;
    [SerializeField]
    private SlotTime[] m_lockSlotTimeVector;
    [SerializeField]
    private bool m_isOpen = false;
    [SerializeField]
    private bool m_isLock;
    [SerializeField]
    private float m_rotation = 110;


    #region attributes_digicode
    [Header("Digicode")]

    [SerializeField]
    private GameObject m_AssociatedDigicodeGameObject;
    [SerializeField]
    private string m_password;

    private Text m_textOnScreen;
    #endregion

    private void Start()
    {
        //If there is a digicode associated
        if (m_AssociatedDigicodeGameObject != null)
        {
            if (parametrizedDigicode() == false)
            {
                Debug.LogWarning("The Digicode couldn't be set properly, therefore the state of the door " + gameObject.name + " has been set to unlock.");
                m_isLock = false;
            }

        }



        //create the box collider, one for colision and one for trigger
        BoxCollider boxCol = gameObject.GetComponent(typeof(BoxCollider)) as BoxCollider;
        if (boxCol == null)
        {
            gameObject.AddComponent<BoxCollider>();
            boxCol = gameObject.AddComponent<BoxCollider>();
            boxCol.center = new Vector3(0.5f, 0f, 0f);
            boxCol.size = new Vector3(1.5f, 2f, 2f);
            boxCol.isTrigger = true;
        }

        m_initialRotation = transform.localRotation.eulerAngles.z;
    }

    private void Update()
    {
        m_isLock = isDoorLock();
        if (m_AssociatedDigicodeGameObject != null)
        {
            if (Vector3.Distance(m_AssociatedDigicodeGameObject.transform.position, GameObject.Find("Player").transform.position) < 3)
                Cursor.visible = true;
            print("a");
        }
    }
    private void OnTriggerEnter(Collider character)
    {
        if (character.tag == "NPC")
        {

            if (m_isOpen == false)
            {
                AI aiInContact = character.GetComponent<AI>();
                aiInContact.StopClassicalSchedule = true;
                //Transform goal = transform;
                //if(other.transform )
                //aiInContact.executeAction(enumAction.WALK, transform);
                openAndCloseDoor(character);
            }
        }

        if (character.tag == "Player")
            UIManager.Instance.enableUIPressButton(true, "F");
    }

    private void OnTriggerStay(Collider character)
    {

        if (character.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.F))
            {

                openAndCloseDoor(character);
                UIManager.Instance.enableUIPressButton(false);
            }
        }
    }

    private void OnTriggerExit(Collider character)
    {
        if (character.tag == "Player")
            UIManager.Instance.enableUIPressButton(false);

        if (character.tag == "NPC")
        {
            openAndCloseDoor(character);
        }
    }

    void openAndCloseDoor(Collider character)
    {
        print("open and close the door" + m_isLock);
        if (m_isLock)
        {

            UIManager.Instance.enableMessageBox("La porte est fermée à clé.");
        }
        else
        {
            m_actualRotation = transform.localRotation.eulerAngles.z;
            if (m_isOpen)
                StartCoroutine(rotateTo(m_initialRotation, character));
            else
                StartCoroutine(rotateTo(m_rotation, character));
        }

    }

    IEnumerator rotateTo(float breakRotation, Collider character)
    {
        //speed of rotation of the door
        float frequencyOfLoop = 0.0001f;
        uint nbrIteration = 0;
        float saveRotation = transform.localRotation.eulerAngles.z;

        if (transform.localRotation.eulerAngles.z < breakRotation)
        {
            //open the door
            while (transform.localRotation.eulerAngles.z <= breakRotation)
            {
                //avoid making loop
                if (nbrIteration > 720)
                {
                    //In this case we return to the initial position. 
                    transform.Rotate(0, 0, 360 - transform.localRotation.eulerAngles.z + m_initialRotation);
                    yield break;
                }

                transform.Rotate(0, 0, 0.5f);
                nbrIteration++;
                yield return new WaitForSeconds(frequencyOfLoop);
            }
            m_isOpen = true;
            if (character.tag == "NPC")
            {
                if (m_isOpen == true)
                    character.GetComponent<AI>().StopClassicalSchedule = false;
            }
        }
        else
        {
            //close the door
            while (transform.localRotation.eulerAngles.z >= breakRotation)
            {
                //If we want to rotate to 0 degree, we'll have an infinite loop, therefore we have to break it
                if (breakRotation == 0 && transform.localRotation.eulerAngles.z < 360 && transform.localRotation.eulerAngles.z > 358)
                {
                    transform.Rotate(0, 0, 1.0f);
                    m_isOpen = false;
                    yield break;
                }


                //avoid making loop
                if (nbrIteration > 720)
                {
                    //In this case we return to the initial position. 
                    transform.Rotate(0, 0, 360 - transform.localRotation.eulerAngles.z + m_initialRotation);
                    yield break;
                }

                transform.Rotate(0, 0, -0.5f);
                nbrIteration++;
                yield return new WaitForSeconds(frequencyOfLoop);
            }
            m_isOpen = false;
        }
    }

    public bool isDoorLock()
    {
        if (m_lockSlotTimeVector != null)
        {
            foreach (SlotTime slot in m_lockSlotTimeVector)
            {
                slot.to.DayG = GameManager.Instance.CurrentTimeInGame.DayG;
                slot.from.DayG = GameManager.Instance.CurrentTimeInGame.DayG;

                if (GameManager.Instance.CurrentTimeInGame > slot.from && GameManager.Instance.CurrentTimeInGame < slot.to)
                    return true;
            }
        }
        return m_isLock;
    }

    #region digicode
    public bool parametrizedDigicode()
    {

        if (m_AssociatedDigicodeGameObject.transform.Find("Canvas_screen_digicode/TextDigicode"))
            m_textOnScreen = m_AssociatedDigicodeGameObject.transform.Find("Canvas_screen_digicode/TextDigicode").GetComponent<Text>();
        else
        {
            Debug.LogWarning("A digicode was given with the door " + gameObject.name + " , but the TextDigicode UI was not found.");
            return false;
        }

        List<Button> buttonsDigicode = new List<Button>();
        for (int i = 0; i < 10; i++)
        {
            string buttonName = "Canvas_screen_digicode/Button_digi_" + i.ToString();
            if (m_AssociatedDigicodeGameObject.transform.Find(buttonName))
                buttonsDigicode.Add(m_AssociatedDigicodeGameObject.transform.Find(buttonName).GetComponent<Button>());
            else
            {
                Debug.LogWarning("A digicode was given with the door " + gameObject.name + " , but the button_digi_" + i.ToString() + "UI was not found.");
                return false;
            }
        }


        //the addListener have to be set one by one
        (buttonsDigicode[0]).onClick.AddListener(delegate () { this.printStringOnScreen("0"); });
        (buttonsDigicode[1]).onClick.AddListener(delegate () { this.printStringOnScreen("1"); });
        (buttonsDigicode[2]).onClick.AddListener(delegate () { this.printStringOnScreen("2"); });
        (buttonsDigicode[3]).onClick.AddListener(delegate () { this.printStringOnScreen("3"); });
        (buttonsDigicode[4]).onClick.AddListener(delegate () { this.printStringOnScreen("4"); });
        (buttonsDigicode[5]).onClick.AddListener(delegate () { this.printStringOnScreen("5"); });
        (buttonsDigicode[6]).onClick.AddListener(delegate () { this.printStringOnScreen("6"); });
        (buttonsDigicode[7]).onClick.AddListener(delegate () { this.printStringOnScreen("7"); });
        (buttonsDigicode[8]).onClick.AddListener(delegate () { this.printStringOnScreen("8"); });
        (buttonsDigicode[9]).onClick.AddListener(delegate () { this.printStringOnScreen("9"); });


        Button buttonReturn = m_AssociatedDigicodeGameObject.transform.Find("Canvas_screen_digicode/Button_digi_return").GetComponent<Button>();
        buttonReturn.onClick.AddListener(delegate () { this.returnTextOnScreen(); });
        return true;
    }

    public void returnTextOnScreen()
    {
        if(m_textOnScreen.text.Length > 0)
            m_textOnScreen.text = m_textOnScreen.text.Substring(0, m_textOnScreen.text.Length - 1);
        
    }

    public void printStringOnScreen(string strToPrint)
    {
        if (m_textOnScreen.text.Length + strToPrint.Length <= m_password.Length)
            m_textOnScreen.text += strToPrint;
        else
            m_textOnScreen.text = "";
        if (isPasswordCorrect())
        {
            m_isLock = false;
            m_textOnScreen.text = "";
        }
            

    }

    public bool isPasswordCorrect()
    {
        return m_textOnScreen.text == m_password;
    }
    #endregion
}

//I don't use struct, because we don't see it in inspectector then.
[System.Serializable]
public class SlotTime
{
    public TimeInGame from;
    public TimeInGame to;
}
