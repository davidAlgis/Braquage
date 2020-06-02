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
    private List<Text> m_textButtonMail;

    [Header("Camera security")]
    [SerializeField]
    private List<GameObject> m_camerasAttachedGO = new List<GameObject>();
    private List<GameObject> m_securityCameraButtonGO = new List<GameObject>();
    


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
            if (DebugTool.tryFindGOChildren(gameObject, "Screen/Canvas_screen_mailOS", out m_canvasGO, LogType.Error) == false)
                return false;

        if(m_cameraOS)
        {
            if (DebugTool.tryFindGOChildren(gameObject, "Screen/Canvas_screen_cameraOS", out m_canvasGO, LogType.Error) == false)
                return false;
            if( initSecurityCameraOS() == false)
                return false;
        }
            
        
            

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

            if(DebugTool.tryFindGOChildren(gameObject, "Screen/PointToLookOnScreen", out pointToLookOnScreen))
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
            Debug.LogWarning("Unable find canvas component in " + m_canvasGO.name);
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
                if (initMailOS() == false)
                    return;

            if(m_cameraOS)
            {
                /*enableWhiteScreen();
                if (initSecurityCameraOS() == false)
                    return;*/
                connectMainMenuSecurityCamera();
                //GameObject.Find("Camera2").transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), 30);
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
    private bool initMailOS()
    {
        if (m_mails.Count < 2)
        {
            Debug.LogError("Unable to init Mail OS, because m_mails.Count is empty");
            return false;
        }
            
        for(int i=0; i<7;i++)
        {
            
            GameObject buttonMailGO, textGO, textContentMailGO, headerTextMailGO, receiverMailTextGO, senderMailGO;

            if (DebugTool.tryFindGOChildren(m_canvasGO, "ReceiverMail/TextReceiverMail", out receiverMailTextGO, LogType.Error) == false)
                return false;

            if (receiverMailTextGO.TryGetComponent(out Text receiverText))
                receiverText.text = "à:" + m_mails[0].receiverAdressMail;
            else
                Debug.LogWarning("Unable to find any text component in " + receiverMailTextGO.name);

            if (DebugTool.tryFindGOChildren(m_canvasGO, "SenderMail/TextSenderMail", out senderMailGO, LogType.Error) == false)
                return false;

            if (senderMailGO.TryGetComponent(out Text senderText))
                senderText.text = "de:" + m_mails[0].senderAdressMail;
            else
                Debug.LogWarning("Unable to find any text component in " + senderMailGO.name);


            if (DebugTool.tryFindGOChildren(m_canvasGO, "TextContentMail", out textContentMailGO, LogType.Error) == false)
                return false;

            if (textContentMailGO.TryGetComponent(out Text contentMail))
                contentMail.text = m_mails[0].contentMail;
            else
                Debug.LogWarning("Unable to find any text component in " + senderMailGO.name);

            if (DebugTool.tryFindGOChildren(m_canvasGO, "headerMail2/headerMail2Text", out headerTextMailGO, LogType.Error) == false)
                return false;

            if (headerTextMailGO.TryGetComponent(out Text headerText))
                headerText.text = m_mails[0].objectMail + " - " + m_mails[0].timeOfSend.ToString();
            else
                Debug.LogWarning("Unable to find any text component in " + headerTextMailGO.name);

            if (DebugTool.tryFindGOChildren(m_canvasGO, "ButtonMail" + (i + 1).ToString(), out buttonMailGO, LogType.Error) == false)
                return false;

            if (buttonMailGO.TryGetComponent(out Button button))
            {

                //when we use lambda function we need to use a temporary variable in the loop because of https://stackoverflow.com/questions/271440/captured-variable-in-a-loop-in-c-sharp
                print(i);
                    print(m_mails.Count);

                if (m_mails.Count > i)
                {

                    int tempIndexButton = i;
                    button.onClick.AddListener(delegate () { this.printMailContentOnComputer(tempIndexButton); });
                }
                else
                    button.interactable = false;
                
            }


            if (DebugTool.tryFindGOChildren(buttonMailGO, "TextButtonMail" + (i + 1).ToString(), out textGO))
            {
                if (textGO.TryGetComponent(out Text textToAdd))
                {

                    if(m_mails.Count > i)
                        textToAdd.text = m_mails[i].senderAdressMail + "\n" + m_mails[i].objectMail;
                    else
                        textToAdd.text = "";
                    
                }
                else
                    Debug.LogWarning("Unable to find any text component in " + textGO.name);
            }
        }



        return true;
    }

    //define the button of camera
    private bool initSecurityCameraOS()
    {

        if(m_camerasAttachedGO != null)
        {

            GameObject buttonPatternGO;
            //find the patron for the button
            if (DebugTool.tryFindGOChildren(m_canvasGO, "ButtonCameraPattern", out buttonPatternGO, LogType.Error) == false)
                return false;
            Image imageSecurityCameraPattern = buttonPatternGO.GetComponent<Image>();

            int indexCamera = 0;
            foreach(GameObject cameraParentGO in m_camerasAttachedGO)
            {

                GameObject cameraGO;

                if (DebugTool.tryFindGOChildren(cameraParentGO, "security_camera_Camera", out cameraGO, LogType.Error))
                {
                    if (cameraGO.TryGetComponent(out Camera securityCamera) == false)
                    { 
                        Debug.LogError("Unable to find any Camera component in " + cameraGO.name);
                        return false;
                    }

                    RenderTexture cameraRenderTexture = new RenderTexture(144, 144, 16, RenderTextureFormat.ARGB32);
                    cameraRenderTexture.Create();
                    securityCamera.targetTexture = cameraRenderTexture;
                    securityCamera.Render();

                    GameObject buttonCameraGO = new GameObject();
                    buttonCameraGO.transform.parent = m_canvasGO.transform;
                    buttonCameraGO.name = "Button_securityCamera_n" + indexCamera.ToString();
                    RectTransform rectTransformButton = buttonCameraGO.AddComponent<RectTransform>();
                    rectTransformButton.Rotate(new Vector3(0.0f, 1.0f, 0.0f), -90);
                    rectTransformButton.localPosition = Vector3.zero;

                    rectTransformButton.anchorMin = new Vector2(0.0f, 0.0f);
                    rectTransformButton.anchorMax = new Vector2(1.0f, 1.0f);
                    rectTransformButton.offsetMin = new Vector2(0.0f, 0.0f);
                    rectTransformButton.offsetMax = new Vector2(0.0f, 0.0f);
                    rectTransformButton.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    Button buttonSecurityCamera = buttonCameraGO.AddComponent<Button>();
                    Image imageSecurityCamera = buttonCameraGO.AddComponent<Image>();

                    imageSecurityCamera.sprite = imageSecurityCameraPattern.sprite;






                    GameObject rawImageGO = new GameObject();
                    rawImageGO.transform.parent = buttonCameraGO.transform;
                    rawImageGO.name = "RawImage_securityCamera_n" + indexCamera.ToString();
                    RectTransform rectTransformImage = rawImageGO.AddComponent<RectTransform>();
                    rectTransformImage.Rotate(new Vector3(0.0f, 1.0f, 0.0f), -90);
                    rectTransformImage.localPosition = Vector3.zero;

                    rectTransformImage.anchorMin = new Vector2(0.01f, 0.01f);
                    rectTransformImage.anchorMax = new Vector2(0.99f, 0.99f);
                    rectTransformImage.offsetMin = new Vector2(0.0f, 0.0f);
                    rectTransformImage.offsetMax = new Vector2(0.0f, 0.0f);
                    rectTransformImage.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    rawImageGO.AddComponent<CanvasRenderer>();
                    RawImage rawImage = rawImageGO.AddComponent<RawImage>();
                    rawImage.texture = cameraRenderTexture;
                    //rawImage.enabled = false;
                    buttonCameraGO.SetActive(false);
                    m_securityCameraButtonGO.Add(buttonCameraGO);
                }
                else
                    return false;

                indexCamera++;
            }
        }
        return true;
    }

    private bool enableWhiteScreen()
    {
        GameObject whiteScreenGO;
        if (DebugTool.tryFindGOChildren(m_canvasGO, "WhiteBackground", out whiteScreenGO, LogType.Warning) == false)
            return false;

        if (whiteScreenGO.TryGetComponent(out Image whiteScreen) == false)
            return false;

        whiteScreen.enabled = true;

        return true;
    }

    //print 9 camera on the page.
    private bool connectMainMenuSecurityCamera(int pageToPrint = 0)
    {
        if (m_securityCameraButtonGO == null)
        {
            Debug.LogError("m_securityCameraRawImage has not been defined");
            return false;
        }

        int nbrOfCamera = m_securityCameraButtonGO.Count;

        if (nbrOfCamera < 9*pageToPrint)
        {
            Debug.LogError("Unable to print the " + pageToPrint + " page, because there aren't enough camera.");
            return false;
        }

        /*We have to define the number of camera on this page, 
            * for example, if we want to display the 0th page
            * and we only have 3 camera on 9 to display, then 
            * we have to do the loop only on the 3 canvas.
            * */
        int nbrOfCameraForThisPage;

        if (nbrOfCamera < 9 * (pageToPrint + 1))
            nbrOfCameraForThisPage = nbrOfCamera - 9 * pageToPrint;
        else
            nbrOfCameraForThisPage = 9;

        for (int i = 9 * pageToPrint; i < nbrOfCameraForThisPage; i++)
        {

            if (i > nbrOfCamera)
            {
                Debug.LogError("Unable to display the " + i + " camera, because there aren't enough camera.");
                return false;
            }
            //m_securityCameraButtonGO[i].enabled = true;
            m_securityCameraButtonGO[i].SetActive(true);
            //update rectTransform
            /*if (m_securityCameraRawImage[i].gameObject.TryGetComponent(out AspectRatioFitter aspectRatioFitter))
                aspectRatioFitter.enabled = false;*/

            //m_securityCameraButtonGO[i].rectTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);


            //the size is uniform
            //m_securityCameraRawImage[i].rectTransform.sizeDelta = new Vector2(0.24f, 0.14f);

            if (m_securityCameraButtonGO[i].TryGetComponent(out RectTransform rectTransform) == false)
            {
                Debug.LogError("Unable to find any RectTransform componnement in " + m_securityCameraButtonGO[i].name);
                return false;
            }
                


            switch (i % 9)
            {
                case 8:
                    rectTransform.anchorMin = new Vector2(0.04f, 0.04f);
                    rectTransform.anchorMax = new Vector2(0.32f, 0.32f);
                    break;
                case 7:
                    rectTransform.anchorMin = new Vector2(0.36f, 0.04f);
                    rectTransform.anchorMax = new Vector2(0.64f, 0.32f);
                    break;
                case 6:
                    rectTransform.anchorMin = new Vector2(0.68f, 0.04f);
                    rectTransform.anchorMax = new Vector2(0.96f, 0.32f);
                    break;
                case 5:
                    rectTransform.anchorMin = new Vector2(0.04f, 0.33f);
                    rectTransform.anchorMax = new Vector2(0.32f, 0.61f);
                    break;
                case 4:
                    rectTransform.anchorMin = new Vector2(0.36f, 0.33f);
                    rectTransform.anchorMax = new Vector2(0.64f, 0.61f); 
                    break;
                case 3:
                    rectTransform.anchorMin = new Vector2(0.68f, 0.33f);
                    rectTransform.anchorMax = new Vector2(0.96f, 0.61f); 
                    break;
                case 2:
                    rectTransform.anchorMin = new Vector2(0.68f, 0.62f);
                    rectTransform.anchorMax = new Vector2(0.96f, 0.90f);
                    break;
                case 1:
                    rectTransform.anchorMin = new Vector2(0.36f, 0.62f);
                    rectTransform.anchorMax = new Vector2(0.64f, 0.90f);
                    break;
                case 0:
                    rectTransform.anchorMin = new Vector2(0.04f, 0.62f);
                    rectTransform.anchorMax = new Vector2(0.32f, 0.90f);
                    break;
            }
        }
        
        return true;
    }

    private void printSecurityCameraOnComputer(int index)
    {

    }

    void printMailContentOnComputer(int index)
    {
        if (m_coherencyComputer == false)
            return;
        print(index);
        if(index >= m_mails.Count || index < 0)
        {
            Debug.LogError("Unable to print the " + index.ToString() + " mail because it was not defined in m_mails");
            return;
        }


        GameObject textContentMailGO, headerTextMailGO, receiverMailTextGO, senderMailGO;

        if (DebugTool.tryFindGOChildren(m_canvasGO, "ReceiverMail/TextReceiverMail", out receiverMailTextGO, LogType.Error) == false)
            return;

        if (receiverMailTextGO.TryGetComponent(out Text receiverText))
            receiverText.text = "à:"  + m_mails[index].receiverAdressMail;
        else
            Debug.LogWarning("Unable to find any text component in " + receiverMailTextGO.name);

        if (DebugTool.tryFindGOChildren(m_canvasGO, "SenderMail/TextSenderMail", out senderMailGO, LogType.Error) == false)
            return;

        if (senderMailGO.TryGetComponent(out Text senderText))
            senderText.text = "de:" + m_mails[index].senderAdressMail;
        else
            Debug.LogWarning("Unable to find any text component in " + senderMailGO.name);


        if (DebugTool.tryFindGOChildren(m_canvasGO, "TextContentMail", out textContentMailGO, LogType.Error) == false)
            return;

        if (textContentMailGO.TryGetComponent(out Text contentMail))
            contentMail.text = m_mails[index].contentMail;
        else
            Debug.LogWarning("Unable to find any text component in " + senderMailGO.name);

        if (DebugTool.tryFindGOChildren(m_canvasGO, "headerMail2/headerMail2Text", out headerTextMailGO, LogType.Error) == false)
            return;

        if (headerTextMailGO.TryGetComponent(out Text headerText))
            headerText.text = m_mails[index].objectMail + " - " + m_mails[index].timeOfSend.ToString();
        else
            Debug.LogWarning("Unable to find any text component in " + headerTextMailGO.name);
    }
}

[System.Serializable]
public class Mail
{
    public string senderAdressMail;
    public string receiverAdressMail;
    public string objectMail;
    public string contentMail;
    public TimeInGame timeOfSend;

}
