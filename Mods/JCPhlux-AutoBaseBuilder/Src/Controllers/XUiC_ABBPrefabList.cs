using UnityEngine;

public class XUiC_ABBPrefabList : XUiController
{
    public static string ID = "";
    private XUiC_SimpleButton btnPreview;
    private XUiC_ABBPrefabFileList fileList;
    private bool hasPrefab;
    private XUiV_Label noPreviewLabel;
    private ABBPrefabInstance prefabInstance;
    private Vector3i prefabOffset = Vector3i.zero;
    private XUiV_Texture prefabPreview;
    private byte prefabRotation = 0;
    private XUiC_PhluxSlider sliderFacing;
    private XUiC_PhluxSlider sliderHorizontalOffset;
    private XUiC_PhluxSlider sliderVerticalOffset;
    private TileEntityAutoBaseBuilder tileEntity;
    private int RotationToFaceNorth => prefabInstance == null ? 0 : prefabInstance.prefab.rotationToFaceNorth;

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

        sliderHorizontalOffset = GetChildById("sliderHorizontalOffset") as XUiC_PhluxSlider;
        sliderHorizontalOffset.Label = Localization.Get("xuiSliderHorizontalOffset");
        sliderHorizontalOffset.Hide();
        sliderHorizontalOffset.ValueFormatter = SliderHorizontalOffset_ValueFormatter;
        sliderHorizontalOffset.OnValueChanged += OnSliderHorizontalOffsetChanged;

        sliderVerticalOffset = GetChildById("sliderVerticalOffset") as XUiC_PhluxSlider;
        sliderVerticalOffset.Label = Localization.Get("xuiSliderVerticalOffset");
        sliderVerticalOffset.Hide();
        sliderVerticalOffset.ValueFormatter = SliderVerticalOffset_ValueFormatter;
        sliderVerticalOffset.OnValueChanged += OnSliderVerticalOffsetChanged;

        sliderFacing = GetChildById("sliderFacing") as XUiC_PhluxSlider;
        sliderFacing.Label = Localization.Get("xuiSliderFacing");
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

    public void SetTileEntity(TileEntityAutoBaseBuilder tileEntity)
    {
        this.tileEntity = tileEntity;
        hasPrefab = !tileEntity.prefabLocation.Equals(PathAbstractions.AbstractedLocation.None);

        if (hasPrefab && fileList.SelectByLocation(tileEntity.prefabLocation))
        {
            prefabOffset = tileEntity.prefabOffset;
            prefabRotation = tileEntity.prefabRotation.Value;
            OnEntrySelectionChanged(null, fileList.SelectedEntry);
            OnPreviewPressed(this, -1);
        }
        else
            prefabRotation = UtilsHelpers.MirrorSimpleRotation(tileEntity.Rotation);
    }

    private void ClearPrefab()
    {
        prefabInstance?.CleanFromWorld(GameManager.Instance.World, true);
        DynamicPrefabDecorator dpd = GameManager.Instance.GetDynamicPrefabDecorator();
        dpd.RemoveActivePrefab(GameManager.Instance.World);
        prefabOffset = Vector3i.zero;
        prefabInstance = null;
        prefabRotation = UtilsHelpers.MirrorSimpleRotation(tileEntity.Rotation);
        sliderHorizontalOffset.Hide();
        sliderVerticalOffset.Hide();
        sliderFacing.Hide();
    }

