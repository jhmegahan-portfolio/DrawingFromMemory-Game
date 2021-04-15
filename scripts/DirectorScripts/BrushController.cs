using UnityEngine;

public class BrushController : DirectorAccess {
    #region variables, global
    // brush variables
    private static int brushWidth;
    private static int brushHeight;
    private static Color brushColor;
    private static Color[] brush;
    private static SpriteRenderer cursorSprite;
    private static GameObject cursorObj;

    // exposed to inspector, for designers to alter
    [SerializeField] int brushSizeDefault = 25;
    [SerializeField] Sprite defaultCursorTexture;
    [SerializeField] CrayonColors.Color brushColorStateDefault = CrayonColors.Color.RED;

    private struct OldLine {
        public float endX;
        public float endY;
        public float ctrlTangetLength;
        public Vector2 direction;

        public OldLine(float endX, float endY, float ctrlTangetLength, Vector2 direction) {
            this.endX = endX;
            this.endY = endY;
            this.ctrlTangetLength = ctrlTangetLength;
            this.direction = direction;
        }
    } OldLine oldLine;

    bool paintLastFrame = false;

    [SerializeField] float percentInfluence = 0.1f;
    static private LayerMask paintLayer;

    #endregion variables, global
    #region MonoBehavior
    private void Start() {
        // setup the brush
        SetBrushColor(brushColorStateDefault);
        SetBrushSize(brushSizeDefault);
        oldLine = new OldLine(0f, 0f, 0f, Vector2.zero);

        // setup the mouse cursor
        cursorObj = new GameObject("Cursor");
        cursorObj.AddComponent<SpriteRenderer>();
        cursorSprite = cursorObj.GetComponent<SpriteRenderer>();
        SetCursorTexture(defaultCursorTexture);
        cursorSprite.sortingLayerName = "Cursor";
        Vector3 paperScale = GameObject.FindGameObjectWithTag("Paper").GetComponent<Transform>().localScale;
        Vector3 currentCursorScale = cursorObj.transform.localScale;
        float newScaleX = paperScale.x * currentCursorScale.x;
        float newScaleY = paperScale.y * currentCursorScale.y;
        cursorObj.transform.localScale = new Vector3(newScaleX, newScaleY, 1f);
        cursorObj.SetActive(false);

        // clear all the paintings
        Clear();

        paintLayer = (1 << LayerMask.NameToLayer("Paint"));
    }

    private void Update() {
        UpdateCursorPos();
        PaintMouseInputAtInBoundCoord();
    }
    #endregion MonoBehavior
    #region set methods
    // called from UI buttons - set the new brush size and call to update the brush
    public void SetBrushSize(int size) {
        brushHeight = size;
        brushWidth = size;
        SetBrush();
    }

    // set new mouse cursor texture that appears over the painting space
    public void SetCursorTexture(Sprite tex) {
        cursorSprite.sprite = tex;
    }

    // called from the UI buttons - set the new brush color and call to update the brush
    public void SetBrushColor(CrayonColors.Color c) {
        switch (c) {
            case CrayonColors.Color.RED:
                brushColor = new Color(0.7f, 0f, 0f, 0.7f);
                break;
            case CrayonColors.Color.ORANGE:
                brushColor = new Color(1f, 0.3f, 0f, 0.7f);
                break;
            case CrayonColors.Color.YELLOW:
                brushColor = new Color(0.9f, 0.9f, 0f, 0.7f);
                break;
            case CrayonColors.Color.GREEN:
                brushColor = new Color(0f, 0.5f, 0f, 0.7f);
                break;
            case CrayonColors.Color.BLUE:
                brushColor = new Color(0.3f, 0.5f, 0.9f, 0.7f);
                break;
            case CrayonColors.Color.PURPLE:
                brushColor = new Color(0.5f, 0f, 0.5f, 0.7f);
                break;
            case CrayonColors.Color.BROWN:
                brushColor = new Color(0.5f, 0.2f, 0f, 0.7f);
                break;
            case CrayonColors.Color.ERASE:
                brushColor = new Color(0f, 0f, 0f, 0f);
                break;
            default:
                break;
        }
        SetBrush();
    }

    // for each pixel in the brush, set to brush color
    private void SetBrush() {
        brush = new Color[GetBrushWidth() * GetBrushHeight()];
        for (int i = 0; i < brush.Length; i++) {
            brush[i] = GetBrushColor();
        }
    }
    #endregion set methods
    #region get methods
    private int GetBrushWidth() {
        return brushWidth;
    }

    private int GetBrushHeight() {
        return brushHeight;
    }

    private Sprite GetCursorTexture() {
        return cursorSprite.sprite;
    }

    private Color GetBrushColor() {
        return brushColor;
    }

    private Color[] GetBrush() {
        return brush;
    }
    #endregion get methods
    #region methods
    private void UpdateCursorPos() {
        Vector3 cursorPos = Input.mousePosition;
        cursorPos.z = Camera.main.nearClipPlane;
        cursorPos = Camera.main.ScreenToWorldPoint(cursorPos);
        cursorObj.transform.position = cursorPos;
    }

