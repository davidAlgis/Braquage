using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterViewFieldAI : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            RaycastHit hit;
            Vector3 fromPosition = gameObject.transform.position;
            Vector3 toPosition = GameObject.Find("Player").GetComponent<Transform>().position;
            Vector3 direction = toPosition - fromPosition;


            if (Physics.Raycast(fromPosition, direction, out hit))
            {
                GetComponentInParent<NPC>().EnterInDetectionZone = Time.time;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            print("collide");
            RaycastHit hit;
            Vector3 fromPosition = gameObject.transform.position;
            Vector3 toPosition = GameObject.Find("Player").GetComponent<Transform>().position;
            Vector3 direction = toPosition - fromPosition;


            if (Physics.Raycast(fromPosition, direction, out hit))
            {
                
                if(hit.collider.gameObject.name=="Player")
                {
                    print("collide2");
                    GetComponentInParent<NPC>().SecondInDetectionZone = Time.time - GetComponentInParent<NPC>().EnterInDetectionZone;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            RaycastHit hit;
            Vector3 fromPosition = gameObject.transform.position;
            Vector3 toPosition = GameObject.Find("Player").GetComponent<Transform>().position;
            Vector3 direction = toPosition - fromPosition;


            if (Physics.Raycast(fromPosition, direction, out hit))
            {

                if (hit.collider.gameObject.name == "Player")
                {
                    GetComponentInParent<NPC>().ExitInDetectionZone = Time.time;
                }
            }
        }
    }
}
