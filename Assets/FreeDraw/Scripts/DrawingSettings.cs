using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FreeDraw
{
    // Helper methods used to set drawing settings
    public class DrawingSettings : MonoBehaviour
    {
        public static bool isCursorOverUI = false;
        public float Transparency = 1f;
        public GameObject[] meloks;

        // Changing pen settings is easy as changing the static properties Drawable.Pen_Colour and Drawable.Pen_Width
        public void SetMarkerColour(Color new_color)
        {
            Drawable.Pen_Colour = new_color;
        }
        // new_width is radius in pixels
        public void SetMarkerWidth(int new_width)
        {
            Drawable.Pen_Width = new_width;
        }
        public void SetMarkerWidth(float new_width)
        {
            SetMarkerWidth((int)new_width);
        }

        public void SetTransparency(float amount)
        {
            Transparency = amount;
            Color c = Drawable.Pen_Colour;
            c.a = amount;
            Drawable.Pen_Colour = c;
        }


        // Call these these to change the pen settings


        public void SetMarkerCustom(Melok melok)
        {
            Cursor.SetCursor(melok.cursorColor, new Vector2(melok.cursorColor.width / 2, melok.cursorColor.height / 2), CursorMode.Auto);
            Debug.Log(melok.cursorColor);
            Color c = melok.Color;
            c.a = Transparency;
            SetMarkerColour(c);
            Drawable.drawable.SetPenBrush();

            foreach (GameObject obj in meloks)
            {
                if (!obj.activeSelf)
                {
                    obj.SetActive(true);
                }
            }
            melok.gameObject.SetActive(false);
        }

        public void SetMarkerRed()
        {
            // Color c = Color.red;
            Color c = new Color(0.86f, 0.24f, 0.11f);
            c.a = Transparency;
            SetMarkerColour(c);
            Drawable.drawable.SetPenBrush();
        }
        public void SetMarkerGreen()
        {
            Color c = Color.green;
            c.a = Transparency;
            SetMarkerColour(c);
            Drawable.drawable.SetPenBrush();
        }
        public void SetMarkerBlue()
        {
            // Color c = Color.blue;
            Color c = new Color(0.18f, 0.23f, 0.78f);
            c.a = Transparency;
            SetMarkerColour(c);
            Drawable.drawable.SetPenBrush();
        }
        public void SetMarkerBlack()
        {
            Color c = Color.black;
            // Color c = new Color(0.25f, 0.25f, 0.28f);
            c.a = Transparency;
            SetMarkerColour(c);
            Drawable.drawable.SetPenBrush();
        }
        public void SetEraser()
        {
            SetMarkerColour(new Color(255f, 255f, 255f, 0f));
        }

        public void PartialSetEraser()
        {
            SetMarkerColour(new Color(255f, 255f, 255f, 0.5f));
        }
    }
}