    private void OnEntryDoubleClicked(XUiC_PrefabFileList.PrefabFileEntry entry)
    {
        if (!btnPreview.Enabled)
            return;
        OnPreviewPressed(this, -1);
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
            Object.Destroy(texture);
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

    private void OnPageNumberChanged(int pageNumber) => fileList.SelectedEntryIndex = fileList.Page * fileList.PageLength;

    private void OnPreviewPressed(XUiController sender, int mouseButton)
    {
        XUiC_PrefabFileList.PrefabFileEntry selectedEntry = fileList.SelectedEntry.GetEntry();
        PathAbstractions.AbstractedLocation location = selectedEntry.location;

        Prefab prefab = new();
        prefab.Load(location);

        if (hasPrefab && tileEntity.prefabLocation.Equals(location))
        {
            prefabOffset = tileEntity.prefabOffset;
            prefabRotation = tileEntity.prefabRotation.Value;
        }
        else
        {
            int offsetX = -prefab.size.x / 2;
            int offsetZ = -prefab.size.z;

            if (tileEntity.Rotation <= 1)
                offsetZ = 1;

            if (tileEntity.Rotation % 2 != 0)
                MathUtils.Swap(ref offsetX, ref offsetZ);

            prefabOffset = new(offsetX, prefab.yOffset, offsetZ);
            prefabRotation = UtilsHelpers.NormalizeSimpleRotation(prefabRotation + prefab.rotationToFaceNorth);
        }
        Vector3i prefabPos = tileEntity.ToWorldPos() + prefabOffset;

        DynamicPrefabDecorator dpd = GameManager.Instance.GetDynamicPrefabDecorator();
        prefabInstance = new ABBPrefabInstance(dpd.GetNextId(), prefab.location, prefabPos, 0, prefab, 0);
        prefabInstance.CreateBoundingBox();
        dpd.AddPrefab(prefabInstance);
        SelectionBox box = prefabInstance.GetBox();
        box.EnableCollider("Untagged", 14);
        box.SetCaptionVisibility(false);
        SelectionBoxManager.Instance.SetActive("DynamicPrefabs", prefabInstance.name, true);

        UpdateRotation();

        UpdateSelectionBox();

        btnPreview.Enabled = false;
        RefreshBindings(true);
        SetHorizontalSlider();
        SetVerticalSlider();
        SetRotationSlider();
    }

    private void OnSliderFacingChanged(XUiC_PhluxSlider _sender)
    {
        if (prefabInstance == null)
            return;

        byte facing = SliderFacing_Value();
        if (facing == prefabInstance.rotation)
            return;

        prefabRotation = facing;

        UpdateRotation();

        switch (tileEntity.Rotation)
        {
            case 0:
                prefabOffset.x = -prefabInstance.boundingBoxSize.x / 2;
                prefabOffset.z = 1;
                break;

            case 1:
                prefabOffset.x = 1;
                prefabOffset.z = -prefabInstance.boundingBoxSize.z / 2;
                break;

            case 2:
                prefabOffset.x = -prefabInstance.boundingBoxSize.x / 2;
                prefabOffset.z = -prefabInstance.boundingBoxSize.z;
                break;

            default:
                prefabOffset.x = -prefabInstance.boundingBoxSize.x;
                prefabOffset.z = -prefabInstance.boundingBoxSize.z / 2;
                break;
        }

        Vector3i prefabPos = tileEntity.ToWorldPos() + prefabOffset;
        prefabInstance.SetBoundingBoxPosition(prefabPos);
        UpdateSelectionBox();
        SetHorizontalSlider();
    }

    private void OnSliderHorizontalOffsetChanged(XUiC_PhluxSlider _sender)
    {
        if (prefabInstance == null)
            return;

        bool isNorthSouth = tileEntity.Rotation % 2 == 0;
        int boundingBoxVal = isNorthSouth ? prefabInstance.boundingBoxSize.x : prefabInstance.boundingBoxSize.z;
        int halfSize = Mathf.FloorToInt(boundingBoxVal / 2.0f);
        int offsetVal = isNorthSouth ? prefabOffset.x : prefabOffset.z;
        int newVal = SliderHorizontalOffset_Value() - halfSize;
        if (newVal == offsetVal)
            return;

        int xDiff = newVal - offsetVal;
        Vector3i deltaVec = Vector3i.zero;
        if (isNorthSouth)
        {
            prefabOffset.x = newVal;
            deltaVec.x = xDiff;
        }
        else
        {
            prefabOffset.z = newVal;
            deltaVec.z = xDiff;
        }

        prefabInstance.MoveBoundingBox(deltaVec);
        UpdateSelectionBox();
    }

    private void OnSliderVerticalOffsetChanged(XUiC_PhluxSlider sender)
    {
        if (prefabInstance == null)
            return;

        int offsetY = SliderVerticalOffset_Value() + prefabInstance.prefab.yOffset;

        if (offsetY == prefabOffset.y)
            return;

        int yDiff = offsetY - prefabOffset.y;

        prefabOffset.y = offsetY;

        Vector3i deltaVec = new(0, yDiff, 0);
        prefabInstance.MoveBoundingBox(deltaVec);
        UpdateSelectionBox();
    }

    private void SetHorizontalSlider()
    {
        if (prefabInstance == null)
            return;

        bool isNorthSouth = tileEntity.Rotation % 2 == 0;
        int boundingBoxVal = isNorthSouth ? prefabInstance.boundingBoxSize.x : prefabInstance.boundingBoxSize.z;
        int halfSize = Mathf.FloorToInt(boundingBoxVal / 2.0f);
        int offsetVal = isNorthSouth ? prefabOffset.x : prefabOffset.z;
        int defaultVal = offsetVal + halfSize;
        sliderHorizontalOffset.SetAndShow(defaultVal, -halfSize, halfSize, 1);
    }

    private void SetRotationSlider() => sliderFacing.SetAndShow(prefabRotation, 0, 3, 1);

    private void SetVerticalSlider()
    {
        if (prefabInstance == null)
            return;

        int minVal = 1 - (prefabInstance.boundingBoxSize.y + prefabInstance.prefab.yOffset);
        int maxVal = -prefabInstance.prefab.yOffset;
        int defaultVal = prefabOffset.y - prefabInstance.prefab.yOffset;
        sliderVerticalOffset.SetAndShow(defaultVal, minVal, maxVal, 1);
    }

    private byte SliderFacing_Value() => prefabInstance == null ? (byte)0 : (byte)Mathf.RoundToInt(sliderFacing.Value);

    private string SliderFacing_ValueFormatter(float value) => UtilsHelpers.BlockFaceFromSimpleRotation(SliderFacing_Value() - RotationToFaceNorth).ToString();

    private int SliderHorizontalOffset_Value() => prefabInstance == null ? 0 : Mathf.RoundToInt(tileEntity.Rotation % 3 == 0 ? sliderHorizontalOffset.Value : -sliderHorizontalOffset.Value);

    private string SliderHorizontalOffset_ValueFormatter(float value) => SliderHorizontalOffset_Value().ToString();

    private int SliderVerticalOffset_Value() => prefabInstance == null ? 0 : Mathf.RoundToInt(sliderVerticalOffset.Value);

    private string SliderVerticalOffset_ValueFormatter(float value) => SliderVerticalOffset_Value().ToString();

    private void UpdateRotation()
    {
        if (prefabInstance == null)
            return;

        while (prefabRotation != prefabInstance.rotation)
            prefabInstance.RotateAroundY();
        prefabInstance.UpdateImposterView();
    }

    private bool UpdateSelectionBox()
    {
        if (prefabInstance == null)
            return false;

        SelectionBox box = prefabInstance.GetBox();
        Vector3i size = prefabInstance.boundingBoxSize;
        Vector3i corner1 = prefabInstance.boundingBoxPosition;
        Vector3i corner2 = corner1 + new Vector3i(size.x, 0, size.z);
        Vector3i corner3 = corner1 + new Vector3i(0, 0, size.z);
        Vector3i corner4 = corner1 + new Vector3i(size.x, 0, 0);
        if (
            !UtilsHelpers.CanBuildAtPosition(corner1) ||
            !UtilsHelpers.CanBuildAtPosition(corner2) ||
            !UtilsHelpers.CanBuildAtPosition(corner3) ||
            !UtilsHelpers.CanBuildAtPosition(corner4)
            )
        {
            box.SetAllFacesColor(Color.red);
            return false;
        }

        box.SetAllFacesColor(Color.green);
        return true;
    }
}