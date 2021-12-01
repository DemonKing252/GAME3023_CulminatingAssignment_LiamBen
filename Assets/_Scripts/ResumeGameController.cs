using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeGameController : MonoBehaviour
{

    private const string keyCode = "_GameData";

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey(keyCode))
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
