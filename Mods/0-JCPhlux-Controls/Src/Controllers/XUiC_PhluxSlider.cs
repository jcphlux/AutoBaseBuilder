using System;
using UnityEngine;

public delegate void XUiEvent_PhluxSliderValueChanged(XUiC_PhluxSlider _sender);

public class XUiC_PhluxSlider : XUiController
{
    public string Tag;
    protected XUiC_PhluxSliderBar barController;
    protected bool initialized;
    protected float left = float.NaN;
    protected float maxVal = 1f;
    protected float minVal = 0f;
    protected string name;
    protected XUiC_PhluxSliderThumb thumbController;
    protected Func<float, string> valueFormatter;
    protected float width;
    private readonly CachedStringFormatterFloat internalValueFormatter = new("0.00");
    private float sliderStep = 0.1f;
    private float sliderVal = 0f;
    private float step = 0.1f;
    private float val = 0f;
    private bool visible = true;

    public event XUiEvent_PhluxSliderValueChanged OnValueChanged;

    public bool IsVisible => Visible;

    public string Label
    {
        get => name;
        set
        {
            name = value;
            IsDirty = true;
        }
    }

    public float SliderStep
    {
        get => sliderStep;
        set
        {
            step = Mathf.Clamp((maxVal - minVal) * value, minVal, maxVal);
            sliderStep = Mathf.Clamp01(value);
        }
    }

    public float SliderValue
    {
        get => sliderVal;
        set
        {
            float newVal = Mathf.Lerp(minVal, maxVal, value);
            if ((double)newVal == val && (double)value == sliderVal)
                return;
            val = newVal;
            sliderVal = Mathf.Clamp01(value);
            if (!thumbController.IsDragging)
                UpdateThumb();
            IsDirty = true;
        }
    }

    public float Step
    {
        get => step;
        set
        {
            step = Mathf.Clamp(value, minVal, maxVal);
            sliderStep = Mathf.Clamp01(value / (maxVal - minVal + 1));
        }
    }

    public float Value
    {
        get => val;
        set
        {
            float newSliderVal = Mathf.InverseLerp(minVal, maxVal, value);
            if ((double)value == val && (double)newSliderVal == sliderVal)
                return;
            val = Mathf.Clamp(value, minVal, maxVal);
            sliderVal = newSliderVal;
            if (!thumbController.IsDragging)
                UpdateThumb();
            IsDirty = true;
        }
    }

    public Func<float, string> ValueFormatter
    {
        get => valueFormatter;
        set
        {
            valueFormatter = value;
            IsDirty = true;
        }
    }

    protected bool Visible
    {
        get => visible; set
        {
            visible = value;
            RefreshBindingsSelfAndChildren();
        }
    }

    public override bool GetBindingValue(ref string value, string bindingName)
    {
        switch (bindingName)
        {
            case "visible":
                value = Visible ? "True" : "False";
                return Visible;

            case "name":
                value = name;
                return true;

            case nameof(value):
                value = valueFormatter == null ? internalValueFormatter.Format(val) : valueFormatter(val);
                return true;

            default:
                return false;
        }
    }

    public void Hide() => Visible = false;

    public override void Init()
    {
        base.Init();
        thumbController = GetChildById("thumb") as XUiC_PhluxSliderThumb;
        if (thumbController == null)
        {
            Log.Error("Thumb slider not found!");
        }
        else
        {
            barController = GetChildById("bar") as XUiC_PhluxSliderBar;
            if (barController == null)
            {
                Log.Error("Thumb bar not found!");
            }
            else
            {
                left = (float)barController.ViewComponent.Position.x;
                width = (float)barController.ViewComponent.Size.x;
                thumbController.SetDimensions(left, width);
            }
        }
    }

    public void Reset() => initialized = false;

    public void Set(float value = 0f, float min = 0f, float max = 1f, float step = 0.1f)
    {
        minVal = Mathf.Min(min, max);
        maxVal = Mathf.Max(min, max);
        Step = step;
        Value = value;
    }

    public void SetAndShow(float value = 0f, float min = 0f, float max = 1f, float step = 0.1f)
    {
        Show();
        Set(value, min, max, step);
    }

    public void Show() => Visible = true;

    public override void Update(float _dt)
    {
        base.Update(_dt);
        if (thumbController == null || thumbController.ViewComponent == null || float.IsNaN(left) || !IsDirty)
            return;
        RefreshBindings();
        IsDirty = false;
    }

    internal void SliderValueChanged(float _newVal)
    {
        SliderValue = _newVal;
        if (OnValueChanged == null)
            return;
        OnValueChanged(this);
    }

    internal void UpdateThumb() => thumbController.ThumbPosition = SliderValue;
}