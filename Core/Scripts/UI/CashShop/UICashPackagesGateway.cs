using UnityEngine;

namespace MultiplayerARPG
{
    public class UICashPackagesGateway : MonoBehaviour
    {
        public bool alwaysShowPackagesUI;
        public string nonMobileUrl = "http://localhost/";
        public UICashPackages uiCashPackages;

        public void OnClickShowCashPackages()
        {
            if (Application.isMobilePlatform || alwaysShowPackagesUI)
            {
                if (uiCashPackages != null)
                    uiCashPackages.Show();
                return;
            }
            Application.OpenURL(nonMobileUrl);
        }
    }
}
