using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogs : MonoBehaviour
{

    public int successCounter = 0;
    public GameObject background, bigPicture;
    public string startText, finalText;
    public string[] success, failure, failureBig, failureShowLine, failureWrongColor;
    public GameObject[] drawings;

    public GameObject cursor, pencils;

    public Font regular, bold;

    private bool successChecked = false;

    void Start()
    {
        cursor.GetComponent<CursorMovement>().SetNoCursor();

        background.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        gameObject.GetComponent<Text>().color = new Color(1f, 1f, 1f, 0f);

        StartCoroutine(showStartText());
    }

    private void Update()
    {
        if (!successChecked)
        {
            if (successCounter == 4)
            {
                FinalText();
            }
        }
    }

    public void FinalText()
    {
        successChecked = true;
        StartCoroutine(showFinalText());
    }

    IEnumerator showFinalText()
    {
        cursor.GetComponent<CursorMovement>().SetNoCursor();

        gameObject.GetComponent<Text>().text = finalText;
        gameObject.GetComponent<Text>().font = bold;

        while (background.GetComponent<Image>().color.a < 1f)
        {
            yield return new WaitForSeconds(0.05f);
            background.GetComponent<Image>().color += new Color(0f, 0f, 0f, 0.1f);
            gameObject.GetComponent<Text>().color += new Color(0f, 0f, 0f, 0.1f);
        }

        cursor.GetComponent<CursorMovement>().ResetCursor();
        FreeDraw.Drawable.Pen_Colour = new Color(0f, 0f, 0f, 0f);

        pencils.SetActive(false);
        //bigPicture.transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }

    IEnumerator showStartText()
    {
        gameObject.GetComponent<Text>().text = "Пора карандаши доставать и портрет дорисовывать.\nВсего-то ничего осталось - раз, два, три... Четыре кусочка!";
        gameObject.GetComponent<Text>().font = regular;

        while (background.GetComponent<Image>().color.a < 1f)
        {
            yield return new WaitForSeconds(0.05f);
            background.GetComponent<Image>().color += new Color(0f, 0f, 0f, 0.1f);
            gameObject.GetComponent<Text>().color += new Color(0f, 0f, 0f, 0.1f);
        }
        yield return new WaitForSeconds(4f);
        while (background.GetComponent<Image>().color.a > 0f)
        {
            yield return new WaitForSeconds(0.05f);
            background.GetComponent<Image>().color -= new Color(0f, 0f, 0f, 0.1f);
            gameObject.GetComponent<Text>().color -= new Color(0f, 0f, 0f, 0.1f);
        }

        cursor.GetComponent<CursorMovement>().SetCursor();
        pencils.SetActive(true);
    }

    public void ShowSuccessText (int id)
    {
        successCounter += 1;

        gameObject.GetComponent<Text>().text = success[id];
        gameObject.GetComponent<Text>().font = regular;
        StartCoroutine(ShowText(5f));
    }

    public void ShowFailureText(int id, int wrongColor, float mistakes)
    {
        if (wrongColor > 0)
        {
            gameObject.GetComponent<Text>().text = failureWrongColor[id];
        }
        else if (mistakes < 3f || mistakes >= 4f)
        {
            gameObject.GetComponent<Text>().text = failure[id];
        }
        else
        {
            gameObject.GetComponent<Text>().text = failureShowLine[id];
        }
        gameObject.GetComponent<Text>().font = regular;

        StartCoroutine(ShowText(3.5f));
    }

    public void ShowBigFailureText(int id, int wrongColor, float mistakes)
    {
        if (wrongColor > 0)
        {
            gameObject.GetComponent<Text>().text = failureWrongColor[id];
        }
        else if (mistakes < 3f || mistakes >= 4f)
        {
            gameObject.GetComponent<Text>().text = failureBig[id];
        }
        else
        {
            gameObject.GetComponent<Text>().text = failureShowLine[id];
        }
        gameObject.GetComponent<Text>().font = regular;

        StartCoroutine(ShowText(3.5f));
    }

    IEnumerator ShowText(float seconds)
    {
        cursor.GetComponent<CursorMovement>().SetNoCursor();

        if (successCounter != 4)
        {
            while (background.GetComponent<Image>().color.a < 1f)
            {
                yield return new WaitForSeconds(0.05f);
                background.GetComponent<Image>().color += new Color(0f, 0f, 0f, 0.1f);
                gameObject.GetComponent<Text>().color += new Color(0f, 0f, 0f, 0.1f);
            }
            yield return new WaitForSeconds(seconds);
            while (background.GetComponent<Image>().color.a > 0f)
            {
                yield return new WaitForSeconds(0.05f);
                background.GetComponent<Image>().color -= new Color(0f, 0f, 0f, 0.1f);
                gameObject.GetComponent<Text>().color -= new Color(0f, 0f, 0f, 0.1f);
            }

            if (gameObject.GetComponent<Text>().text == success[0] || gameObject.GetComponent<Text>().text == success[1] || gameObject.GetComponent<Text>().text == success[2] || gameObject.GetComponent<Text>().text == success[3])
            {
                cursor.GetComponent<CursorMovement>().SetCursor();
                FreeDraw.Drawable.Pen_Colour = new Color(0f, 0f, 0f, 0f);
            }
            else
            {
                cursor.GetComponent<CursorMovement>().ResetCursor();
            }
        }
        else
        {
            while (background.GetComponent<Image>().color.a < 1f)
            {
                yield return new WaitForSeconds(0.05f);
                background.GetComponent<Image>().color += new Color(0f, 0f, 0f, 0.1f);
                gameObject.GetComponent<Text>().color += new Color(0f, 0f, 0f, 0.1f);
            }
            bigPicture.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
    }

}
