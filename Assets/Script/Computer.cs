using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Computer : MonoBehaviour
{
    [SerializeField]
    private bool m_cameraOS = false;
    [SerializeField]
    private bool m_mailOS = false;

    private bool m_isOnComputer = false;
    private Transform m_saveLastTransformCamera;
    private GameObject m_canvasGO;
    private bool m_coherencyComputer = false;


    [Header("Mail")]
    [SerializeField]
    private List<Mail> m_mails;
    

    [Header("Camera security")]
    [SerializeField]
    private List<GameObject> m_camerasAttachedGO;
    


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

        m_coherencyComputer = checkCoherency();
    }

    private bool checkCoherency()
    {
        if(m_mailOS)
            if (DebugTool.tryFindGOChildren(gameObject, "Canvas_screen_mailOS", out m_canvasGO, LogType.Error) == false)
                return false;

        if(m_cameraOS)
            if (DebugTool.tryFindGOChildren(gameObject, "Canvas_screen_cameraOS", out m_canvasGO, LogType.Error) == false)
                return false;

        if (m_mailOS == false && m_cameraOS == false)
            Debug.LogWarning("Unknow OS to print");
        
            

        return true;
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
            GameObject pointToLookOnScreen;

            if(DebugTool.tryFindGOChildren(gameObject, "PointToLookOnScreen", out pointToLookOnScreen))
            { 
                actualCamera.transform.position = pointToLookOnScreen.transform.position;
                GameObject screen;
                if(DebugTool.tryFindGOChildren(gameObject, "Screen", out screen))
                    actualCamera.transform.LookAt(screen.transform);
            }
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

            if (m_mailOS)
                if (initListMail() == false)
                    return;

            if(m_cameraOS)
            {
                GameObject.Find("Camera2").transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), 30);
            }


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
    private bool initListMail()
    {
        List<Text> listMailText = new List<Text>();
        List<Button> buttonMail = new List<Button>();

        for (int i = 0; i < 3; i++)
        {
            GameObject buttonMailGO, textTitleGO, textGO;

            if (DebugTool.tryFindGOChildren(m_canvasGO, "ButtonMail" + (i + 1).ToString(), out buttonMailGO, LogType.Error) == false)
                return false;

            if (DebugTool.tryFindGOChildren(m_canvasGO, "TextTitleMail", out textTitleGO))
            {
                if(textTitleGO.TryGetComponent(out Text textTitle))
                    textTitle.text = "Mail OS v0.1";
                else
                    Debug.LogWarning("unable to find any text componement in " + textTitleGO.name);
            }

            if (buttonMailGO.TryGetComponent(out Button button))
            {
                //when we use lambda function we need to use a temporary variable in the loop because of https://stackoverflow.com/questions/271440/captured-variable-in-a-loop-in-c-sharp
                int tempIndexButton = i;
                buttonMail.Add(button);
                button.onClick.AddListener(delegate () { this.printMailContentOnComputer(tempIndexButton); });
            }


            if (DebugTool.tryFindGOChildren(buttonMailGO, "TextButtonMail" + (i + 1).ToString(), out textGO))
            {
                if (textGO.TryGetComponent(out Text textToAdd))
                    listMailText.Add(textToAdd);
                else
                    Debug.LogWarning("unable to find any text componement in " + textGO.name);
            }
        }

        

        //to delete
        if (m_mails.Count < 3)
        {
            Debug.LogError("not enough mail have been define in " + gameObject.name);
            return false;
        }

        if (m_mails != null)
        {
            for (int i = 0; i < 3; i++)
            {
                listMailText[i].text = m_mails[i].senderAdressMail + "\n" + m_mails[0].objectMail;
            }
        }

        return true;
    }

    private void updateSecurityCamera()
    {
        RenderTexture t = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/Material/Other/camera_render_texture.renderTexture", typeof(RenderTexture));
        //m_canvasGO.transform.Find("RawImage").GetComponent<RawImage>().texture = t;
    }

    private void Update()
    {
        //update the renderer texture if the player "can see" the screen
        if (m_cameraOS)
            if (Vector3.Distance(GameManager.Instance.getPlayerPosition(), gameObject.transform.position) < 10.0f)
                updateSecurityCamera();
    }

    void printMailContentOnComputer(int index)
    {
        if (m_coherencyComputer == false)
            return;

        if(index >= m_mails.Count || index < 0)
        {
            Debug.LogError("Unable to print the " + index.ToString() + " mail because it was not defined in m_mails");
            return;
        }

        GameObject TextContentMailGO, TextObjectMailGO, TextReceiverMailGO, TextSenderMailGO;
        

        if(DebugTool.tryFindGOChildren(m_canvasGO, "TextContentMail", out TextContentMailGO, LogType.Error) == false)
            return;

        if (DebugTool.tryFindGOChildren(m_canvasGO, "TextObjectMail", out TextObjectMailGO, LogType.Error) == false)
            return;

        if (DebugTool.tryFindGOChildren(m_canvasGO, "TextReceiverMail", out TextReceiverMailGO, LogType.Error) == false)
            return;

        if (DebugTool.tryFindGOChildren(m_canvasGO, "TextSenderMail", out TextSenderMailGO, LogType.Error) == false)
            return;

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
