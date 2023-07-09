using UnityEngine;

public class XUiC_PhluxSliderBar : XUiController
{
    private XUiC_PhluxSlider sliderController;

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

    public override void Init()
    {
        base.Init();
        sliderController = GetParentByType<XUiC_PhluxSlider>();
    }

    protected override void OnPressed(int _mouseButton)
    {
        Vector2i mouseXuiPosition = xui.GetMouseXUIPosition();
        XUiController xuiController = this;
        Vector2i position = xuiController.ViewComponent.Position;
        while (xuiController.Parent != null && xuiController.Parent.ViewComponent != null)
        {
            xuiController = xuiController.Parent;
            position += xuiController.ViewComponent.Position;
        }
        Vector2i vector2i = position + new Vector2i((int)xuiController.ViewComponent.UiTransform.parent.localPosition.x, (int)xuiController.ViewComponent.UiTransform.parent.localPosition.y);
        int num = (vector2i + ViewComponent.Size).x - vector2i.x;
        sliderController.SliderValueChanged((float)(mouseXuiPosition.x - vector2i.x) / num);
    }

    protected override void OnScrolled(float _delta)
    {
        base.OnScrolled(_delta);
        sliderController.SliderValueChanged(sliderController.SliderValue + Mathf.Clamp(_delta, -sliderController.SliderStep, sliderController.SliderStep));
    }
}