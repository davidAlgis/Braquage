using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Knowledge : MonoBehaviour
{


}


public class Password : Knowledge
{
    [SerializeField]
    private GameObject m_elementAssociated;

    [SerializeField]
    private string m_password;

}