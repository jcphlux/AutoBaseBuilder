using System;
using UnityEngine;

public class XUiC_ABBPrefabList : XUiController
{
    public static string ID = "";
    private XUiC_ABBPrefabFileList fileList;
    private XUiV_Texture prefabPreview;
    private XUiV_Label noPreviewLabel;
    private XUiC_SimpleButton btnPreview;
    private XUiC_BetterSlider sliderHorizontalOffset;
    private XUiC_BetterSlider sliderVerticalOffset;
    private XUiC_BetterSlider sliderFacing;
    private Vector3i prefabOffset = Vector3i.zero;
    private byte prefabRotation = 0;
    private ABBPrefabInstance prefabInstance;
    private TileEntityAutoBaseBuilder tileEntity;
    private byte Rotation => tileEntity?.blockValue.rotation switch
    {
        0 or 27 => 0,
        1 or 26 => 1,
        2 or 25 => 2,
        3 or 24 => 3,
        _ => throw new ArgumentOutOfRangeException(),
    };

    public override void Init()
    {
        base.Init();
        ID = WindowGroup.ID;
        prefabPreview = (XUiV_Texture)GetChildById("prefabPreview").ViewComponent;
        noPreviewLabel = (XUiV_Label)GetChildById("noPreview").ViewComponent;
        noPreviewLabel.IsVisible = false;

        fileList = GetChildById("files") as XUiC_ABBPrefabFileList;
        fileList.SelectionChanged += OnEntrySelectionChanged;
        fileList.OnEntryDoubleClicked += OnEntryDoubleClicked;
        fileList.PageNumberChanged += OnPageNumberChanged;

        btnPreview = GetChildById("btnPreview") as XUiC_SimpleButton;
        btnPreview.Enabled = false;
        btnPreview.OnPressed += OnPreviewPressed;

        sliderHorizontalOffset = GetChildById("sliderHorizontalOffset") as XUiC_BetterSlider;
        sliderHorizontalOffset.Label = "Horizontal Offset"; // Localization.Get("xuiSliderOffset");
        sliderHorizontalOffset.Hide();
        sliderHorizontalOffset.ValueFormatter = SliderHorizontalOffset_ValueFormatter;
        sliderHorizontalOffset.OnValueChanged += OnSliderHorizontalOffsetChanged;

        sliderVerticalOffset = GetChildById("sliderVerticalOffset") as XUiC_BetterSlider;
        sliderVerticalOffset.Label = "Vertical Offset"; // Localization.Get("xuiSliderOffset");
        sliderVerticalOffset.Hide();
        sliderVerticalOffset.ValueFormatter = SliderVerticalOffset_ValueFormatter;
        sliderVerticalOffset.OnValueChanged += OnSliderVerticalOffsetChanged;

        sliderFacing = GetChildById("sliderFacing") as XUiC_BetterSlider;
        sliderFacing.Label = "Facing"; // Localization.Get("xuiSliderOffset");
        sliderFacing.Hide();
        sliderFacing.ValueFormatter = SliderFacing_ValueFormatter;
        sliderFacing.OnValueChanged += OnSliderFacingChanged;
    }

    public override void OnClose()
    {
        base.OnClose();
        ClearPrefab();
        btnPreview.Enabled = fileList.SelectedEntry != null;
    }

    public override bool GetBindingValue(ref string value, string bindingName)
    {
        switch (bindingName)
        {
            case "hasprefab":
                bool hasPrefrab = prefabInstance != null;
                value = hasPrefrab ? "True" : "False";
                return hasPrefrab;
            default:
                return base.GetBindingValue(ref value, bindingName);
        }
    }

    private void OnEntrySelectionChanged(
  XUiC_ListEntry<XUiC_PrefabFileList.PrefabFileEntry> previousEntry,
  XUiC_ListEntry<XUiC_PrefabFileList.PrefabFileEntry> newEntry)
    {
        btnPreview.Enabled = newEntry != null;
        ClearPrefab();
        if (prefabPreview.Texture != null)
        {
            Texture2D texture = (Texture2D)prefabPreview.Texture;
            prefabPreview.Texture = null;
            UnityEngine.Object.Destroy(texture);
        }
        if (newEntry?.GetEntry() != null)
        {
            string path = newEntry.GetEntry().location.FullPathNoExtension + ".jpg";
            if (SdFile.Exists(path))
            {
                Texture2D tex = new Texture2D(1, 1, TextureFormat.RGB24, false);
                byte[] data = SdFile.ReadAllBytes(path);
                tex.LoadImage(data);
                prefabPreview.Texture = tex;
                noPreviewLabel.IsVisible = false;
                prefabPreview.IsVisible = true;
                return;
            }
        }
        noPreviewLabel.IsVisible = true;
        prefabPreview.IsVisible = false;
    }

    private void OnEntryDoubleClicked(XUiC_PrefabFileList.PrefabFileEntry entry)
    {
        if (!btnPreview.Enabled)
            return;
        OnPreviewPressed(this, -1);
    }

    private void OnPageNumberChanged(int pageNumber) => fileList.SelectedEntryIndex = fileList.Page * fileList.PageLength;

    private void OnPreviewPressed(XUiController sender, int mouseButton)
    {
        XUiC_PrefabFileList.PrefabFileEntry selectedEntry = fileList.SelectedEntry.GetEntry();
        PathAbstractions.AbstractedLocation location = selectedEntry.location;
        LoadPrefab(location);
    }

    private int SliderHorizontalOffset_Value() => prefabInstance == null ? 0 : Mathf.RoundToInt(sliderHorizontalOffset.Value);

    private string SliderHorizontalOffset_ValueFormatter(float value) => SliderHorizontalOffset_Value().ToString();

