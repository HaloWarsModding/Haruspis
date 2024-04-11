using Ethereal.ModManager.UI;

namespace Ethereal.ModManager.Dialogs
{
    public class DBoxes
    {
        public static EtherealBox CreateDialogBox(DBoxType type, string value = "")
        {
            return type switch
            {
                DBoxType.GameNotFound => new EtherealBox(
                                        EtherealBox.EtherealBoxDialog.YesNo,
                                        Properties.Resources.GameNotFoundTitle,
                                        Properties.Resources.GameNotFoundContent,
                                        Properties.Resources.GameNotFoundYes,
                                        Properties.Resources.GameNotFoundNo),
                DBoxType.Welcome => new EtherealBox(
                                        EtherealBox.EtherealBoxDialog.Yes,
                                        Properties.Resources.WelcomeTitle,
                                        string.Format(Properties.Resources.WelcomeContent, value),
                                        Properties.Resources.WelcomeYes),
                DBoxType.Distribution => new EtherealBox(
                                        EtherealBox.EtherealBoxDialog.YesNo,
                                        Properties.Resources.DistributionTitle,
                                        string.Format(Properties.Resources.DistributionContent, value),
                                        Properties.Resources.DistributionYes,
                                        Properties.Resources.DistributionNo),
                DBoxType.SelectMod => new EtherealBox(
                                        EtherealBox.EtherealBoxDialog.Yes,
                                        Properties.Resources.SelectModTitle,
                                        string.Format(Properties.Resources.SelectModContent, value),
                                        Properties.Resources.SelectModYes),
                DBoxType.RemoveMod => new EtherealBox(
                                        EtherealBox.EtherealBoxDialog.Yes,
                                        Properties.Resources.RemoveModTitle,
                                        Properties.Resources.RemoveModContent,
                                        Properties.Resources.RemoveModYes),
                _ => throw new ArgumentException("Invalid box type"),
            };
        }
    }

    public enum DBoxType
    {
        GameNotFound,
        Welcome,
        Distribution,
        SelectMod,
        RemoveMod
    }

}