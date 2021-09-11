using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsPopUp : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI pointsText;
    [SerializeField]
    private float displayTime = 2.0f;
    [SerializeField]
    private float raiseHeight = 5.0f;
    [SerializeField]
    private Color startingColor;
    [SerializeField]
    private Color finalColor;

    public void Start()
    {
        pointsText.color = startingColor;
        StartCoroutine(FadeOutScore());
    }

    private IEnumerator FadeOutScore()
    {
        float timer = 0;
        Vector3 startingPosition = transform.position;
        Vector3 endPosition = startingPosition + new Vector3(0, raiseHeight, 0);
        

        while (timer < displayTime)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startingPosition, endPosition, timer / displayTime);
            pointsText.color = Color.Lerp(startingColor, finalColor, timer / displayTime);

            yield return new WaitForEndOfFrame();
        }

        DestroySelf();
    }

    public void ChangePoints(int points)
    {
        pointsText.text = points.ToString();
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
