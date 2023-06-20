using System;
using UnityEngine;

public class BlockAutoBaseBuilder : BlockSecureLoot
{

	private Vector2i LootSize = new Vector2i(8, 4);

	private float BoundHelperSize = 2.59f;

	private float RepairSpeed = 2000f;

	private float TakeDelay = 30f;

	// Copied from vanilla BlockLandClaim code
	// public override void OnBlockEntityTransformBeforeActivated(
	// 	WorldBase _world,
	// 	Vector3i _blockPos,
	// 	int _cIdx,
	// 	BlockValue _blockValue,
	// 	BlockEntityData _ebcd)
	// {
	// 	base.OnBlockEntityTransformBeforeActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
	// }

	public override void Init()
	{
		base.Init();
		TakeDelay = !Properties.Values.ContainsKey("TakeDelay") ? TakeDelay
			: StringParsers.ParseFloat(Properties.Values["TakeDelay"]);
		RepairSpeed = !Properties.Values.ContainsKey("AutoBaseBuilderSpeedFactor") ? RepairSpeed
			: StringParsers.ParseFloat(Properties.Values["AutoBaseBuilderSpeedFactor"]);
	}

	// Copied from vanilla BlockLandClaim code
	public override void OnBlockLoaded(
		WorldBase _world,
		int _clrIdx,
		Vector3i _blockPos,
		BlockValue _blockValue)
	{
		base.OnBlockLoaded(_world, _clrIdx, _blockPos, _blockValue);
		if (GameManager.IsDedicatedServer) return;
		if (_world.GetTileEntity(_clrIdx, _blockPos) is TileEntityAutoBaseBuilder tileEntityLandAutoRepair)
		{
			Transform boundsHelper = LandClaimBoundsHelper.GetBoundsHelper(_blockPos.ToVector3());
			if (boundsHelper != null)
			{
				boundsHelper.localScale = new Vector3(BoundHelperSize, BoundHelperSize, BoundHelperSize);
				boundsHelper.localPosition = new Vector3(_blockPos.x + 0.5f, _blockPos.y + 0.5f, _blockPos.z + 0.5f);
				tileEntityLandAutoRepair.BoundsHelper = boundsHelper;
				tileEntityLandAutoRepair.ResetBoundHelper(Color.gray);
			}
		}
	}

	// Copied from vanilla BlockLandClaim code
	// public override void OnBlockValueChanged(
	// 	WorldBase _world,
	// 	Chunk _chunk,
	// 	int _clrIdx,
	// 	Vector3i _blockPos,
	// 	BlockValue _oldBlockValue,
	// 	BlockValue _newBlockValue)
	// {
	// 	base.OnBlockValueChanged(_world, _chunk, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
	// }

	// Copied from vanilla BlockLandClaim code
	public override void OnBlockEntityTransformAfterActivated(
		WorldBase _world,
		Vector3i _blockPos,
		int _cIdx,
		BlockValue _blockValue,
		BlockEntityData _ebcd)
	{
		if (_ebcd == null) return;
		if (_world.GetTileEntity(_cIdx, _blockPos) is TileEntityAutoBaseBuilder te) {
			te.repairSpeed = RepairSpeed;
		}
		else {
			Chunk chunkFromWorldPos = (Chunk) _world.GetChunkFromWorldPos(_blockPos);
			te = new TileEntityAutoBaseBuilder(chunkFromWorldPos)
			{
				localChunkPos = World.toBlock(_blockPos),
				lootListName = lootList
			};
			te.repairSpeed = RepairSpeed;
			te.SetContainerSize(LootSize, false);
			chunkFromWorldPos.AddTileEntity(te);
		}

		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
	}

