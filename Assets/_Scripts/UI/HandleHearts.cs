using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleHearts : MonoBehaviour
{

    [SerializeField]
    GameObject playerRef;
    [SerializeField]
    GameObject heart0;
    [SerializeField]
    GameObject heart1;
    [SerializeField]
    GameObject heart2;
    [SerializeField]
    GameObject heart3;
    [SerializeField]
    GameObject heart4;

    float playerHealth;
    float maxHealth;

    public void SetPlayerRef(GameObject go)
    {
        playerRef = go;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdatePlayerHealth();
        maxHealth = playerRef.GetComponent<CombatAttributes>().GetMaxHealth();
    }

    // Update is called once per frame
    void Update()
    {
        //Don't want to do this every single frame, but for now that's ok.
        UpdatePlayerHealth();
    }

    void UpdatePlayerHealth()
    {
        playerHealth = playerRef.GetComponent<CombatAttributes>().GetHealth();
        HandleHeartImages();
    }

    void HandleHeartImages()
    {
        //if we were doing a game with more hearts, Id say we should make an array and loop through it. This is good if the max we'll be using is 5.
        //heart0.GetComponent<Image>().enabled = playerHealth > 0;
        //heart1.GetComponent<Image>().enabled = playerHealth > 1;
        //heart2.GetComponent<Image>().enabled = playerHealth > 2;
        //heart3.GetComponent<Image>().enabled = playerHealth > 3;
        //heart4.GetComponent<Image>().enabled = playerHealth > 4;

        float overallPercentage = playerHealth / maxHealth;
        float[] percentages =
        {
            Mathf.Clamp(overallPercentage / 0.2f,          0f, 1f),
            Mathf.Clamp((overallPercentage - 0.2f) / 0.2f, 0f, 1f),
            Mathf.Clamp((overallPercentage - 0.4f) / 0.2f, 0f, 1f),
            Mathf.Clamp((overallPercentage - 0.6f) / 0.2f, 0f, 1f),
            Mathf.Clamp((overallPercentage - 0.8f) / 0.2f, 0f, 1f),
        };


        heart0.transform.Find("Image").GetComponent<Image>().rectTransform.sizeDelta = new Vector2(percentages[0] * 40f, 40f);
        heart1.transform.Find("Image").GetComponent<Image>().rectTransform.sizeDelta = new Vector2(percentages[1] * 40f, 40f);
        heart2.transform.Find("Image").GetComponent<Image>().rectTransform.sizeDelta = new Vector2(percentages[2] * 40f, 40f);
        heart3.transform.Find("Image").GetComponent<Image>().rectTransform.sizeDelta = new Vector2(percentages[3] * 40f, 40f);
        heart4.transform.Find("Image").GetComponent<Image>().rectTransform.sizeDelta = new Vector2(percentages[4] * 40f, 40f);

    }
}
