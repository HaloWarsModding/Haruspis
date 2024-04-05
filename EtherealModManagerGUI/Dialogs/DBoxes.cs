using EtherealModManager.UI;

namespace EtherealModManager.Dialogs
{
    public class DBoxes
    {
        public static EtherealBox CreateDialogBox(DBoxType type)
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
                                        Properties.Resources.WelcomeContent,
                                        Properties.Resources.WelcomeYes),
                DBoxType.Distribution => new EtherealBox(
                                        EtherealBox.EtherealBoxDialog.YesNo,
                                        Properties.Resources.DistributionTitle,
                                        Properties.Resources.DistributionContent,
                                        Properties.Resources.DistributionYes,
                                        Properties.Resources.DistributionNo),
                _ => throw new ArgumentException("Invalid box type"),
            };
        }
    }

    public enum DBoxType
    {
        GameNotFound,
        Welcome,
        Distribution
    }

}