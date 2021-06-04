using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarController : MonoBehaviour
{
    [SerializeField]
    private float raiseTime = 2f;
    [SerializeField]
    private float maxScale = 1f;
    [SerializeField]
    private LayerMask WallLayer;
    [SerializeField]
    private string wallTag;

    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Grow());
    }


    private IEnumerator Grow()
    {
        float startScale = transform.localScale.y;

        do
        {
            Vector3 newScale = transform.localScale;
            newScale.y = Mathf.Lerp(startScale, maxScale, timer / raiseTime);
            timer += Time.deltaTime;
            if (timer >= raiseTime)
            {
                newScale.y = maxScale;
            }
            transform.localScale = newScale;
            yield return null;
        }
        while (timer < raiseTime);

        gameObject.layer = WallLayer;
        gameObject.tag = wallTag;

        // Once done stop the coroutine
        StopCoroutine(Grow());
    }
}
