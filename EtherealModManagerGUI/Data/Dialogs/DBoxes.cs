using EtherealModManagerGUI.UI;

namespace EtherealModManagerGUI.Data.Dialogs
{
    internal class DBoxes
    {
        public static EtherealBox GameNotFoundBox()
        {
            return new EtherealBox(
                EtherealBox.EtherealBoxDialog.YesNo,
                Properties.Resources.GameNotFoundTitle,
                Properties.Resources.GameNotFoundContent,
                Properties.Resources.GameNotFoundYes,
                Properties.Resources.GameNotFoundNo);
        }

        public static EtherealBox Welcome()
        {
            return new EtherealBox(
                EtherealBox.EtherealBoxDialog.Yes,
                Properties.Resources.WelcomeTitle,
                Properties.Resources.WelcomeContent,
                Properties.Resources.WelcomeYes);

        }

        public static EtherealBox Distribution()
        {
            return new EtherealBox(
                 EtherealBox.EtherealBoxDialog.YesNo,
                 Properties.Resources.DistributionTitle,
                 Properties.Resources.DistributionContent,
                 Properties.Resources.DistributionYes,
                 Properties.Resources.DistributionNo);
        }
    }
}
