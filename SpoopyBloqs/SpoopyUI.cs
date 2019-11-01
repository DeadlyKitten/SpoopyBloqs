using CustomUI.GameplaySettings;

namespace SpoopyBloqs
{
    class SpoopyUI
    {
        public static void CreateUI()
        {
            var spoopyToggle = GameplaySettingsUI.CreateToggleOption(GameplaySettingsPanels.ModifiersRight, "Spoopy Bloqs", "Missed a bloq? Try to hit its ghost!", null, 0);
            spoopyToggle.GetValue = Plugin.IsEnabled;
            spoopyToggle.OnToggle += (value) => { Plugin.IsEnabled = value; };
        }

    }
}
