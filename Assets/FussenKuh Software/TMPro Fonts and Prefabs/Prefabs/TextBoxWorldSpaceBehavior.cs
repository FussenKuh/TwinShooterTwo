using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class TextBoxWorldSpaceBehavior : MonoBehaviour
{
    [SerializeField]
    bool debugTesting = false;

    [Header("Background Information:")]
    [SerializeField]
    float _width = 1f;
    [SerializeField]
    float _height = 1f;
    [SerializeField]
    Sprite _image;

    [Header("Content Information:")]
    [SerializeField]
    [TextArea]
    string _text = "";
    [SerializeField]
    float _leftMargin = 0.2f;
    [SerializeField]
    float _topMargin = 0.2f;
    [SerializeField]
    float _rightMargin = 0.2f;
    [SerializeField]
    float _bottomMargin = 0.2f;

    [SerializeField]
    TMP_FontAsset _font = null;
    [SerializeField]
    Material _fontMaterialPreset = null;


    // These items were manually set in the inspector, then the [HideInInspector] 
    // tag was applied since the user should never alter them
    [HideInInspector]
    [SerializeField]
    RectTransform _background = null;
    [HideInInspector]
    [SerializeField]
    TMP_Text _tmpText = null;
    [HideInInspector]
    [SerializeField]
    Image _backgroundImage = null;

    /// <summary>
    /// The width of the text box
    /// </summary>
    public float  Width { get { return _width;  } set { _width  = value; Resize(); } }
    /// <summary>
    /// The height of the text box
    /// </summary>
    public float Height { get { return _height; } set { _height = value; Resize(); } }
    /// <summary>
    /// The image used for the background of the text box
    /// </summary>
    public Sprite Image { get { return _image;  } set { _image  = value; _backgroundImage.sprite = _image; } }

    /// <summary>
    /// The text displayed in the text box
    /// </summary>
    public string Text { get { return _text;   } set { _text = value;  _tmpText.SetText(_text); } }

    /// <summary>
    /// The left margin for the text in the text box
    /// </summary>
    public float LeftMargin   { get { return _leftMargin; } set { _leftMargin = value; Resize(); } }
    /// <summary>
    /// The top margin for the text in the text box
    /// </summary>
    public float TopMargin    { get { return _topMargin; } set { _topMargin = value; Resize(); } }
    /// <summary>
    /// The right margin for the text in the text box
    /// </summary>
    public float RightMargin  { get { return _rightMargin; } set { _rightMargin = value; Resize(); } }
    /// <summary>
    /// The bottom margin for the text in the text box
    /// </summary>
    public float BottomMargin { get { return _bottomMargin; } set { _bottomMargin = value; Resize(); } }

    /// <summary>
    /// The Text Mesh Pro font to use for the text
    /// </summary>
    public TMP_FontAsset Font { get { return _font; } set { _font = value; UpdateFont(); } }
    /// <summary>
    /// The override font material preset to use for the text
    /// </summary>
    public Material FontMaterialPreset { get { return _fontMaterialPreset; } set { _fontMaterialPreset = value; UpdateFont(); } }

    #region Private
    void Resize()
    {
        _background.sizeDelta = new Vector2(_width, _height);
        Vector4 tmp = new Vector4(_leftMargin, _topMargin, _rightMargin, _bottomMargin);
        _tmpText.margin = tmp;
    }

    void UpdateFont()
    {
        if (_tmpText != null) { _tmpText.font = _font; }
        if (_fontMaterialPreset != null & _tmpText != null) { _tmpText.fontMaterial = _fontMaterialPreset; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _tmpText.SetText(_text);
        Resize();
        if (_tmpText != null) { _tmpText.font = _font; }
        if (_fontMaterialPreset != null & _tmpText != null) { _tmpText.fontMaterial = _fontMaterialPreset; }
    }

    // Update is called once per frame
    void Update()
    {
        if (debugTesting)
        {
            _tmpText.SetText(_text);
            _backgroundImage.sprite = _image;
            Resize();
            UpdateFont();
        }

    }

    #endregion
}
