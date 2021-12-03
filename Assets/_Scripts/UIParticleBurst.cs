using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIParticleBurst : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> bursts;

    public int innerBurstCount = 36;
    public int outterBurstCount = 12;

    public float lifeTime = 5f;
    public float speed = 0.8f;

    [SerializeField]
    private Color particleColor;

    void Start()
    {
        foreach(Transform child in transform)
        {
            bursts.Add(child.gameObject);
            child.gameObject.SetActive(false);
            child.gameObject.GetComponent<UnityEngine.UI.Image>().color = particleColor;
        }
        StartBurst();
    }

    public void StartBurst()
    {
        StartCoroutine(EmitInner());
        StartCoroutine(EmitOutter());
    }
    IEnumerator EmitOutter()
    {
        //Debug.Log("Starting new thread at : " + Time.time.ToString());
        yield return new WaitForSeconds(0.5f);

        float time = 0f;
        while (time < lifeTime)
        {
            for (int i = 0; i < outterBurstCount; i++)
            {
                if (!bursts[i + innerBurstCount].activeSelf)
                    bursts[i + innerBurstCount].SetActive(true);

                float angle = ((float)(i - innerBurstCount) / (float)(outterBurstCount)) * 360f;
                bursts[i + innerBurstCount].transform.position += new Vector3(speed * Mathf.Cos(angle * Mathf.Deg2Rad), speed * Mathf.Sin(angle * Mathf.Deg2Rad));
                bursts[i + innerBurstCount].transform.localScale = Vector3.Lerp(Vector3.one * 0.1f, Vector3.one, time / (lifeTime + 0.3f));
                //bursts[i].transform.position += new Vector3(3f, 0f);
            }
            yield return null;

            time += Time.deltaTime;
        }
        for (int i = innerBurstCount; i < outterBurstCount + innerBurstCount; i++)
        {
            bursts[i].transform.localPosition = Vector3.zero;
            bursts[i].transform.localScale = Vector3.one;
            bursts[i].SetActive(false);
        }

    }
    IEnumerator EmitInner()
    {
        //Debug.Log("Starting new thread at : " + Time.time.ToString());

        float time = 0f;
        while (time < lifeTime + 0.3f)
        {
            for (int i = 0; i < innerBurstCount; i++)
            {
                if (!bursts[i].activeSelf)
                    bursts[i].SetActive(true);

                float angle = ((float)i / (float)innerBurstCount) * 360f;
                bursts[i].transform.position += new Vector3(speed * Mathf.Cos(angle * Mathf.Deg2Rad), speed * Mathf.Sin(angle * Mathf.Deg2Rad));
                bursts[i].transform.localScale = Vector3.Lerp(Vector3.one * 0.1f, Vector3.one * 0.6f, time / (lifeTime + 0.3f));
                //bursts[i].transform.position += new Vector3(3f, 0f);
            }
            yield return null;

            time += Time.deltaTime;
        }
        for (int i = 0; i < innerBurstCount; i++)
        {
            bursts[i].transform.localPosition = Vector3.zero;
            bursts[i].transform.localScale = Vector3.one;
            bursts[i].SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartBurst();
        }
        
    }
}
