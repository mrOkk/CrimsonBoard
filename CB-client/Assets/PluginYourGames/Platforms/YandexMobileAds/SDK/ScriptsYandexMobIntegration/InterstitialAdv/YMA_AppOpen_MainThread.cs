#if InterstitialAdv_yg
namespace YG
{
    using System;
    using UnityEngine;
    using YandexMobileAds;
    using YandexMobileAds.Base;
    using YG.Insides;

    public class YMA_AppOpen_MainThread : MonoBehaviour
    {
        private AppOpenAdLoader appOpenAdLoader;
        private AppOpenAd appOpenAd;
        private bool isAdShowOnColdStart;
        private bool isAdLoadingProcess;

        public void Setup()
        {
            appOpenAdLoader = new AppOpenAdLoader();
            appOpenAdLoader.OnAdLoaded += HandleAdLoaded;
            appOpenAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;

            RequestInterstitial();

            if (YG2.infoYG.common.yandexMobAppOpenAdEnable)
                AppStateObserver.OnAppStateChanged += HandleAppStateChanged;
        }

        public void OnDestroy()
        {
            if (YG2.infoYG.common.yandexMobAppOpenAdEnable)
                AppStateObserver.OnAppStateChanged -= HandleAppStateChanged;
        }

        public void RequestInterstitial()
        {
            //Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(YG2.infoYG.common.yandexMobSetAgeRestrictedUser);

            string adUnitId = YG2.infoYG.common.yandexMobAdsTestingMode 
                ? "demo-appopenad-yandex"
                : YG2.infoYG.common.yandexMobAppOpenAdID;

            if (appOpenAd != null)
            {
                appOpenAd.Destroy();
            }

            AdRequestConfiguration config = new AdRequestConfiguration.Builder(adUnitId).Build();
            appOpenAdLoader.LoadAd(config);
            isAdLoadingProcess = true;
        }

        private void ShowAppOpenAd()
        {
            if (appOpenAd != null) appOpenAd.Show();
        }

        #region Interstitial callback handlers

        public void HandleAdLoaded(object sender, AppOpenAdLoadedEventArgs args)
        {
            isAdLoadingProcess = false;
            appOpenAd = args.AppOpenAd;

            appOpenAd.OnAdClicked += HandleAdClicked;
            appOpenAd.OnAdShown += HandleAdShown;
            appOpenAd.OnAdFailedToShow += HandleAdFailedToShow;
            appOpenAd.OnAdDismissed += HandleAdDismissed;

            if (!isAdShowOnColdStart
                && YG2.infoYG.InterstitialAdv.showFirstAdv)
            {
                ShowAppOpenAd();
            }

            isAdShowOnColdStart = true;
        }

        public void HandleAppStateChanged(object sender, AppStateChangedEventArgs args)
        {
            if (isAdShowOnColdStart && args.IsInBackground == false)
            {
                ShowAppOpenAd();
            }
        }

        private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) => YGInsides.ErrorInterAdv();
        private void HandleAdClicked(object sender, EventArgs args) => YG2.optionalPlatform.onClickedInterAdv?.Invoke();

        private void HandleAdShown(object sender, EventArgs args) => YGInsides.OpenInterAdv();

        private void HandleAdDismissed(object sender, EventArgs args)
        {
            AdvertisementWasShown();
            YGInsides.CloseInterAdv();
        }

        private void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            AdvertisementWasShown();
            YGInsides.ErrorInterAdv();
        }

        private void AdvertisementWasShown()
        {
            if (appOpenAd != null)
            {
                appOpenAd.Destroy();
                appOpenAd = null;
            }

            if (!isAdLoadingProcess
                && YG2.infoYG.common.yandexMobAppOpenAdEnable)
            {
                RequestInterstitial();
            }
        }

        #endregion
    }
}
#endif