using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEditor;

public class Door : MonoBehaviour
{
    /*The doors works with a system of front and back
     * if the character who try to open them is front 
     * to the door it'll use the front attributes, if 
     * he's on the back then it'll use the back
     * attributes.
     */

    [SerializeField]
    private bool isFront = true;

    private float m_initialRotation;
    private float m_actualRotation;
    private bool m_isOpen = false;
    [SerializeField]
    private float m_rotation = 110;

    #region Front_Door
    [Header("Front Door")]
    [SerializeField]
    private GameObject m_frontGO;
    [SerializeField]
    private bool m_isLockFront;
    private bool m_initStateLockFront;

    //digicode Front Door
    [SerializeField]
    private GameObject m_associatedDigicodeGOFront;
    [SerializeField]
    private string m_passwordFront;
    private Text m_textOnScreenFront;
    #endregion


    #region Back_Door
    [Header("Back Door")]
    [SerializeField]
    private GameObject m_backGO;
    
    [SerializeField]
    private bool m_isLockBack;
    private bool m_initStateLockBack;

    //Digicode Back Door
    [SerializeField]
    private GameObject m_associatedDigicodeGOBack;
    [SerializeField]
    private string m_passwordBack;
    private Text m_textOnScreenBack;
    #endregion

    public bool IsOpen { get => m_isOpen; set => m_isOpen = value; }
    public GameObject AssociatedDigicodeGOFront { get => m_associatedDigicodeGOFront; set => m_associatedDigicodeGOFront = value; }
    public bool IsLock { get => m_isLockFront; set => m_isLockFront = value; }
    public GameObject FrontGO { get => m_frontGO; set => m_frontGO = value; }
    public GameObject BackGO { get => m_backGO; set => m_backGO = value; }
    public GameObject AssociatedDigicodeGOBack { get => m_associatedDigicodeGOBack; set => m_associatedDigicodeGOBack = value; }
    public bool IsFront { get => isFront; set => isFront = value; }

    private void Start()
    {

        /*define init lock state, which will be use 
         * once the door has been closed. To lock the 
         * in his initial state. */

        m_initStateLockFront = m_isLockFront;
        m_initStateLockBack = m_isLockBack;

        //If there is a digicode associated
        if (m_associatedDigicodeGOFront != null)
            if (initDigicode(sideDoor.FRONT) == false)
            {
                Debug.LogWarning("The Digicode couldn't be set properly, therefore the state of the door " + gameObject.name + " has been set to unlock.");
                m_isLockFront = false;
            }

        if (m_associatedDigicodeGOBack != null)
            if (initDigicode(sideDoor.BACK) == false)
            {
                Debug.LogWarning("The Digicode couldn't be set properly, therefore the state of the door " + gameObject.name + " has been set to unlock.");
                m_isLockFront = false;
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

        //add front and back GO
        if(DebugTool.tryFindGOChildren(gameObject,"back_"+gameObject.name,out m_backGO) == false)
        {
            m_backGO = new GameObject();
            m_backGO.name = "back_" + gameObject.name;
            m_backGO.transform.parent = gameObject.transform;
            m_backGO.transform.localPosition = new Vector3(0.0f, -1.0f, 0.0f);
        }

        if (DebugTool.tryFindGOChildren(gameObject, "front_" + gameObject.name, out m_frontGO) == false)
        {
            m_frontGO = new GameObject();
            m_frontGO.name = "front_" + gameObject.name;
            m_frontGO.transform.parent = gameObject.transform;
            m_frontGO.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);
        }
        

    }

    private void OnTriggerEnter(Collider character)
    {
        float distanceCharacterFront = Vector3.Distance(character.transform.position, m_frontGO.transform.position);
        float distanceCharacterBack = Vector3.Distance(character.transform.position, m_backGO.transform.position);

        //the character is on the front of the door
        if (distanceCharacterFront <= distanceCharacterBack)
            isFront = true;
        //the character is on the back of the door
        else
            isFront = false; 

        if (character.tag == "NPC")
            if (m_isOpen == false)
                character.GetComponent<NPC>().tryToOpenTheDoor(this);

        if (character.tag == "Player")
            UIManager.Instance.enableUIPressButton(true, "F");
    }

