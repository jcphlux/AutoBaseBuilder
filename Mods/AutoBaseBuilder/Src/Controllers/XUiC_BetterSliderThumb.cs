using UnityEngine;

public class XUiC_BetterSliderThumb : XUiController
{
    private Vector2i lastMousePos = new(-100000, -100000);
    private bool isOver;
    private bool isDragging;
    private float left;
    private float width;
    private XUiC_BetterSlider sliderController;
    private XUiC_BetterSliderBar sliderBarController;

    public float ThumbPosition
    {
        get => (ViewComponent.UiTransform.localPosition.x - left) / width;
        set
        {
            ViewComponent.Position = new Vector2i((int)((double)value * width + left), ViewComponent.Position.y);
            ViewComponent.UiTransform.localPosition = new Vector3((int)((double)value * width + left), ViewComponent.Position.y, 0.0f);
        }
    }

    public bool IsDragging => isDragging;

    public override void Init()
    {
        base.Init();
        ViewComponent.EventOnHover = true;
        sliderController = GetParentByType<XUiC_BetterSlider>();
        sliderBarController = sliderController.GetChildByType<XUiC_BetterSliderBar>();
    }

    public void SetDimensions(float _left, float _width)
    {
        left = _left;
        width = _width;
    }

    public override bool GetBindingValue(ref string value, string bindingName)
    {
        switch (bindingName)
        {
            case "visible":
                bool isVisible = sliderController != null && sliderController.IsVisible;
                value = isVisible ? "True" : "False";
                return isVisible;
            default:
                return false;
        }
    }

    protected override void OnHovered(bool _isOver)
    {
        base.OnHovered(_isOver);
        isOver = _isOver;
    }

    protected override void OnDragged(EDragType _dragType, Vector2 _mousePositionDelta)
    {
        base.OnDragged(_dragType, _mousePositionDelta);
        if (!isDragging && !isOver)
            return;
        Vector2i mouseXuiPosition = xui.GetMouseXUIPosition();
        switch (_dragType)
        {
            case EDragType.DragStart:
                lastMousePos = mouseXuiPosition;
                isDragging = true;
                break;
            case EDragType.DragEnd:
                isDragging = false;
                break;
        }
        if (mouseXuiPosition.x - lastMousePos.x == 0)
            return;
        float num = Mathf.Clamp(ViewComponent.UiTransform.localPosition.x + (mouseXuiPosition.x - lastMousePos.x), left, left + width);
        lastMousePos = mouseXuiPosition;
        ViewComponent.UiTransform.localPosition = new Vector3(num, ViewComponent.UiTransform.localPosition.y, ViewComponent.UiTransform.localPosition.z);
        ViewComponent.Position = new Vector2i((int)num, ViewComponent.Position.y);
        sliderController.SliderValueChanged(ThumbPosition);
    }

    protected override void OnScrolled(float _delta)
    {
        base.OnScrolled(_delta);
        if (sliderBarController == null)
            return;
        sliderBarController.Scrolled(_delta);
    }
}