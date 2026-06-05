#if InterstitialAdv_yg
namespace YG
{
    using System;
    using UnityEngine;
    using YandexMobileAds;
    using YandexMobileAds.Base;
    using YG.Insides;

    public class YMA_MainThread_InterAdv : MonoBehaviour
    {
        private InterstitialAdLoader interstitialAdLoader;
        private Interstitial interstitial;

        public void Setup()
        {
            if (YG2.infoYG.InterstitialAdv.showFirstAdv || YG2.infoYG.common.yandexMobAppOpenAdEnable)
            {
                YMA_AppOpen_MainThread appOpen;
                appOpen = gameObject.AddComponent<YMA_AppOpen_MainThread>();
                appOpen.Setup();
            }

            interstitialAdLoader = new InterstitialAdLoader();
            interstitialAdLoader.OnAdLoaded += HandleAdLoaded;
            interstitialAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
        }

        public void RequestInterstitial()
        {
            //Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(YG2.infoYG.common.yandexMobSetAgeRestrictedUser);

            string adUnitId = YG2.infoYG.common.yandexMobAdsTestingMode 
                ? "demo-interstitial-yandex" 
                : YG2.infoYG.common.yandexMobInterAdID;

            if (interstitial != null)
            {
                interstitial.Destroy();
            }

            AdRequestConfiguration config = new AdRequestConfiguration.Builder(adUnitId).Build();
            interstitialAdLoader.LoadAd(config);
        }

        public void ShowInterstitial()
        {
            if (interstitial == null) return;

            interstitial.OnAdClicked += HandleAdClicked;
            interstitial.OnAdShown += HandleAdShown;
            interstitial.OnAdFailedToShow += HandleAdFailedToShow;
            interstitial.OnAdDismissed += HandleAdDismissed;

            interstitial.Show();
        }

        #region Interstitial callback handlers

        private void HandleAdLoaded(object sender, InterstitialAdLoadedEventArgs args)
        {
            interstitial = args.Interstitial;
            YG2.optionalPlatform.onLoadedInterAdv?.Invoke();
        }

        private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) => YGInsides.ErrorInterAdv();
        private void HandleAdClicked(object sender, EventArgs args) => YG2.optionalPlatform.onClickedInterAdv?.Invoke();

        private void HandleAdShown(object sender, EventArgs args) => YGInsides.OpenInterAdv();

        private void HandleAdDismissed(object sender, EventArgs args)
        {
            DestroyAd();
            YGInsides.CloseInterAdv();
        }

        private void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            DestroyAd();
            YGInsides.ErrorInterAdv();
        }

        private void DestroyAd()
        {
            if (interstitial != null)
            {
                interstitial.Destroy();
                interstitial = null;
            }
        }

        #endregion
    }
}
#endif