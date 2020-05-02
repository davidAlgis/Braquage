using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Digicode : MonoBehaviour
{
    [SerializeField]
    private string m_password;
    [SerializeField]
    private GameObject m_doorAssociated;
    private Text m_textOnScreen;

    public string Password { get => m_password; set => m_password = value; }

    // Start is called before the first frame update
    void Start()
    {
        m_textOnScreen = transform.Find("Canvas_screen_digicode/TextDigicode").GetComponent<Text>();

        List<Button> buttonsDigicode = new List<Button>();
        for(int i=0;i<10;i++)
        {
            string buttonName = "Canvas_screen_digicode/Button_digi_" + i.ToString();
            buttonsDigicode.Add(transform.Find(buttonName).GetComponent<Button>());
            
            //(buttonsDigicode[i]).onClick.AddListener(delegate () { this.printStringOnScreen(i.ToString()); });

        }

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


        Button buttonReturn= transform.Find("Canvas_screen_digicode/Button_digi_return").GetComponent<Button>();
    }

    void printStringOnScreen(string strToPrint)
    {
        if (m_textOnScreen.text.Length + strToPrint.Length <= m_password.Length)
            m_textOnScreen.text += strToPrint;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