    private void OnSliderHorizontalOffsetChanged(XUiC_BetterSlider _sender)
    {
        if (prefabInstance == null)
            return;

        int xDiff = SliderHorizontalOffset_Value() - prefabOffset.x;
        if (xDiff == 0)
            return;

        prefabOffset.x = SliderHorizontalOffset_Value();
        Vector3i move = Vector3i.zero;
        switch (Rotation)
        {
            case 0:
                move.x = xDiff;
                break;
            case 1:
                move.z = -xDiff;
                break;
            case 2:
                move.x = -xDiff;
                break;
            case 3:
                move.z = xDiff;
                break;
        }
        prefabInstance.MoveBoundingBox(move);
    }

    private int SliderVerticalOffset_Value() => prefabInstance == null ? 0 : Mathf.RoundToInt(sliderVerticalOffset.Value);

    private string SliderVerticalOffset_ValueFormatter(float value) => SliderVerticalOffset_Value().ToString();

    private void OnSliderVerticalOffsetChanged(XUiC_BetterSlider sender)
    {
        if (prefabInstance == null)
            return;

        int yDiff = SliderVerticalOffset_Value() - prefabOffset.y;

        if (yDiff == 0)
            return;

        prefabOffset.y = SliderVerticalOffset_Value();
        Vector3i move = Vector3i.zero;
        move.y = yDiff;
        prefabInstance.MoveBoundingBox(move);
    }

    private byte SliderFacing_Value() => prefabInstance == null ? (byte)0 : (byte)Mathf.RoundToInt(sliderFacing.Value);

    private string SliderFacing_ValueFormatter(float value) => ((BlockFace)SliderFacing_Value() + 2).ToString();

    private void OnSliderFacingChanged(XUiC_BetterSlider _sender)
    {
        if (prefabInstance == null)
            return;

    }

    private void SetSliders()
    {
        if (prefabInstance == null)
        {
            return;
        }
        int rotation = (prefabRotation + Rotation) % 4;
        int prefabHorizontalValue = rotation == 0 || rotation == 2 ? prefabOffset.x : prefabOffset.z;
        int horizontalValue = rotation == 0 || rotation == 2 ? prefabInstance.boundingBoxSize.x : prefabInstance.boundingBoxSize.z;
        int halfSize = Mathf.FloorToInt(horizontalValue / 2.0f);
        sliderHorizontalOffset.SetAndShow(prefabHorizontalValue, -halfSize, halfSize, 1);

        float verticalMax = prefabInstance.prefab.yOffset * -1;
        float verticalMin = (prefabInstance.boundingBoxSize.y - verticalMax - 1) * -1;
        sliderVerticalOffset.SetAndShow(prefabOffset.y, verticalMin, verticalMax, 1);
    }

    public void SetTileEntity(TileEntityAutoBaseBuilder tileEntity)
    {
        this.tileEntity = tileEntity;

        if (!tileEntity.prefabLocation.Equals(PathAbstractions.AbstractedLocation.None) && fileList.SelectByLocation(tileEntity.prefabLocation))
        {
            prefabOffset = tileEntity.prefabOffset;
            prefabRotation = tileEntity.prefabRotation;
            OnEntrySelectionChanged(null, fileList.SelectedEntry);
            OnPreviewPressed(this, -1);
        }
    }

    private void LoadPrefab(PathAbstractions.AbstractedLocation location)
    {
        Prefab selectedPrefab = new Prefab();
        selectedPrefab.Load(location);

        Vector3i prefabPos = GetPrefabPos(selectedPrefab);
        DynamicPrefabDecorator dpd = GameManager.Instance.GetDynamicPrefabDecorator();
        prefabInstance = new ABBPrefabInstance(dpd.GetNextId(), selectedPrefab.location, prefabPos, 0, selectedPrefab, 0);
        prefabInstance.CreateBoundingBox();
        dpd.AddPrefab(prefabInstance);

        SelectionBoxManager.Instance.SetActive("DynamicPrefabs", prefabInstance.name, true);

        int rotCount = (prefabRotation + Rotation) % 4;
        while (rotCount-- > 0)
            prefabInstance.RotateAroundY();
        prefabInstance.UpdateImposterView();
        btnPreview.Enabled = false;
        RefreshBindings(true);
        SetSliders();
    }

    private void ClearPrefab()
    {
        prefabInstance?.CleanFromWorld(GameManager.Instance.World, true);
        DynamicPrefabDecorator dpd = GameManager.Instance.GetDynamicPrefabDecorator();
        dpd.RemoveActivePrefab(GameManager.Instance.World);
        prefabOffset = Vector3i.zero;
        prefabInstance = null;
        sliderHorizontalOffset.Hide();
        sliderVerticalOffset.Hide();
    }

    private Vector3i GetPrefabPos(Prefab prefab)
    {
        Vector3i prefabPos = tileEntity.ToWorldPos();
        prefabPos.y += prefab.yOffset;

        int v = prefab.rotationToFaceNorth % 2;
        switch (Rotation)
        {
            case 0:
                prefabPos.z += 1;
                prefabPos.x -= (v == 0 ? prefab.size.x : prefab.size.z) / 2;
                break;
            case 1:
                prefabPos.z -= (v == 0 ? prefab.size.x : prefab.size.z) / 2;
                prefabPos.x += 1;
                break;
            case 2:
                prefabPos.z -= v != 0 ? prefab.size.x : prefab.size.z;
                prefabPos.x -= (v == 0 ? prefab.size.x : prefab.size.z) / 2;
                break;
            case 3:
                prefabPos.z -= (v == 0 ? prefab.size.x : prefab.size.z) / 2;
                prefabPos.x -= v != 0 ? prefab.size.x : prefab.size.z;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return prefabPos + prefabOffset;
    }
}
