using CustomUI.GameplaySettings;

namespace SpoopyBloqs
{
    class SpoopyUI
    {
        public static void CreateUI()
        {
            var darthMaulToggle = GameplaySettingsUI.CreateToggleOption(GameplaySettingsPanels.ModifiersRight, "Spoopy Bloqs", "Missed a bloq? No worries, you can try again!", null, 0);
            darthMaulToggle.GetValue = Plugin.IsEnabled;
            darthMaulToggle.OnToggle += (value) => { Plugin.IsEnabled = value; };
        }

    }
}
