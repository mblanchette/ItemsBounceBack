## Makes equippable items bounce out of hurt/kill boxes!
A very simple mod. No more losing items for the rest of the level when your teammate knocks you off the map (does not apply to carts and item upgrades)

If you have any issues, report them to me through either [the mod listing on the R.E.P.O Modding Discord](https://discord.com/channels/1344557689979670578/1356098096064495667) or on [github](https://github.com/SeroRonin/ItemsBounceBack)

### Mod Compatiblity (for Developers):
By default, this mod populates a dictionary of all items, making most hotbar-equippable items bounce. The following item types are disabled by default: cart, pocket_cart, and item_upgrade.

If you want to override a custom item's behaviour, add this mod as a soft dependency and call `ItemsBounceBack.TryAddBounceEntry(itemAssetName, bool)` somewhere in your code (this only needs to be called once, don't put it in `Update()` )

#

If you like my mods, please consider supporting me via one of the badges below!

<a href="https://ko-fi.com/V7V7JC77Y"><img src="https://cdn.prod.website-files.com/5c14e387dab576fe667689cf/64f1a9ddd0246590df69e9f9_ko-fi_logo_03-p-500.png" height="50" ></a>
<a href="https://www.patreon.com/SeroRonin"><img src="https://static-00.iconduck.com/assets.00/patreon-icon-2048x2048-f80b89j2.png" height="50" ></a>

#