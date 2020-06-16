using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FreeDraw;

public class SpritesComparing : MonoBehaviour
{
    public Texture2D sprite, drawing;
    public GameObject drawableSprite;

    public Texture2D[] sprites, drawings;
    public GameObject[] drawableSprites;

    public void Comparison()
    {
        float blackPixels = 0, blackPixels1 = 0, blackPixels2 = 0, whitePixels = 0, whitePixels1 = 0, whitePixels2 = 0, white1black2 = 0;
        int texWidht = (int)sprite.width;
        int texHeight = (int)sprite.height;

        Debug.Log(sprite.width + " " + sprite.height);

        for (int i = 0; i < texWidht; i++)
        {
            for (int ii = 0; ii < texHeight; ii++)
            {

                if (sprite.GetPixel(i,ii) == Color.black)
                {
                    blackPixels1++;
                }
                if (drawing.GetPixel(i, ii) == Color.black)
                {
                    blackPixels2++;
                }
                if (sprite.GetPixel(i, ii) == Color.white)
                {
                    whitePixels1++;
                }
                if (drawing.GetPixel(i, ii) == Color.white)
                {
                    whitePixels2++;
                }

                if (sprite.GetPixel(i, ii) == Color.white && drawing.GetPixel(i, ii) != Color.white)
                {
                    white1black2++;
                }


                if (sprite.GetPixel(i, ii) == drawing.GetPixel(i, ii) && drawing.GetPixel(i,ii) != Color.white && sprite.GetPixel(i, ii) != Color.white)
                {
                    blackPixels++;
                }
                if (sprite.GetPixel(i, ii) == drawing.GetPixel(i, ii) && drawing.GetPixel(i, ii) == Color.white && sprite.GetPixel(i, ii) == Color.white)
                {
                    whitePixels++;
                }

            }
        }

        if (blackPixels / blackPixels1 >= 0.5f && white1black2 / whitePixels1 <= 0.5f)
        {
            Debug.Log("GOOD     " + blackPixels / blackPixels1 * 100 + "%" + "     " + white1black2 / whitePixels1 * 100 + "%");
        }
        else
        {
            Debug.Log("BAD     " + blackPixels / blackPixels1 * 100 + "%" + "     " + white1black2 / whitePixels1 * 100 + "%");
            drawableSprite.GetComponent<Drawable>().ResetCanvas();
        }
    }
}
