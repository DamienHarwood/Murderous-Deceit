using System.Collections;
using System.Collections.Generic;
using RileyMcGowan;
using UnityEngine;

public class TestCode : MonoBehaviour
{

    private LimbHealth[] limbArray;
    public enum ListOfTargets
    {
        level1Target,
        level2Target,
        level3Target
    }
    public ListOfTargets listOfTargets;
    // Start is called before the first frame update
    void Start()
    {
        limbArray = FindObjectsOfType<LimbHealth>();
        if (listOfTargets == ListOfTargets.level1Target)
        {
            for (int i = 0; i < limbArray.Length; i++)
            {
                limbArray[i].enabled = false;
            }
        }
        if (listOfTargets == ListOfTargets.level2Target)
        {
            for (int i = 0; i < limbArray.Length; i++)
            {
                limbArray[i].enabled = false;
            }
        }
        if (listOfTargets == ListOfTargets.level3Target)
        {
            for (int i = 0; i < limbArray.Length; i++)
            {
                limbArray[i].enabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
