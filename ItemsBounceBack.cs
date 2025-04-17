using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using REPOLib.Extensions;
using Steamworks.Ugc;

namespace ItemsBounceBack
{
    [BepInPlugin("SeroRonin.ItemsBounceBack", "ItemsBounceBack", "1.1.0")]
    [BepInDependency(REPOLib.MyPluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
    public class ItemsBounceBack : BaseUnityPlugin
    {
        public static ItemsBounceBack Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger => Instance._logger;
        private ManualLogSource _logger => base.Logger;
        internal Harmony? Harmony { get; set; }

        public Dictionary<string, bool> shouldItemBounce = new Dictionary<string, bool>();

        private void Awake()
        {
            Instance = this;

            // Prevent the plugin from being deleted
            this.gameObject.transform.parent = null;
            this.gameObject.hideFlags = HideFlags.HideAndDontSave;

            Patch();

            Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");   
        }

        internal void Patch()
        {
            Harmony ??= new Harmony(Info.Metadata.GUID);
            Harmony.PatchAll();
        }

        internal void Unpatch()
        {
            Harmony?.UnpatchSelf();
        }

        public static bool ShouldItemBounce( GameObject gameObject )
        {
            var itemComp = gameObject.GetComponent<ItemAttributes>();
            var itemEquip = gameObject.GetComponent<ItemEquippable>();
            var shouldBounce = false;
            if ( itemComp && itemEquip && !SemiFunc.RunIsArena())
            {
                Instance.shouldItemBounce.TryGetValue(itemComp.itemAssetName, out shouldBounce);
            }

            return shouldBounce;
        }

        public static bool TryAddBounceEntry( string assetName, bool shouldBounce = true, bool overrideEntry = true )
        {
            if ( !Instance.shouldItemBounce.ContainsKey(assetName) )
            {
                Instance.shouldItemBounce.Add(assetName, shouldBounce);
                return true;
            }
            else
            {
                if (overrideEntry)
                {
                    Instance.shouldItemBounce[assetName] = shouldBounce;
                }
            }

            return false;
        }

        public static void PopulateDictionary()
        {
            var itemList = StatsManager.instance.GetItems();
            foreach ( var item in itemList )
            {
                // These item types should not bounce by default, individual entries can be overriden by developers in their own code
                var shouldBounce = ( !(item.itemType == SemiFunc.itemType.item_upgrade) && !(item.itemType == SemiFunc.itemType.cart) && !(item.itemType == SemiFunc.itemType.pocket_cart) );

                TryAddBounceEntry(item.itemAssetName, shouldBounce, false);
            }
        }
    }
}