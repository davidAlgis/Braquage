using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            UIManager.Instance.enableUIPressButton(true, "F");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            GameManager.Instance.Money += 100;
            GameObject.Find("Money").SetActive(false);
            UIManager.Instance.updateUIMoney();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            UIManager.Instance.enableUIPressButton(false);
        }
    }
}
