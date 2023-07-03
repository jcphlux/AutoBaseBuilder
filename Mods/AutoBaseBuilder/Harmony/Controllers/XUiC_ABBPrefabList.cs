using System;
using UnityEngine;


public class XUiC_ABBPrefabList : XUiController
{
    public static string ID = "";
    private XUiC_ABBPrefabFileList fileList;
    private XUiV_Texture prefabPreview;
    private XUiV_Label noPreviewLabel;
    private XUiC_SimpleButton btnPreview;
    private XUiC_Slider sliderHorizontalOffset;
    private XUiC_Slider sliderVerticalOffset;
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
        fileList.SelectionChanged += new XUiEvent_ListSelectionChangedEventHandler<XUiC_ABBPrefabFileList.PrefabFileEntry>(OnEntrySelectionChanged);
        fileList.OnEntryDoubleClicked += new XUiC_ABBPrefabFileList.EntryDoubleClickedDelegate(OnEntryDoubleClicked);
        fileList.PageNumberChanged += new XUiEvent_ListPageNumberChangedEventHandler(OnPageNumberChanged);

        btnPreview = GetChildById("btnPreview") as XUiC_SimpleButton;
        btnPreview.Enabled = false;
        btnPreview.OnPressed += new XUiEvent_OnPressEventHandler(OnPreviewPressed);

        sliderHorizontalOffset = GetChildById("sliderHorizontalOffset") as XUiC_Slider;
        sliderHorizontalOffset.Label = "Horizontal Offset"; // Localization.Get("xuiSliderOffset");
        sliderHorizontalOffset.Value = 0.5f;
        sliderHorizontalOffset.ValueFormatter = new Func<float, string>(SliderHorizontalOffset_ValueFormatter);
        sliderHorizontalOffset.OnValueChanged += new XUiEvent_SliderValueChanged(OnSliderHorizontalOffsetChanged);

