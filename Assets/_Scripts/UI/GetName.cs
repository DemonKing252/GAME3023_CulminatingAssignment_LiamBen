using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CombatAttributes))]
public class GetName : MonoBehaviour
{
    [SerializeField]
    GameObject defaultGameObjectWithName;

    private string nameOfGameObject;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetEnemyName(string name)
    {
        
        nameOfGameObject = name;
        GetComponent<TextMeshProUGUI>().SetText(nameOfGameObject);
    }
}
