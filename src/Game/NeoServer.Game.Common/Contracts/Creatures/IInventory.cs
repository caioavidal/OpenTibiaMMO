﻿using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Creatures
{
    public delegate void RemoveItemFromSlot(IInventory inventory, IPickupable item, Slot slot, byte amount = 1);

    public delegate void AddItemToSlot(IInventory inventory, IPickupable item, Slot slot, byte amount = 1);

    public delegate void FailAddItemToSlot(InvalidOperation invalidOperation);

    public interface IInventory : IStore
    {
        IPlayer Owner { get; }

        ushort TotalAttack { get; }

        ushort TotalDefense { get; }

        byte TotalArmor { get; }

        byte AttackRange { get; }
        IContainer BackpackSlot { get; }
        IWeapon Weapon { get; }
        bool HasShield { get; }
        float TotalWeight { get; }
        IDictionary<ushort, uint> Map { get; }
        ulong TotalMoney { get; }

        IItem this[Slot slot] { get; }

        event AddItemToSlot OnItemAddedToSlot;
        event FailAddItemToSlot OnFailedToAddToSlot;
        event RemoveItemFromSlot OnItemRemovedFromSlot;

        Result<IPickupable> TryAddItemToSlot(Slot slot, IPickupable item);
        bool RemoveItemFromSlot(Slot slot, byte amount, out IPickupable removedItem);
        Result<bool> CanAddItemToSlot(Slot slot, IItem item);
        ulong GetTotalMoney(IDictionary<ushort, uint> inventoryMap);
    }
}