        sliderVerticalOffset = GetChildById("sliderVerticalOffset") as XUiC_Slider;
        sliderVerticalOffset.Label = "Vertical Offset"; // Localization.Get("xuiSliderOffset");
        sliderVerticalOffset.Value = 0.5f;
        sliderVerticalOffset.ValueFormatter = new Func<float, string>(SliderVerticalOffset_ValueFormatter);
        sliderVerticalOffset.OnValueChanged += new XUiEvent_SliderValueChanged(OnSliderVerticalOffsetChanged);
    }

    public override void OnClose()
    {
        base.OnClose();
        ClearPrefab();
        btnPreview.Enabled = fileList.SelectedEntry != null;
    }

    private void OnEntrySelectionChanged(
      XUiC_ListEntry<XUiC_ABBPrefabFileList.PrefabFileEntry> _previousEntry,
      XUiC_ListEntry<XUiC_ABBPrefabFileList.PrefabFileEntry> _newEntry)
    {
        btnPreview.Enabled = _newEntry != null;
        ClearPrefab();
        if (prefabPreview.Texture != null)
        {
            Texture2D texture = (Texture2D)prefabPreview.Texture;
            prefabPreview.Texture = null;
            UnityEngine.Object.Destroy(texture);
        }
        if (_newEntry?.GetEntry() != null)
        {
            string path = _newEntry.GetEntry().location.FullPathNoExtension + ".jpg";
            if (SdFile.Exists(path))
            {
                Texture2D tex = new(1, 1, TextureFormat.RGB24, false);
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

    private void OnEntryDoubleClicked(XUiC_ABBPrefabFileList.PrefabFileEntry _entry)
    {
        if (!btnPreview.Enabled)
            return;
        OnPreviewPressed(this, -1);
    }

    private void OnPageNumberChanged(int _pageNumber) => fileList.SelectedEntryIndex = fileList.Page * fileList.PageLength;

    private void OnPreviewPressed(XUiController _sender, int _mouseButton)
    {
        PathAbstractions.AbstractedLocation location = fileList.SelectedEntry.GetEntry().location;
        LoadPrefab(location);
        //prefabInstance.SetBoundingBoxPosition(prefabPos + Vector3i.right);
    }

    private int SliderHorizontalOffset_Value()
    {
        if (prefabInstance == null)
        {
            return 0;
        }

        float clampedValue = Mathf.Clamp(sliderHorizontalOffset.Value, 0.0f, 1.0f);
        int halfSize = Mathf.FloorToInt(prefabInstance.boundingBoxSize.x / 2.0f);
        int offset = Mathf.RoundToInt((clampedValue - 0.5f) * (2 * halfSize));

        return Mathf.Clamp(offset, -halfSize, halfSize);
    }

    private void SliderHorizontalOffset_Reset()
    {
        if (prefabInstance == null)
        {
            return;
        }

        prefabOffset.x = 0;
        sliderHorizontalOffset.Value = 0.5f;
    }

    private string SliderHorizontalOffset_ValueFormatter(float _value) => SliderHorizontalOffset_Value().ToString();

    private void OnSliderHorizontalOffsetChanged(XUiC_Slider _sender)
    {
        if (prefabInstance == null)
        {
            sliderHorizontalOffset.Value = 0.5f;
            return;
        }

        int xDiff = prefabOffset.x - SliderHorizontalOffset_Value();
        Vector3i move = Vector3i.zero;
        if (xDiff != 0)
        {
            move.x = xDiff;
            prefabOffset.x = SliderHorizontalOffset_Value();
            prefabInstance.MoveBoundingBox(move);
        }
    }
    private int SliderVerticalOffset_Value()
    {
        if (prefabInstance == null)
        {
            return 0;
        }

        float clampedValue = Mathf.Clamp(sliderVerticalOffset.Value, 0.0f, 1.0f);
        float yOffset = prefabInstance.prefab.yOffset;
        float offset = yOffset + (clampedValue * (prefabInstance.boundingBoxSize.y - 1));

        return Mathf.RoundToInt(offset);
    }

    private void SliderVerticalOffset_Reset()
    {
        if (prefabInstance == null)
        {
            return;
        }

        float yOffset = prefabInstance.prefab.yOffset;
        float offset = -yOffset;
        float clampedValue = Mathf.InverseLerp(0, prefabInstance.boundingBoxSize.y - 1, offset);
        sliderVerticalOffset.Value = Mathf.Clamp01(clampedValue);
    }

    private string SliderVerticalOffset_ValueFormatter(float _value) => SliderVerticalOffset_Value().ToString();

    private void OnSliderVerticalOffsetChanged(XUiC_Slider _sender)
    {
        if (prefabInstance == null)
        {
            sliderVerticalOffset.Value = 0.5f;
            return;
        }
        int yDiff = prefabOffset.y - SliderVerticalOffset_Value();
        Vector3i move = Vector3i.zero;
        if (yDiff != 0)
        {
            move.y = yDiff;
            prefabOffset.y = SliderVerticalOffset_Value();
            prefabInstance.MoveBoundingBox(move);
        }
    }

    public void SetTileEntity(TileEntityAutoBaseBuilder _tileEntity)
    {
        tileEntity = _tileEntity;

        if (!tileEntity.prefabLocation.Equals(PathAbstractions.AbstractedLocation.None) & fileList.SelectByLocation(tileEntity.prefabLocation))
        {
            prefabOffset = _tileEntity.prefabOffset;
            prefabRotation = _tileEntity.prefabRotation;
            OnEntrySelectionChanged(null, fileList.SelectedEntry);
            OnPreviewPressed(this, -1);
        }
    }

    private void LoadPrefab(PathAbstractions.AbstractedLocation location)
    {
        Prefab selectedPrefab = new();
        selectedPrefab.Load(location);

        Vector3i prefabPos = GetPrefabPos(selectedPrefab);
        DynamicPrefabDecorator dpd = GameManager.Instance.GetDynamicPrefabDecorator();
        prefabInstance = new(dpd.GetNextId(), selectedPrefab.location, prefabPos, 0, selectedPrefab, 0);
        prefabInstance.CreateBoundingBox();
        dpd.AddPrefab(prefabInstance);

        SelectionBoxManager.Instance.SetActive("DynamicPrefabs", prefabInstance.name, _bActive: true);

        int rotCount = Rotation;
        while (rotCount-- > 0)
            prefabInstance.RotateAroundY();
        prefabInstance.UpdateImposterView();
        btnPreview.Enabled = false;
        SliderHorizontalOffset_Reset();
        SliderVerticalOffset_Reset();
    }

    private void ClearPrefab()
    {
        prefabInstance?.CleanFromWorld(GameManager.Instance.World, true);
        DynamicPrefabDecorator dpd = GameManager.Instance.GetDynamicPrefabDecorator();
        dpd.RemoveActivePrefab(GameManager.Instance.World);
        prefabOffset = Vector3i.zero;
        prefabInstance = null;
    }

    private Vector3i GetPrefabPos(Prefab _prefab)
    {
        Vector3i prefabPos = tileEntity.ToWorldPos();
        prefabPos.y += _prefab.yOffset;

        int v = _prefab.rotationToFaceNorth % 2;
        switch (Rotation)
        {
            case 0:
                prefabPos.z += 1;
                prefabPos.x -= (v == 0 ? _prefab.size.x : _prefab.size.z) / 2;
                break;
            case 1:
                prefabPos.z -= (v == 0 ? _prefab.size.x : _prefab.size.z) / 2;
                prefabPos.x += 1;
                break;
            case 2:
                prefabPos.z -= v != 0 ? _prefab.size.x : _prefab.size.z;
                prefabPos.x -= (v == 0 ? _prefab.size.x : _prefab.size.z) / 2;
                break;
            case 3:
                prefabPos.z -= (v == 0 ? _prefab.size.x : _prefab.size.z) / 2;
                prefabPos.x -= v != 0 ? _prefab.size.x : _prefab.size.z;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return prefabPos + prefabOffset;
    }
}