    private void OnTriggerStay(Collider character)
    {
        //Todo: is this redundant
        float distanceCharacterFront = Vector3.Distance(character.transform.position, m_frontGO.transform.position);
        float distanceCharacterBack = Vector3.Distance(character.transform.position, m_backGO.transform.position);

        //the character is on the front of the door
        if (distanceCharacterFront <= distanceCharacterBack)
            isFront = true;
        //the character is on the back of the door
        else
            isFront = false;

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
        //define the actual rotation which will be used in rotateTo
        m_actualRotation = transform.localRotation.eulerAngles.z;
        if (isFront)
        {
            //if door is open, then try to close it
            if (m_isOpen)
            {
                StartCoroutine(rotateTo(m_initialRotation));
                //reset the lock state
                m_isLockFront = m_initStateLockFront;
            }
            else
            {
                //if door is locked return false
                if (m_isLockFront)
                {
                    return false;
                    //UIManager.Instance.enableMessageBox("La porte est fermée à clé.");
                }
                else
                    StartCoroutine(rotateTo(m_rotation));
            }
        }
        else
        {
            //if door is open, then try to close it
            if (m_isOpen)
            {
                StartCoroutine(rotateTo(m_initialRotation));
                //reset the lock state
                m_isLockBack = m_initStateLockBack;
            }
            else
            {
                //if door is locked return false
                if (m_isLockBack)
                {
                    return false;
                    //UIManager.Instance.enableMessageBox("La porte est fermée à clé.");
                }
                else
                    StartCoroutine(rotateTo(m_rotation));
            }
        }
        return true;
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


    #region digicode

    //init the front and the back digicode
    public bool initDigicode(sideDoor side)
    {
        if(side == sideDoor.FRONT)
        {
            //init m_textcomponent
            GameObject textOnScreenGOFront;

            if (DebugTool.tryFindGOChildren(m_associatedDigicodeGOFront, "Canvas_screen_digicode/TextDigicode", out textOnScreenGOFront, LogType.Error) == false)
                return false;

            if (textOnScreenGOFront.TryGetComponent(out Text textTempFront))
                m_textOnScreenFront = textTempFront;
            else
            {
                Debug.LogError("unable to find any text component in " + textOnScreenGOFront.name);
                return false;
            }

            List<Button> buttonsDigicodeFront = new List<Button>();
            for (int i = 0; i < 10; i++)
            {
                string buttonName = "Canvas_screen_digicode/Button_digi_" + i.ToString();

                GameObject buttonGOFront;
                if (DebugTool.tryFindGOChildren(m_associatedDigicodeGOFront, buttonName, out buttonGOFront, LogType.Error) == false)
                    return false;

                if (buttonGOFront.TryGetComponent(out Button buttonToAddFront))
                    buttonsDigicodeFront.Add(buttonToAddFront);
                else
                {
                    Debug.LogError("unable to find any text component in " + buttonGOFront.name);
                    return false;
                }
                sideDoor tempSideDoor = sideDoor.FRONT;
                string tempStrToLambdaFront = i.ToString();
                //when we use lambda function we need to use a temporary variable in the loop because of https://stackoverflow.com/questions/271440/captured-variable-in-a-loop-in-c-sharp
                //https://csharpindepth.com/Articles/Closures
                (buttonsDigicodeFront[i]).onClick.AddListener(delegate () { this.printStringOnScreen(tempStrToLambdaFront, tempSideDoor); });
            }


            GameObject buttonReturnGOFront;
            if (DebugTool.tryFindGOChildren(m_associatedDigicodeGOFront, "Canvas_screen_digicode/Button_digi_return", out buttonReturnGOFront, LogType.Error) == false)
                return false;

            if (buttonReturnGOFront.TryGetComponent(out Button buttonReturnFront))
            {
                sideDoor tempSideDoor = sideDoor.FRONT; 
                buttonReturnFront.onClick.AddListener(delegate () { this.deleteCharacterOnScreen(tempSideDoor); });

            }
            else
            {
                Debug.LogError("unable to find any text component in " + buttonReturnGOFront.name);
                return false;
            }
            return true;
        }
        else
        {
            GameObject textOnScreenGOBack;

            if (DebugTool.tryFindGOChildren(m_associatedDigicodeGOBack, "Canvas_screen_digicode/TextDigicode", out textOnScreenGOBack, LogType.Error) == false)
                return false;

            if (textOnScreenGOBack.TryGetComponent(out Text textTempBack))
                m_textOnScreenBack = textTempBack;
            else
            {
                Debug.LogError("unable to find any text component in " + textOnScreenGOBack.name);
                return false;
            }

            List<Button> buttonsDigicodeBack = new List<Button>();
            for (int i = 0; i < 10; i++)
            {
                string buttonName = "Canvas_screen_digicode/Button_digi_" + i.ToString();

                GameObject buttonGOBack;
                if (DebugTool.tryFindGOChildren(m_associatedDigicodeGOBack, buttonName, out buttonGOBack, LogType.Error) == false)
                    return false;

                if (buttonGOBack.TryGetComponent(out Button buttonToAddBack))
                    buttonsDigicodeBack.Add(buttonToAddBack);
                else
                {
                    Debug.LogError("unable to find any text component in " + buttonGOBack.name);
                    return false;
                }
                string tempStrToLambdaBack = i.ToString();
                sideDoor tempSideDoor = sideDoor.BACK;
                //when we use lambda function we need to use a temporary variable in the loop because of https://stackoverflow.com/questions/271440/captured-variable-in-a-loop-in-c-sharp
                //https://csharpindepth.com/Articles/Closures
                (buttonsDigicodeBack[i]).onClick.AddListener(delegate () { this.printStringOnScreen(tempStrToLambdaBack, tempSideDoor); });
            }


            GameObject buttonReturnGOBack;
            if (DebugTool.tryFindGOChildren(m_associatedDigicodeGOBack, "Canvas_screen_digicode/Button_digi_return", out buttonReturnGOBack, LogType.Error) == false)
                return false;

            if (buttonReturnGOBack.TryGetComponent(out Button buttonReturnBack))
            {
                sideDoor tempSideDoor = sideDoor.BACK;
                buttonReturnBack.onClick.AddListener(delegate () { this.deleteCharacterOnScreen(tempSideDoor); });

            }
            else
            {
                Debug.LogError("unable to find any text component in " + buttonReturnGOBack.name);
                return false;
            }
            return true;
        }
    }

    public void deleteCharacterOnScreen(sideDoor side)
    {
        if(side == sideDoor.FRONT)
            if (m_textOnScreenFront.text.Length > 0)
                m_textOnScreenFront.text = m_textOnScreenFront.text.Substring(0, m_textOnScreenFront.text.Length - 1);
        else
            if (m_textOnScreenBack.text.Length > 0)
                m_textOnScreenBack.text = m_textOnScreenBack.text.Substring(0, m_textOnScreenBack.text.Length - 1);
        
    }

    public void printStringOnScreen(string strToPrint, sideDoor side)
    {
        if(side == sideDoor.FRONT)
        {
            if (m_textOnScreenFront.text.Length + strToPrint.Length < m_passwordFront.Length)
                m_textOnScreenFront.text += strToPrint;
            else
            {
                m_textOnScreenFront.text += strToPrint;
                StartCoroutine(waitForEraseDisplay(1.0f, side));
            }


            if (isPasswordCorrect(m_textOnScreenFront.text, side))
                StartCoroutine(waitForEraseDisplay(1.0f, side));
        }
        else
        {
            
            if (m_textOnScreenBack.text.Length + strToPrint.Length < m_passwordBack.Length)
                m_textOnScreenBack.text += strToPrint;
            else
            {
                m_textOnScreenBack.text += strToPrint;
                StartCoroutine(waitForEraseDisplay(1.0f, side));
            }


            if (isPasswordCorrect(m_textOnScreenBack.text, side))
                StartCoroutine(waitForEraseDisplay(1.0f, side));
        }


    }

    public bool tryPassword(string passwordToTest)
    {
        sideDoor side;
        if (isFront)
            side = sideDoor.FRONT;
        else
            side = sideDoor.BACK;

        StartCoroutine(tryPasswordCoroutine(passwordToTest, side));
        if (isPasswordCorrect(passwordToTest, side))
        {
            //m_isLock = false;
            return true;
        }
        else
            return false;
    }

    public IEnumerator tryPasswordCoroutine(string passwordToTest, sideDoor side)
    {
        int nbrOfCharToEnter = passwordToTest.Length;
        for (int i = 0; i < nbrOfCharToEnter; i++)
        {
            printStringOnScreen(passwordToTest[i].ToString(), side);
            yield return new WaitForSeconds(1.0f);
        }
        if (isPasswordCorrect(passwordToTest, side))
        {
            if (side == sideDoor.FRONT)
                m_isLockFront = false;
            else
                m_isLockBack = false;
        }
    }

    public IEnumerator waitForEraseDisplay(float delay, sideDoor side)
    {
        if (side == sideDoor.FRONT)
        {
            if (isPasswordCorrect(m_textOnScreenFront.text, side))
                m_isLockFront = false;

            yield return new WaitForSeconds(delay);

            m_textOnScreenFront.text = "";
        }
        else
        {
            if (isPasswordCorrect(m_textOnScreenBack.text, side))
                m_isLockBack = false;

            yield return new WaitForSeconds(delay);

            m_textOnScreenBack.text = "";
        }
    }

    public bool isPasswordCorrect(string passwordToTest, sideDoor side)
    {
        if(side == sideDoor.FRONT)
            return passwordToTest == m_passwordFront;
        else
            return passwordToTest == m_passwordBack;

    }
    #endregion

    //generate Gizmos for back and front of the door 
    void OnDrawGizmosSelected()
    {

#if UNITY_EDITOR
        Gizmos.color = Color.red;
        if(m_frontGO != null)
        {
            Gizmos.DrawSphere(m_frontGO.transform.position, 0.05f);
            Handles.Label(m_frontGO.transform.position, "Front");
        }
            
        Gizmos.color = Color.blue;
        if(m_backGO != null)
        {
            Gizmos.DrawSphere(m_backGO.transform.position, 0.05f);
            Handles.Label(m_backGO.transform.position, "Back");
        }
            
#endif
    }

    
}

public enum sideDoor
{
   FRONT,
   BACK
}