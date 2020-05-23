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
    private GameObject m_associatedDigicodeGO;
    [SerializeField]
    private string m_password;

    private Text m_textOnScreen;

    public bool IsOpen { get => m_isOpen; set => m_isOpen = value; }
    #endregion

    private void Start()
    {
        //If there is a digicode associated
        if (m_associatedDigicodeGO != null)
        {
            if (initDigicode() == false)
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
    }
    private void OnTriggerEnter(Collider character)
    {
        if (character.tag == "NPC")
            if (m_isOpen == false)
                character.GetComponent<NPC>().tryToOpenTheDoor(this);

        if (character.tag == "Player")
            UIManager.Instance.enableUIPressButton(true, "F");
    }

    private void OnTriggerStay(Collider character)
    {

        if (character.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.F))
            {

                tryToOpenOrCloseDoor();
                UIManager.Instance.enableUIPressButton(false);
            }
        }
    }

    private void OnTriggerExit(Collider character)
    {
        if (character.tag == "Player")
            UIManager.Instance.enableUIPressButton(false);

        if (character.tag == "NPC")
            character.GetComponent<NPC>().tryToCloseTheDoor(this);
    }

    public bool tryToOpenOrCloseDoor()
    {
        if (m_isLock)
            return false;
                //UIManager.Instance.enableMessageBox("La porte est fermée à clé.");
        else
        {
            m_actualRotation = transform.localRotation.eulerAngles.z;
            if (m_isOpen)
                StartCoroutine(rotateTo(m_initialRotation));
            else
                StartCoroutine(rotateTo(m_rotation));
            return true;
        }
    }

    IEnumerator rotateTo(float breakRotation)
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
    public bool initDigicode()
    {
        //init m_textComponement
        GameObject textOnScreenGO;

        if (DebugTool.tryFindGOChildren(m_associatedDigicodeGO, "Canvas_screen_digicode/TextDigicode", out textOnScreenGO, LogType.Error) == false)
            return false;

        if (textOnScreenGO.TryGetComponent(out Text textTemp))
            m_textOnScreen = textTemp;
        else
        {
            Debug.LogError("unable to find any text componement in " + textOnScreenGO.name);
            return false;
        }

        List<Button> buttonsDigicode = new List<Button>();
        for (int i = 0; i < 10; i++)
        {
            string buttonName = "Canvas_screen_digicode/Button_digi_" + i.ToString();

            GameObject buttonGO;
            if (DebugTool.tryFindGOChildren(m_associatedDigicodeGO, buttonName, out buttonGO, LogType.Error) == false)
                return false;

            if (buttonGO.TryGetComponent(out Button buttonToAdd))
                buttonsDigicode.Add(buttonToAdd);
            else
            {
                Debug.LogError("unable to find any text componement in " + buttonGO.name);
                return false;
            }

            string tempStrToLambda = i.ToString();
            //when we use lambda function we need to use a temporary variable in the loop because of https://stackoverflow.com/questions/271440/captured-variable-in-a-loop-in-c-sharp
            //https://csharpindepth.com/Articles/Closures
            (buttonsDigicode[i]).onClick.AddListener(delegate () { this.printStringOnScreen(tempStrToLambda); });
        }


        GameObject buttonReturnGO;
        if(DebugTool.tryFindGOChildren(m_associatedDigicodeGO, "Canvas_screen_digicode/Button_digi_return",out buttonReturnGO, LogType.Error) == false)
            return false;

        if (buttonReturnGO.TryGetComponent(out Button buttonReturn))
            buttonReturn.onClick.AddListener(delegate () { this.deleteCharacterOnScreen(); });
        else
        {
            Debug.LogError("unable to find any text componement in " + buttonReturnGO.name);
            return false;
        }
        return true;
    }

    public void deleteCharacterOnScreen()
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
        if (isPasswordCorrect(m_textOnScreen.text))
        {
            m_isLock = false;
            m_textOnScreen.text = "";
        }
            

    }

    public bool tryPassword(string passwordToTest)
    {
        if (isPasswordCorrect(passwordToTest))
        {
            m_isLock = false;
            return true;
        }
        else
            return false;
    }

    public bool isPasswordCorrect(string passwordToTest)
    {
        return passwordToTest == m_password;
    }
    #endregion

}

//I don't use struct, because we don't see it in inspector then.
[System.Serializable]
public class SlotTime
{
    public TimeInGame from;
    public TimeInGame to;
}
