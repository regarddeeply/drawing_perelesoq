using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FreeDraw
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]  // REQUIRES A COLLIDER2D to function
    // 1. Attach this to a read/write enabled sprite image
    // 2. Set the drawing_layers  to use in the raycast
    // 3. Attach a 2D collider (like a Box Collider 2D) to this sprite
    // 4. Hold down left mouse to draw on this texture!
    public class Drawable : MonoBehaviour
    {
        // PEN COLOUR
        public static Color Pen_Colour = new Color (0f,0f,0f,0f);     // Change these to change the default drawing settings
        // PEN WIDTH (actually, it's a radius, in pixels)
        public static int Pen_Width = 2;

        public delegate void Brush_Function(Vector2 world_position);
        // This is the function called when a left click happens
        // Pass in your own custom one to change the brush type
        // Set the default function in the Awake method
        public Brush_Function current_brush;

        public LayerMask Drawing_Layers;

        public bool Reset_Canvas_On_Play = true;
        // The colour the canvas is reset to each time
        public Color Reset_Colour = new Color(0, 0, 0, 0);  // By default, reset the canvas to be transparent

        // Used to reference THIS specific file without making all methods static
        public static Drawable drawable;
        // MUST HAVE READ/WRITE enabled set in the file editor of Unity
        Sprite drawable_sprite;
        Texture2D drawable_texture;

        Vector2 previous_drag_position;
        Color[] clean_colours_array;
        Color transparent;
        Color32[] cur_colors;
        bool mouse_was_previously_held_down = false;
        bool no_drawing_on_current_drag = false;

        // ƒÎˇ ÓÔÂ‰ÂÎÂÌËˇ, ÍÓ„‰‡ Á‡ÍÓÌ˜ËÎË ËÒÓ‚‡Ú¸
        bool is_drawing_finished = false, is_drawing_started = false, is_mouse_release_counted = false;

        [SerializeField]
        public int id;
        private int wrongColorState = 0;
        private float mistakesCounter = 0f;
        public Color targetColor;
        public Texture2D sprite, drawing;
        public GameObject drawableSprite, realSprite, hintSprite;
        public float truePercent, falsePercent;
        public GameObject drawSettings, cursor;
        public GameObject eraserUI, eraser;
        public GameObject dialog;



//////////////////////////////////////////////////////////////////////////////
// BRUSH TYPES. Implement your own here


        // When you want to make your own type of brush effects,
        // Copy, paste and rename this function.
        // Go through each step
        public void BrushTemplate(Vector2 world_position)
        {
            // 1. Change world position to pixel coordinates
            Vector2 pixel_pos = WorldToPixelCoordinates(world_position);

            // 2. Make sure our variable for pixel array is updated in this frame
            cur_colors = drawable_texture.GetPixels32();

            ////////////////////////////////////////////////////////////////
            // FILL IN CODE BELOW HERE

            // Do we care about the user left clicking and dragging?
            // If you don't, simply set the below if statement to be:
            //if (true)

            // If you do care about dragging, use the below if/else structure
            if (previous_drag_position == Vector2.zero)
            {
                // THIS IS THE FIRST CLICK
                // FILL IN WHATEVER YOU WANT TO DO HERE
                // Maybe mark multiple pixels to colour?
                MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
            }
            else
            {
                // THE USER IS DRAGGING
                // Should we do stuff between the previous mouse position and the current one?
                ColourBetween(previous_drag_position, pixel_pos, Pen_Width, Pen_Colour);
            }
            ////////////////////////////////////////////////////////////////

            // 3. Actually apply the changes we marked earlier
            // Done here to be more efficient
            ApplyMarkedPixelChanges();
            
            // 4. If dragging, update where we were previously
            previous_drag_position = pixel_pos;
        }



        
        // Default brush type. Has width and colour.
        // Pass in a point in WORLD coordinates
        // Changes the surrounding pixels of the world_point to the static pen_colour
        public void PenBrush(Vector2 world_point)
        {
            Vector2 pixel_pos = WorldToPixelCoordinates(world_point);

            cur_colors = drawable_texture.GetPixels32();

            if (previous_drag_position == Vector2.zero)
            {
                // If this is the first time we've ever dragged on this image, simply colour the pixels at our mouse position
                MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
            }
            else
            {
                // Colour in a line from where we were on the last update call
                ColourBetween(previous_drag_position, pixel_pos, Pen_Width, Pen_Colour);
            }
            ApplyMarkedPixelChanges();

            //Debug.Log("Dimensions: " + pixelWidth + "," + pixelHeight + ". Units to pixels: " + unitsToPixels + ". Pixel pos: " + pixel_pos);
            previous_drag_position = pixel_pos;
        }


        // Helper method used by UI to set what brush the user wants
        // Create a new one for any new brushes you implement
        public void SetPenBrush()
        {
            // PenBrush is the NAME of the method we want to set as our current brush
            current_brush = PenBrush;
        }
//////////////////////////////////////////////////////////////////////////////






        // This is where the magic happens.
        // Detects when user is left clicking, which then call the appropriate function
        void Update()
        {
            
            // Is the user holding down the left mouse button?
            bool mouse_held_down = Input.GetMouseButton(0);

            if (mouse_held_down)
            {
                if (!is_drawing_started)
                    is_drawing_started = true;

                if (is_mouse_release_counted)
                    is_mouse_release_counted = false;

                if (is_drawing_finished)
                    is_drawing_finished = false;
            }
            else
            {
                if (is_drawing_started && !is_drawing_finished)
                    is_drawing_finished = true;

                if (is_drawing_finished && !is_mouse_release_counted)
                {   
                    Debug.Log("Mousereleased");
                    StartCoroutine(CheckCoroutine());
                    is_mouse_release_counted = true;
                }

                foreach (Transform child in gameObject.transform.parent.transform)
                {
                    child.gameObject.GetComponent<Drawable>().enabled = true;
                }
            }

            if (mouse_held_down && !no_drawing_on_current_drag)
            {

                // Convert mouse coordinates to world coordinates
                Vector2 mouse_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // Check if the current mouse position overlaps our image
                Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, Drawing_Layers.value);
                if (hit != null && hit.transform != null)
                {
                    // We're over the texture we're drawing on!
                    // Use whatever function the current brush is
                    current_brush(mouse_world_position);

                    foreach (Transform child in gameObject.transform.parent.transform)
                    {
                        if (child.gameObject != hit.gameObject)
                        {
                            child.gameObject.GetComponent<Drawable>().enabled = false;
                        }
                    }
                }

                else
                {
                    // We're not over our destination texture
                    previous_drag_position = Vector2.zero;
                    if (!mouse_was_previously_held_down)
                    {
                        // This is a new drag where the user is left clicking off the canvas
                        // Ensure no drawing happens until a new drag is started
                        no_drawing_on_current_drag = true;
                    }
                }
            }
            // Mouse is released
            else if (!mouse_held_down)
            {
                previous_drag_position = Vector2.zero;
                no_drawing_on_current_drag = false;
            }
            mouse_was_previously_held_down = mouse_held_down;
        }



        // Set the colour of pixels in a straight line from start_point all the way to end_point, to ensure everything inbetween is coloured
        public void ColourBetween(Vector2 start_point, Vector2 end_point, int width, Color color)
        {
            // Get the distance from start to finish
            float distance = Vector2.Distance(start_point, end_point);
            Vector2 direction = (start_point - end_point).normalized;

            Vector2 cur_position = start_point;

            // Calculate how many times we should interpolate between start_point and end_point based on the amount of time that has passed since the last update
            float lerp_steps = 1 / distance;

            for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
            {
                cur_position = Vector2.Lerp(start_point, end_point, lerp);
                MarkPixelsToColour(cur_position, width, color);
            }
        }





        public void MarkPixelsToColour(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            // Figure out how many pixels we need to colour in each direction (x and y)
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            //int extra_radius = Mathf.Min(0, pen_thickness - 2);

            for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
            {
                // Check if the X wraps around the image, so we don't draw pixels on the other side of the image
                if (x >= (int)drawable_sprite.rect.width || x < 0)
                    continue;

                for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
                {
                    MarkPixelToChange(x, y, color_of_pen);
                }
            }
        }
        public void MarkPixelToChange(int x, int y, Color color)
        {
            // Need to transform x and y coordinates to flat coordinates of array
            int array_pos = y * (int)drawable_sprite.rect.width + x;

            // Check if this is a valid position
            if (array_pos > cur_colors.Length || array_pos < 0)
                return;

            cur_colors[array_pos] = color;
        }
        public void ApplyMarkedPixelChanges()
        {
            drawable_texture.SetPixels32(cur_colors);
            drawable_texture.Apply();
        }


        // Directly colours pixels. This method is slower than using MarkPixelsToColour then using ApplyMarkedPixelChanges
        // SetPixels32 is far faster than SetPixel
        // Colours both the center pixel, and a number of pixels around the center pixel based on pen_thickness (pen radius)
        public void ColourPixels(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            // Figure out how many pixels we need to colour in each direction (x and y)
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            //int extra_radius = Mathf.Min(0, pen_thickness - 2);

            for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
            {
                for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
                {
                    drawable_texture.SetPixel(x, y, color_of_pen);
                }
            }

            drawable_texture.Apply();
        }


        public Vector2 WorldToPixelCoordinates(Vector2 world_position)
        {
            // Change coordinates to local coordinates of this image
            Vector3 local_pos = transform.InverseTransformPoint(world_position);

            // Change these to coordinates of pixels
            float pixelWidth = drawable_sprite.rect.width;
            float pixelHeight = drawable_sprite.rect.height;
            float unitsToPixels = pixelWidth / drawable_sprite.bounds.size.x * transform.localScale.x;

            // Need to center our coordinates
            float centered_x = local_pos.x * unitsToPixels + pixelWidth / 2;
            float centered_y = local_pos.y * unitsToPixels + pixelHeight / 2;

            // Round current mouse position to nearest pixel
            Vector2 pixel_pos = new Vector2(Mathf.RoundToInt(centered_x), Mathf.RoundToInt(centered_y));

            return pixel_pos;
        }


        // Changes every pixel to be the reset colour
        public void ResetCanvas()
        {
            drawable_texture.SetPixels(clean_colours_array);
            drawable_texture.Apply();
        }


        
        void Awake()
        {
            drawable = this;
            // DEFAULT BRUSH SET HERE
            current_brush = PenBrush;

            drawable_sprite = this.GetComponent<SpriteRenderer>().sprite;
            drawable_texture = drawable_sprite.texture;

            // Initialize clean pixels to use
            clean_colours_array = new Color[(int)drawable_sprite.rect.width * (int)drawable_sprite.rect.height];
            for (int x = 0; x < clean_colours_array.Length; x++)
                clean_colours_array[x] = Reset_Colour;

            // Should we reset our canvas image when we hit play in the editor?
            if (Reset_Canvas_On_Play)
                ResetCanvas();
        }


        // —–¿¬Õ»¬¿≈Ã  ¿–“»Õ »
        public void Comparison()
        {
            float blackPixels = 0, blackPixels1 = 0, blackPixels2 = 0, whitePixels = 0, whitePixels1 = 0, whitePixels2 = 0, white1black2 = 0, wrongColorPixels = 0;
            int texWidht = (int)sprite.width;
            int texHeight = (int)sprite.height;

            Color transp = new Color(0f, 0f, 0f, 0f);


            for (int i = 0; i < texWidht; i++)
            {
                for (int ii = 0; ii < texHeight; ii++)
                {

                    if (drawing.GetPixel(i, ii) != transp && 
                        (drawing.GetPixel(i, ii).r < (targetColor.r - 0.1f) ||
                        drawing.GetPixel(i, ii).r > (targetColor.r + 0.1f) ||
                        drawing.GetPixel(i, ii).g < (targetColor.g - 0.1f) ||
                        drawing.GetPixel(i, ii).g > (targetColor.g + 0.1f) ||
                        drawing.GetPixel(i, ii).b < (targetColor.b - 0.1f) ||
                        drawing.GetPixel(i, ii).b > (targetColor.b + 0.1f)))
                    {
                        wrongColorPixels++;
                    }

                    if (sprite.GetPixel(i, ii) != transp)
                    {
                        blackPixels1++;
                    }
                    if (drawing.GetPixel(i, ii) != transp)
                    {
                        blackPixels2++;
                    }
                    if (sprite.GetPixel(i, ii) == transp)
                    {
                        whitePixels1++;
                    }
                    if (drawing.GetPixel(i, ii) == transp)
                    {
                        whitePixels2++;
                    }

                    if (sprite.GetPixel(i, ii) == transp && drawing.GetPixel(i, ii) != transp)
                    {
                        white1black2++;
                    }


                    if (drawing.GetPixel(i, ii) != transp && sprite.GetPixel(i, ii) != transp)
                    {
                        blackPixels++;
                    }
                    if (drawing.GetPixel(i, ii) == transp && sprite.GetPixel(i, ii) == transp)
                    {
                        whitePixels++;
                    }

                }
            }

            if (blackPixels2 < 100)
            {
                Debug.Log("NOTHING     " + blackPixels / blackPixels1 * 100 + "%" + "     " + white1black2 / whitePixels1 * 100 + "%" + "     " + blackPixels + " " + blackPixels1);
                drawableSprite.GetComponent<Drawable>().ResetCanvas();
            }
            else if (wrongColorPixels > 1f)
            {
                Debug.Log("WRONG COLOR     " + wrongColorPixels);
                wrongColorState = 1;
                StartCoroutine(PictureErase());

                //DIALOG
                dialog.GetComponent<Dialogs>().ShowFailureText(id, wrongColorState, mistakesCounter);
            }
            else if ((blackPixels / blackPixels1) < (truePercent / 2f) || (white1black2 / whitePixels1) > (falsePercent * 2f))
            {
                Debug.Log("VERY BAD     " + blackPixels / blackPixels1 * 100 + "%" + "     " + white1black2 / whitePixels1 * 100 + "%" + "     " + white1black2 + " " + whitePixels1);
                wrongColorState = 0;
                mistakesCounter += 1.5f;
                StartCoroutine(PictureErase());

                if (mistakesCounter >= 3f && mistakesCounter < 4f)
                {
                    hintSprite.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.7f);
                }
                else if (mistakesCounter >= 4f && mistakesCounter < 7f)
                {
                    hintSprite.GetComponent<SpriteRenderer>().color += new Color(0f, 0f, 0f, 0.05f);
                }

                //DIALOG
                dialog.GetComponent<Dialogs>().ShowBigFailureText(id, wrongColorState, mistakesCounter);
            }
            else if (blackPixels / blackPixels1 < truePercent || white1black2 / whitePixels1 > falsePercent)
            {
                Debug.Log("BAD     " + blackPixels / blackPixels1 * 100 + "%" + "     " + white1black2 / whitePixels1 * 100 + "%" + "     " + white1black2 + " " + whitePixels1);
                wrongColorState = 0;
                mistakesCounter += 1f;
                StartCoroutine(PictureErase());

                if (mistakesCounter >= 3f && mistakesCounter < 4f)
                {
                    hintSprite.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.7f);
                }
                else if (mistakesCounter >= 4f && mistakesCounter < 7f)
                {
                    hintSprite.GetComponent<SpriteRenderer>().color += new Color(0f, 0f, 0f, 0.05f);
                }

                //DIALOG
                dialog.GetComponent<Dialogs>().ShowFailureText(id, wrongColorState, mistakesCounter);
            }
            else
            {
                Debug.Log("GOOD     " + blackPixels / blackPixels1 * 100 + "%" + "     " + white1black2 / whitePixels1 * 100 + "%" + "     " + blackPixels + " " + blackPixels1);
                StartCoroutine(PictureComplete());

                //DIALOG
                dialog.GetComponent<Dialogs>().ShowSuccessText(id);

                hintSprite.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
            }

            blackPixels = 0; blackPixels1 = 0; blackPixels2 = 0; whitePixels = 0; whitePixels1 = 0; whitePixels2 = 0; white1black2 = 0; wrongColorPixels = 0;

        }



        // œ–Œ¬≈–ﬂ≈Ã, —Œ¬œ¿ƒ¿≈“ À»  ¿–“»Õ ¿
        IEnumerator CheckCoroutine()
        {
            yield return new WaitForSeconds(0.05f);
            Comparison();
            
        }



        // «¿Ã≈Õﬂ≈Ã ¬ —À”◊¿≈ ”—œ≈’¿
        IEnumerator PictureComplete()
        {
            cursor.GetComponent<CursorMovement>().SetNoCursor();

            while (realSprite.GetComponent<SpriteRenderer>().color.a < 1f)
            {
                yield return new WaitForSeconds(0.05f);
                realSprite.GetComponent<SpriteRenderer>().color += new Color(0f, 0f, 0f, 0.05f);
                gameObject.GetComponent<SpriteRenderer>().color -= new Color(0f, 0f, 0f, 0.05f);
            }
            if (gameObject.GetComponent<SpriteRenderer>().color.a <= 0.01f)
            {
                gameObject.SetActive(false);
            }

            foreach (GameObject obj in drawSettings.GetComponent<DrawingSettings>().meloks)
            {
                if (!obj.activeSelf)
                {
                    obj.gameObject.GetComponent<Image>().enabled = false;
                    obj.gameObject.GetComponent<Pencil>().enabled = false;
                }
            }
        }

        // —“»–¿≈Ã ¬ —À”◊¿≈ œ–Œ¬¿À¿
        IEnumerator PictureErase()
        {
            cursor.GetComponent<CursorMovement>().SetNoCursor();

            yield return new WaitForSeconds(1f);

            eraserUI.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            eraser.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

            while (gameObject.GetComponent<SpriteRenderer>().color.a > 0.1f)
            {
                yield return new WaitForSeconds(0.05f);
                gameObject.GetComponent<SpriteRenderer>().color -= new Color(0f, 0f, 0f, 0.025f);
            }

            drawableSprite.GetComponent<Drawable>().ResetCanvas();
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

            eraserUI.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            eraser.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        }

    }
}