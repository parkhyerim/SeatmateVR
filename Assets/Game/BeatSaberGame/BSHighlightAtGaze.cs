// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using Tobii.G2OM;
using UnityEngine;


    //Monobehaviour which implements the "IGazeFocusable" interface, meaning it will be called on when the object receives focus
    public class BSHighlightAtGaze : MonoBehaviour, IGazeFocusable
    {
       // public GameManager gameManager;
        private BSGameManager gameManager;

        private static readonly int _baseColor = Shader.PropertyToID("_BaseColor");
        public Color highlightColor = Color.red;
        public float animationTime = 0.1f;

        private Renderer _renderer;
        private Color _originalColor;
        private Color _targetColor;
        private bool isFocusOnRegistered;

    public bool IsFocusOnRegistered { get => isFocusOnRegistered; set => isFocusOnRegistered = value; }

    private void Awake()
        {
            gameManager = FindObjectOfType<BSGameManager>();
        }
    //The method of the "IGazeFocusable" interface, which will be called when this object receives or loses focus
    public void GazeFocusChanged(bool hasFocus)
    {
        //If this object received focus, fade the object's color to highlight color
        if (hasFocus)
        {
               
            if (!isFocusOnRegistered)
            {
                // _targetColor = highlightColor;
                gameManager.EyeFocused(true, this.gameObject.name);
                isFocusOnRegistered = true;
            }     
        }
        //If this object lost focus, fade the object's color to it's original color
        else
        {
            _targetColor = _originalColor;
            if (isFocusOnRegistered)
            {
                gameManager.EyeFocused(false, this.gameObject.name);
                isFocusOnRegistered = false;
            }
        }
    }

    private void Start()
    {
        //_renderer = GetComponent<Renderer>();
        //_originalColor = _renderer.material.color;
        //_targetColor = _originalColor;
    }

    //private void Update()
    //{
    //    //This lerp will fade the color of the object
    //    if (_renderer.material.HasProperty(_baseColor)) // new rendering pipeline (lightweight, hd, universal...)
    //    {
    //        _renderer.material.SetColor(_baseColor, Color.Lerp(_renderer.material.GetColor(_baseColor), _targetColor, Time.deltaTime * (1 / animationTime)));
    //    }
    //    else // old standard rendering pipline
    //    {
    //        _renderer.material.color = Color.Lerp(_renderer.material.color, _targetColor, Time.deltaTime * (1 / animationTime));
    //    }
    //}
    }

