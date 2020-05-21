using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Computer : MonoBehaviour
{
    private bool m_isOnComputer = false;
    private Transform m_saveLastTransformCamera;
    private GameObject m_canvasGO;



    [Header("Mail")]
    [SerializeField]
    private List<Mail> m_mails;


    


    private void Start()
    {
        //add box collider trigger
        BoxCollider boxCol = gameObject.GetComponent(typeof(BoxCollider)) as BoxCollider;
        if (boxCol == null)
        {
            boxCol = gameObject.AddComponent<BoxCollider>();
            boxCol.center = new Vector3(0f, 0.0f, 0.5f);
            boxCol.size = new Vector3(1.5f, 1.5f, 1.2f);
            boxCol.isTrigger = true;
        }

        //detect canvas which will be used all the time
        if ((m_canvasGO = gameObject.transform.Find("Canvas_screen_pc").gameObject) == null)
            Debug.LogWarning("Unable to find canvas screen pc in" + gameObject.name);

        initListMail();
    }

    private void OnTriggerEnter(Collider character)
    {
        if (character.tag == "Player")
            UIManager.Instance.enableUIPressButton(true, "F");
    }

    private void OnTriggerStay(Collider character)
    {
        if (character.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.F) && m_isOnComputer)
            {
                connectDisconnectComputer(false);
                UIManager.Instance.enableUIPressButton(true);
                return;
            }

            if (Input.GetKeyDown(KeyCode.F) && m_isOnComputer == false)
            {
                connectDisconnectComputer(true);
                UIManager.Instance.enableUIPressButton(false);
                return;
            }
        }
    }

    private void OnTriggerExit(Collider character)
    {
        if (character.tag == "Player")
            UIManager.Instance.enableUIPressButton(false);

    }

    private void focusCameraOnScreen()
    {
        Camera actualCamera;
        if ((actualCamera = GameManager.Instance.getActualCamera()) != null)
        {
            Transform cameraTransform = actualCamera.transform;
            GameObject PointToLookOnScreen;
            if((PointToLookOnScreen = gameObject.transform.Find("PointToLookOnScreen").gameObject) != null)
            {
                actualCamera.transform.position = PointToLookOnScreen.transform.position;
                GameObject screen;
                if ((screen = gameObject.transform.Find("Screen").gameObject) != null)
                    actualCamera.transform.LookAt(screen.transform);
                else
                {
                    Debug.LogWarning("Unable to find any Screen GameObject in the childre of the " + gameObject.name);
                }
            }
            else
                Debug.LogWarning("Cannot find the children PointToLookOnScreen of" + gameObject.name);
            
        }
    }

    private void enableDisableCanvasScreen(bool enable)
    {
        if (m_canvasGO.TryGetComponent(out Canvas canvas))
            canvas.enabled = enable;
        else
            Debug.LogWarning("Unable find canvas componement in " + m_canvasGO.name);
    }

    private void connectDisconnectComputer(bool connect)
    {
        if (m_isOnComputer == connect)
        {
            if (m_isOnComputer)
                Debug.LogWarning("Try to connect to the " + gameObject.name + ", but the player is already on " + gameObject.name);
            else
                Debug.LogWarning("Try to disconnect from the " + gameObject.name + ", but the player is already disconnected from " + gameObject.name);
            return;
        }

        if(connect)
        {
            //connect to computer

            //enable the mouse cursor to permit the player to click on button
            UIManager.Instance.enableDisableMouse(true);
            //disable player and camera movement and get last tranform of the camera
            m_saveLastTransformCamera = GameManager.Instance.enableDisablePlayerCameraMovement(false);
            //focus the camera on the middle of the screen
            focusCameraOnScreen();
            //"turn on" computer by enable a white background screen
            enableDisableCanvasScreen(true);

            

            //the player is on computer
            m_isOnComputer = true;
        }
        else
        {
            //disconnect of the computer
            
            //set the camera to the last transform known
            GameManager.Instance.getActualCameraGO().transform.position = m_saveLastTransformCamera.position;
            //enable player and camera movement
            GameManager.Instance.enableDisablePlayerCameraMovement(true);
            //"turn off" computer by disabling the white background screen
            enableDisableCanvasScreen(false);
            //disable the mouse cursor to permit the player to move first person camera
            UIManager.Instance.enableDisableMouse(false);

            //the player isn't on computer
            m_isOnComputer = false;
        }

    }

    //define the text of button for mail
    private void initListMail()
    {
        List<Text> listMailText = new List<Text>();
        List<Button> buttonMail = new List<Button>();

        for (int i = 0; i < 3; i++)
        {
            GameObject buttonMailGO;

            if ((buttonMailGO = m_canvasGO.transform.Find("ButtonMail" + (i + 1).ToString()).gameObject) == null)
            {
                Debug.LogError("unable to find any gameobject of the name " + "ButtonMail" + (i + 1).ToString() + " in " + gameObject.name);
                return;
            }


            if (buttonMailGO.TryGetComponent(out Button button))
            {
                //when we use lambda function we need to use a temporary variable in the loop because of https://stackoverflow.com/questions/271440/captured-variable-in-a-loop-in-c-sharp
                int tempIndexButton = i;
                buttonMail.Add(button);
                button.onClick.AddListener(delegate () { this.printMailContentOnComputer(tempIndexButton); });
            }

            GameObject textGO;
            if ((textGO = buttonMailGO.transform.Find("TextButtonMail" + (i + 1).ToString()).gameObject) != null)
            {
                if (textGO.TryGetComponent(out Text textToAdd))
                    listMailText.Add(textToAdd);
                else
                    Debug.LogWarning("unable to find any text componement in " + textGO.name);
            }
            else
                Debug.LogWarning("unable to find any gameobject of the name " + "TextButtonMail" + (i + 1).ToString() + " in " + buttonMailGO.name);
        }

        //to delete
        if(m_mails.Count < 3)
        {
            Debug.LogError("not enough mail have been define in " + gameObject.name);
            return;
        }

        if (m_mails != null)
        {
            for (int i = 0; i < 3; i++)
            {
                listMailText[i].text = m_mails[i].senderAdressMail + "\n" + m_mails[0].objectMail;
            }
        }
    }

    void printMailContentOnComputer(int index)
    {
        if(index >= m_mails.Count || index < 0)
        {
            Debug.LogError("Unable to print the " + index.ToString() + " mail because it was not defined in m_mails");
            return;
        }

        GameObject TextContentMailGO, TextObjectMailGO, TextReceiverMailGO, TextSenderMailGO;
        
        if((TextContentMailGO = m_canvasGO.transform.Find("TextContentMail").gameObject) == null)
        {
            Debug.LogError("unable to find any gameobject of the name " + "TextContentMail" + " in " + m_canvasGO.name);
            return;
        }

        if ((TextObjectMailGO = m_canvasGO.transform.Find("TextObjectMail").gameObject) == null)
        {
            Debug.LogError("unable to find any gameobject of the name " + "TextObjectMail" + " in " + m_canvasGO.name);
            return;
        }

        if ((TextReceiverMailGO = m_canvasGO.transform.Find("TextReceiverMail").gameObject) == null)
        {
            Debug.LogError("unable to find any gameobject of the name " + "TextReceiverMail" + " in " + m_canvasGO.name);
            return;
        }

        if ((TextSenderMailGO = m_canvasGO.transform.Find("TextSenderMail").gameObject) == null)
        {
            Debug.LogError("unable to find any gameobject of the name " + "TextSenderMail" + " in " + m_canvasGO.name);
            return;
        }

        if(TextContentMailGO.TryGetComponent(out Text textContent))
            textContent.text = m_mails[index].contentMail;
        else
            Debug.LogWarning("unable to find any text componement in " + TextContentMailGO.name);

        if (TextObjectMailGO.TryGetComponent(out Text TextObject))
            TextObject.text = "objet :" + m_mails[index].objectMail;
        else
            Debug.LogWarning("unable to find any text componement in " + TextObjectMailGO.name);

        if (TextReceiverMailGO.TryGetComponent(out Text textReceiver))
            textReceiver.text = "à :" + m_mails[index].receiverAdressMail;
        else
            Debug.LogWarning("unable to find any text componement in " + TextReceiverMailGO.name);

        if (TextSenderMailGO.TryGetComponent(out Text textSender))
            textSender.text = "de :" + m_mails[index].senderAdressMail;
        else
            Debug.LogWarning("unable to find any text componement in " + TextSenderMailGO.name);
    }
}

[System.Serializable]
public class Mail
{
    public string senderAdressMail;
    public string receiverAdressMail;
    public string objectMail;
    public string contentMail;

}