	// Copied from vanilla BlockLandClaim code
	public override void OnBlockAdded(
		WorldBase _world,
		Chunk _chunk,
		Vector3i _blockPos,
		BlockValue _blockValue)
	{
		if (_blockValue.ischild || _world.GetTileEntity(_chunk.ClrIdx, _blockPos) is TileEntityAutoBaseBuilder)
			return;

		// Overload TileEntity creation (base method should still recognize this)
		TileEntityAutoBaseBuilder tileEntity = new TileEntityAutoBaseBuilder(_chunk)
		{
			localChunkPos = World.toBlock(_blockPos),
			lootListName = lootList
		};
		tileEntity.repairSpeed = RepairSpeed;
		tileEntity.SetContainerSize(LootSize, false);
		_chunk.AddTileEntity(tileEntity);

		base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue);
		if (GameManager.IsDedicatedServer) return;
		if (_world.GetTileEntity(_chunk.ClrIdx, _blockPos) is TileEntityAutoBaseBuilder tileEntityLandAutoRepair)
		{
			Transform boundsHelper = LandClaimBoundsHelper.GetBoundsHelper(_blockPos.ToVector3());
			if (boundsHelper != null)
			{
				boundsHelper.localScale = new Vector3(BoundHelperSize, BoundHelperSize, BoundHelperSize);
				boundsHelper.localPosition = new Vector3(_blockPos.x + 0.5f, _blockPos.y + 0.5f, _blockPos.z + 0.5f);
				tileEntityLandAutoRepair.BoundsHelper = boundsHelper;
				tileEntityLandAutoRepair.ResetBoundHelper(Color.gray);
			}
		}
	}

	// Copied from vanilla BlockLandClaim code
	public override void OnBlockRemoved(
		WorldBase _world,
		Chunk _chunk,
		Vector3i _blockPos,
		BlockValue _blockValue)
	{
		base.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
		if (_world.GetTileEntity(_chunk.ClrIdx, _blockPos) is TileEntityAutoBaseBuilder tileEntityLandAutoRepair)
		{
			LandClaimBoundsHelper.RemoveBoundsHelper(_blockPos.ToVector3());
		}
	}

	// Copied from vanilla BlockLandClaim code
	public override void OnBlockUnloaded(
		WorldBase _world,
		int _clrIdx,
		Vector3i _blockPos,
		BlockValue _blockValue)
	{
		base.OnBlockUnloaded(_world, _clrIdx, _blockPos, _blockValue);
		if (_world.GetTileEntity(_clrIdx, _blockPos) is TileEntityAutoBaseBuilder tileEntityLandAutoRepair)
		{
			LandClaimBoundsHelper.RemoveBoundsHelper(_blockPos.ToVector3());
		}
	}

	// Copied from vanilla BlockLandClaim code
	// public override void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
	// {
	// 	base.PlaceBlock(_world, _result, _ea);
	// }

	public override BlockActivationCommand[] GetBlockActivationCommands(
		WorldBase _world,
		BlockValue _blockValue,
		int _clrIdx,
		Vector3i _blockPos,
		EntityAlive _entityFocusing)
	{
		TileEntityAutoBaseBuilder tileEntity = _world.GetTileEntity(_clrIdx, _blockPos) as TileEntityAutoBaseBuilder;
		BlockActivationCommand[] cmds = base.GetBlockActivationCommands(_world, _blockValue, _clrIdx, _blockPos, _entityFocusing);
		Array.Resize(ref cmds, cmds.Length + 3);

        cmds[cmds.Length - 2] = new BlockActivationCommand("take", "hand", false);
        string activate_cmd = tileEntity.IsOn ? "turn_autobuild_off" : "turn_autobuild_on";
		cmds[cmds.Length - 1] = new BlockActivationCommand(activate_cmd, "electric_switch", true);
		if (this.CanPickup)
			cmds[cmds.Length - 2].enabled = true;
		else if ((double) EffectManager.GetValue(PassiveEffects.BlockPickup, _entity: _entityFocusing, tags: _blockValue.Block.Tags) > 0.0)
			cmds[cmds.Length - 2].enabled = true;
		else
			cmds[cmds.Length - 2].enabled = false;

        string prefablist_cmd = "select_prefab";
        if (tileEntity.prefabLocation == null)
        {
            prefablist_cmd = Localization.Get("blockcommand_selected_prefab");
            if (string.IsNullOrEmpty(prefablist_cmd)) prefablist_cmd = "Selected Prefab {0}";
            prefablist_cmd = string.Format(prefablist_cmd, tileEntity.prefabLocation);

        }
        cmds[cmds.Length - 3] = new BlockActivationCommand(prefablist_cmd, "map_town", true);        

        return cmds;
	}

	public override bool OnBlockActivated(
		string _commandName,
		WorldBase _world,
		int _cIdx,
		Vector3i _blockPos,
		BlockValue _blockValue,
		EntityAlive _player)
	{
		if (!(_world.GetTileEntity(_cIdx, _blockPos) is TileEntityAutoBaseBuilder tileEntity)) return false;
		if (_commandName == "take")
		{
			// Copied from vanilla Block::OnBlockActivated
			bool flag = this.CanPickup;
			if ((double) EffectManager.GetValue(PassiveEffects.BlockPickup, _entity: _player, tags: _blockValue.Block.Tags) > 0.0)
			flag = true;
			if (!flag) return false;
			if (!_world.CanPickupBlockAt(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer()))
			{
				_player.PlayOneShot("keystone_impact_overlay");
				return false;
			}
			if (_blockValue.damage > 0)
			{
				GameManager.ShowTooltip(_player as EntityPlayerLocal, Localization.Get("ttRepairBeforePickup"), "ui_denied");
				return false;
			}
			ItemStack itemStack = Block.list[_blockValue.type].OnBlockPickedUp(_world, _cIdx, _blockPos, _blockValue, _player.entityId);
			if (!_player.inventory.CanTakeItem(itemStack) && !_player.bag.CanTakeItem(itemStack))
			{
				GameManager.ShowTooltip(_player as EntityPlayerLocal, Localization.Get("xuiInventoryFullForPickup"), "ui_denied");
				return false;
			}
			TakeItemWithTimer(_cIdx, _blockPos, _blockValue, _player);
			return false;

		}
		else if (_commandName == "turn_autobuild_off" || _commandName == "turn_autobuild_on")
		{
			tileEntity.IsOn = !tileEntity.IsOn;
			return true;
		}
		else if (_commandName == "select_prefab")
		{
            LocalPlayerUI playerUi = (_player as EntityPlayerLocal).PlayerUI;
            playerUi.windowManager.Open("jcphluxBlockAutoBaseBuilderPrefabList", true);
            return true;
        }
		else {
			return base.OnBlockActivated(_commandName, _world, _cIdx, _blockPos, _blockValue, _player);
		}
	}

	public override string GetActivationText(
		WorldBase _world,
		BlockValue _blockValue,
		int _clrIdx,
		Vector3i _blockPos,
		EntityAlive _entityFocusing)
	{
		return base.GetActivationText(_world, _blockValue, _clrIdx, _blockPos, _entityFocusing);
	}

	public void TakeItemWithTimer(
		int _cIdx,
		Vector3i _blockPos,
		BlockValue _blockValue,
		EntityAlive _player)
	{
		if (_blockValue.damage > 0)
		{
			GameManager.ShowTooltip(_player as EntityPlayerLocal, Localization.Get("ttRepairBeforePickup"), "ui_denied");
		}
		else
		{
			LocalPlayerUI playerUi = (_player as EntityPlayerLocal).PlayerUI;
			playerUi.windowManager.Open("timer", true);
			XUiC_Timer childByType = playerUi.xui.GetChildByType<XUiC_Timer>();
			TimerEventData _eventData = new TimerEventData();
			_eventData.Data = new object[4]
			{
				_cIdx,
				_blockValue,
				_blockPos,
				_player
			};
			_eventData.Event += new TimerEventHandler(EventData_Event);
			childByType.SetTimer(TakeDelay, _eventData);
		}
	}

	private void EventData_Event(TimerEventData timerData)
	{
		World world = GameManager.Instance.World;
		object[] data = (object[]) timerData.Data;
		int _clrIdx = (int) data[0];
		BlockValue blockValue = (BlockValue) data[1];
		Vector3i vector3i = (Vector3i) data[2];
		BlockValue block = world.GetBlock(vector3i);
		EntityPlayerLocal entityPlayerLocal = data[3] as EntityPlayerLocal;
		if (block.damage > 0)
		{
			GameManager.ShowTooltip(entityPlayerLocal, Localization.Get("ttRepairBeforePickup"), "ui_denied");
		}
		else if (block.type != blockValue.type)
		{
			GameManager.ShowTooltip(entityPlayerLocal, Localization.Get("ttBlockMissingPickup"), "ui_denied");
		}
		else
		{
			TileEntityAutoBaseBuilder tileEntity = world.GetTileEntity(_clrIdx, vector3i) as TileEntityAutoBaseBuilder;
			if (tileEntity.IsUserAccessing())
			{
				GameManager.ShowTooltip(entityPlayerLocal, Localization.Get("ttCantPickupInUse"), "ui_denied");
			}
			else
			{
				LocalPlayerUI uiForPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
				HandleTakeInternalItems(tileEntity, uiForPlayer);
				ItemStack itemStack = new ItemStack(block.ToItemValue(), 1);
				if (!uiForPlayer.xui.PlayerInventory.AddItem(itemStack))
				uiForPlayer.xui.PlayerInventory.DropItem(itemStack);
				world.SetBlockRPC(_clrIdx, vector3i, BlockValue.Air);
			}
		}
	}

	protected virtual void HandleTakeInternalItems(TileEntityAutoBaseBuilder te, LocalPlayerUI playerUI)
	{
		ItemStack[] items = te.items;
		for (int index = 0; index < items.Length; ++index)
		{
		if (!items[index].IsEmpty() && !playerUI.xui.PlayerInventory.AddItem(items[index]))
			playerUI.xui.PlayerInventory.DropItem(items[index]);
		}
	}

}
