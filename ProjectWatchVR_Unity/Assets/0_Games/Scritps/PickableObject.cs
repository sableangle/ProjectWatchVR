using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    // cakeslice.Outline outline;
    Rigidbody rigibody;
    void Awake()
    {
        // outline = gameObject.AddComponent<cakeslice.Outline>();
        // outline.eraseRenderer = true;

        rigibody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        Renderers = GetComponentsInChildren<Renderer>();

        foreach (var renderer in Renderers)
        {
            _materials.AddRange(renderer.materials);
        }
    }
    void Update()
    {
        ApplyGlow();
    }

    public void OnPointEnter()
    {
        _targetColor = GlowColor;
    }

    public void OnPointOut()
    {
        _targetColor = Color.black;
    }

    public void OnPickStart()
    {
        if (rigibody) rigibody.isKinematic = true;
    }
    public void OnPickFinish()
    {
        if (rigibody) rigibody.isKinematic = false;
    }

    //Glow Effect
    [SerializeField]
    private Color GlowColor = Color.yellow;
    public float LerpFactor = 10;

    public Renderer[] Renderers
    {
        get;
        private set;
    }

    public Color CurrentColor
    {
        get { return _currentColor; }
    }

    private List<Material> _materials = new List<Material>();
    private Color _currentColor;
    private Color _targetColor;
    private void ApplyGlow()
    {
        _currentColor = Color.Lerp(_currentColor, _targetColor, Time.deltaTime * LerpFactor);

        for (int i = 0; i < _materials.Count; i++)
        {
            _materials[i].SetColor("_GlowColor", _currentColor);
        }

        if (_currentColor.Equals(_targetColor))
        {
            return;
        }
    }
}
