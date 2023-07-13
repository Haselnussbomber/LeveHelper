# Changelog

## [2.0.5] (2023-07-13)

- **Fixed:** Icon textures wouldn't load if the path was redirected by Penumbra.

## [2.0.4] (2023-07-08)

Maintenance update with optimizations and a lot of behind-the-scenes changes:

- The plugin now starts asynchronously, which should result in a slightly faster startup of the game.
- Instead of loading all item icons at once when opening the window, the icons are now only loaded when they scroll into the viewport.
- Instead of creating the LeveHelper window when the plugin loads, it's now created only when the player wants to open it. Likewise, the window and it's icons are now unloaded when the window gets closed.
- Instead of manually caching levequests and items, I extended the Lumina sheets and now let Lumina do most of the caching.

## [2.0.3] (2023-04-26)

Updated for Patch 6.4.

Some text has been translated into French and Japanese by using ChatGPT (commit [ee6316c](https://github.com/Haselnussbomber/LeveHelper/commit/ee6316c)).  
Please file a bug if any translation is incorrect.

## [2.0.2] (2023-04-23)

- **Fixed:** To prevent the tab order from being reset,the "Queue" and "Recipe Tree" tabs are now always displayed.

## [2.0.1] (2023-04-23)

### Levequest List

- **Fixed:** When displaying items for an accepted levequest, the list could be displayed incorrectly.
- **Fixed:** The width of the levemete column is no longer fixed to 100px in order to display the whole name, if the window is big enough.

## [2.0.0] (2023-04-21)

I haven't done Levequests in a while, which also means I haven't been working on this plugin. This was the last progress I made roughly a month ago.
I'm releasing this now, just so it doesn't go to waste. Hopefully it works correctly and isn't very buggy. :pray:

---

This is the biggest update to LeveHelper yet, featuring an all-new queue and recipe tree for crafting/gathering leves!

The LeveHelper window is now a bit smaller due to removed columns, so you might have to resize it. It's also now separated into tabs:

### Levequest List

This was and still is the main plugin window listing all levequests in the game.

- **Added:** Filters can now be collapsed to save screen real estate.
- **Added:** Clicking on an accepted levequest will open the quest journal.
- **Added:** Accepted crafting/gathering levequests will list their required items underneath the name.
- **Changed:** Tooltip and name colors have been adjusted.
  - Incomplete levequests are still displayed in red, completed in green.
  - Accepted levequests are now displayed in yellow.
  - Levequests ready for turn in are now displayed in a yellowish-green as a middle ground between accepted and completed.
  - Failed levequests are now displayed in orange as a middle ground between incomplete and accepted.
  - Unavailable levequests (which are based on your character's starting city) are now grayed out.
- **Changed:** Removed allowance cost column as it only affects Grand Company Leves which already have an indicator in their name.
- **Fixed:** The allowance costs of accepted levequests will no longer be included in the needed allowances calculation.

### Queue

This tab lists items you need to gather, fish and/or craft for your currently accepted leves.

- **Gather:** Gatherables are grouped by zone and sorted by teleport cost.
- **Other:** These items may be drops or being sold by vendors. For now, check on GarlandTools DB.
- **Craft:** Items you need to craft are sorted by dependency, so you can easily work your way down the list.

### Recipe Tree

This tab shows the recipe tree for all items required by your currently accepted craft leves.

### Configuration

Previously in its own window, the configuration now moved into its own tab.

## [0.1.7] (2023-02-03)

- **Added:** Added the option to filter for Accepted levequests to the "Status" filter.

## [0.1.6] (2023-02-03)

- **Added:** Accepted levequests will now be shown in yellow text.
- **Added:** A button to quickly set the filter "Type" to the current players job.
- **Changed:** The button to set the filter "Location" to the current zone will now only show when the zone is different.

## [0.1.5] (2023-01-13)

Preliminary update for Patch 6.3 Hotfix 1.

- **Fixed:** Future-proofed reading the number of allowances and active levequests.

## [0.1.4] (2023-01-12)

Preliminary update for Patch 6.3.

## [0.1.3] (2022-08-29)

### Wanted Targets

- **Added:** Zaghnal

## [0.1.2] (2022-08-23)

Updated for Patch 6.2 (Updated Signatures)

## [0.1.1] (2022-08-23)

Preliminary updated for Patch 6.2 (Dalamud changes: .NET 6 and API 7)

## [0.1.0] (2022-08-20)

- **Added:** Wanted Target and Treasure Coffer notifications!
  - Simply click on the name in the chat notification to open up the map with a flag set to its location.
- **Added:** A proper changelog!
- **Added:** The Dalamud Plugin Installer now has a button to visit the plugins GitHub repository.
- **Added:** The repository now has a sponsor link to my [Ko-Fi](https://ko-fi.com/haselnussbomber) page.
- **Fixed:** Levemete names are now properly formatted by the games name formatter. Not sure if this was needed, but I did it anyway.^^

Please report if you found something is not working or if you discover a wanted target, that the plugin didn't notify you for. Thank you!

## [0.0.5] (2022-07-30)

- **Added:** Type filter now has icons.
- **Fixed:** Location and Levemete filters will now reset when not available.

## [0.0.4] (2022-07-30)

- **Fixed:** Allowance days calculation is now correct.
- **Changed:** Class filter is now type filter and grouped into categories.

## [0.0.3] (2022-07-25)

- Bugfixes

## [0.0.2] (2022-07-25)

- **Added:** Allowance Cost column.
- **Added:** ImGui table features for reordering and hiding columns.

## [0.0.1] (2022-07-24)

First release! 🥳

[Unreleased]: https://github.com/Haselnussbomber/LeveHelper/compare/main...dev
[2.0.5]: https://github.com/Haselnussbomber/LeveHelper/compare/v2.0.4...v2.0.5
[2.0.4]: https://github.com/Haselnussbomber/LeveHelper/compare/v2.0.3...v2.0.4
[2.0.3]: https://github.com/Haselnussbomber/LeveHelper/compare/v2.0.2...v2.0.3
[2.0.2]: https://github.com/Haselnussbomber/LeveHelper/compare/v2.0.1...v2.0.2
[2.0.1]: https://github.com/Haselnussbomber/LeveHelper/compare/v2.0.0...v2.0.1
[2.0.0]: https://github.com/Haselnussbomber/LeveHelper/compare/v0.1.7...v2.0.0
[0.1.7]: https://github.com/Haselnussbomber/LeveHelper/compare/v0.1.6...v0.1.7
[0.1.6]: https://github.com/Haselnussbomber/LeveHelper/compare/v0.1.5...v0.1.6
[0.1.5]: https://github.com/Haselnussbomber/LeveHelper/compare/v0.1.4...v0.1.5
[0.1.4]: https://github.com/Haselnussbomber/LeveHelper/compare/v0.1.3...v0.1.4
[0.1.3]: https://github.com/Haselnussbomber/LeveHelper/compare/v0.1.2...v0.1.3
[0.1.2]: https://github.com/Haselnussbomber/LeveHelper/compare/v0.1.1...v0.1.2
[0.1.1]: https://github.com/Haselnussbomber/LeveHelper/compare/v0.1.0...v0.1.1
[0.1.0]: https://github.com/Haselnussbomber/LeveHelper/compare/v0.0.5...v0.1.0
[0.0.5]: https://github.com/Haselnussbomber/LeveHelper/compare/v0.0.4...v0.0.5
[0.0.4]: https://github.com/Haselnussbomber/LeveHelper/compare/v0.0.3...v0.0.4
[0.0.3]: https://github.com/Haselnussbomber/LeveHelper/compare/v0.0.2...v0.0.3
[0.0.2]: https://github.com/Haselnussbomber/LeveHelper/compare/v0.0.1...v0.0.2
[0.0.1]: https://github.com/Haselnussbomber/LeveHelper/commit/139f3c09