    private void PaintMouseInputAtInBoundCoord() {
        // check if raycast intersects with permitted paint space layer
        RaycastHit hit;
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool inBounds = Physics.Raycast(cameraRay, out hit, Mathf.Infinity, paintLayer.value);
        TurnOnInBoundsCursor(inBounds);

        if (inBounds) {
            // convert UV coordinate to a pixel coordinate
            Texture2D paperTex = hit.collider.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
            Vector2 paperPos = ConvertUVToPixelCoord(hit.textureCoord, paperTex);

            /*bool xPosChanged = (paperPos.x != oldLine.endX);
            bool yPosChanged = (paperPos.y != oldLine.endY);
            bool posChanged = (xPosChanged || yPosChanged);*/

            // paint if input conditions met
            if (Input.GetMouseButton(0) /*&& posChanged*/) {
                PaintMouseInput(paperPos, paperTex);
            } else {
                paintLastFrame = false;
            }

        } else {
            paintLastFrame = false;
        }
    }

    private void TurnOnInBoundsCursor(bool b) {
        Cursor.visible = !b;
        cursorObj.SetActive(b);
    }

    private Vector2 ConvertUVToPixelCoord(Vector2 uvCoord, Texture2D texture) {
        Texture2D paperTex = texture;
        Vector2 paperUV = uvCoord;

        float x = paperUV.x * paperTex.width;
        float y = paperUV.y * paperTex.height;
        Vector2 paperPos = new Vector2(x, y);

        return paperPos;
    }

    private void PaintMouseInput(Vector2 pixelCoord, Texture2D texture) {
        Vector2 paperPos = pixelCoord;
        Texture2D paperTex = texture;

        if (!paintLastFrame) {
            // overwrite old line data for new non-contiguous lines
            OverwriteOldLine(paperPos, paperPos, paperPos);
        }

        //  only if the brush width/height is in bounds
        if (BrushAtCoordIsInsideTexture(paperPos, paperTex)) {

            paintLastFrame = true;

            // Find start, end, and control points for a Bezier curve to smooth the brush
            Vector2 P0 = new Vector2(oldLine.endX, oldLine.endY);
            Vector2 P1 = P0 + (oldLine.direction * (oldLine.ctrlTangetLength * percentInfluence));
            Vector2 P2 = paperPos;

            // Approximate the length of the curve, measured in pixels
            float chord = (P2 - P0).magnitude;
            float net = (P0 - P1).magnitude + (P2 - P1).magnitude;
            float arcLength = (net + chord) / 2;

            PaintLine(P0, P1, P2, arcLength, paperTex);
            OverwriteOldLine(P1, P2, paperPos);
        }
    }

    private void PaintLine(Vector2 startPoint, Vector2 controlPoint, Vector2 endPoint, float arcLength, Texture2D texture) {
        Vector2 p0 = startPoint;
        Vector2 p1 = controlPoint;
        Vector2 p2 = endPoint;
        float length = arcLength;
        Texture2D tex = texture;

        // paint in the gaps, between recorded mouse positions after each frame update
        // increment in steps over lentgh of a bezier curve, paint new pixels at each step, and update brush position for next step
        for (int i = 0; i <= length; i++) {
            //t = percent along curve
            float t = (float)i / (float)length;

            // Calculate Quadratic Bezier Curve -- B(t) = P1 + (1 - t)^2(P0 - P1) + t^2(P2 - P1)
            Vector2 paintPos = p1 + Mathf.Pow((1f - t), 2f) * (p0 - p1) + Mathf.Pow(t, 2f) * (p2 - p1);

            if (BrushAtCoordIsInsideTexture(paintPos, tex)) {
                tex.SetPixels((int)paintPos.x - (GetBrushWidth() / 2), (int)paintPos.y - (GetBrushHeight() / 2), GetBrushWidth(), GetBrushHeight(), GetBrush());
            }
        }
        tex.Apply();
    }

    private void OverwriteOldLine(Vector2 controlPoint, Vector2 endpoint, Vector2 pixelCoordinates) {
        Vector2 p1 = controlPoint;
        Vector2 p2 = endpoint;
        Vector2 paperPos = pixelCoordinates;
        OldLine ol = new OldLine();

        // normalize the vector between the control and end points to find direction for the next control
        Vector2 endVector = (p2 - p1);
        if ((int)endVector.magnitude == 0) {
            ol.direction = Vector2.zero;
        } else {
            ol.direction = endVector.normalized;
        }

        ol.ctrlTangetLength = endVector.magnitude;
        ol.endX = paperPos.x;
        ol.endY = paperPos.y;

        oldLine = ol;
    }

    private bool BrushAtCoordIsInsideTexture(Vector2 coordinate, Texture2D texture) {
        Vector2 v = coordinate;
        Texture2D t = texture;
        bool xWithinUpper = v.x + (GetBrushWidth() / 2f) <= t.width;
        bool xWithinLower = v.x - (GetBrushWidth() / 2f) > 0;
        bool yWithinUpper = v.y + (GetBrushHeight() / 2f) <= t.height;
        bool yWithinLower = v.y - (GetBrushHeight() / 2f) > 0;

        bool b = (xWithinUpper && xWithinLower && yWithinUpper && yWithinLower);
        return (b);
    }

    private void FillTextureWithColor(Texture2D tex, Color c) {
        Color[] col = new Color[tex.width * tex.height];
        for (int i = 0; i < col.Length; i++) {
            col[i] = c;
        }
        tex.SetPixels(col);
        tex.Apply();
    }

    public void Clear() {
        GameObject[] paintings = GameObject.FindGameObjectsWithTag("Paint");
        foreach (GameObject painting in paintings) {
            Texture2D t = painting.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
            FillTextureWithColor(t, Color.clear);
        }
    }
}
#endregion methods
