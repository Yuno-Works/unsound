using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AwesomeTechnologies.VegetationSystem;

[RequireComponent ( typeof ( Camera ) )]
public class VegetationCamera : MonoBehaviour
{
    private VegetationSystemPro vegetationSystemPro;

    // Start is called before the first frame update
    void Start ()
    {
        if ( vegetationSystemPro = FindObjectOfType<VegetationSystemPro> () )
        {
            vegetationSystemPro.AddCamera ( GetComponent<Camera> () );
        }
    }
}
