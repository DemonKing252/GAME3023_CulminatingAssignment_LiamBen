using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CombatAttributes))]
public class GetName : MonoBehaviour
{
    [SerializeField]
    GameObject gameObjectWithName;

    private string nameOfGameObject;

    // Start is called before the first frame update
    void Start()
    {
        nameOfGameObject = gameObjectWithName.GetComponent<CombatAttributes>().GetName();
        GetComponent<TextMeshProUGUI>().SetText(nameOfGameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
