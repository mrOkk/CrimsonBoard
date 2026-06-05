#if RewardedAdv_yg
namespace YG
{
    using System;
    using UnityEngine;
    using YandexMobileAds;
    using YandexMobileAds.Base;
    using YG.Insides;

    public class YMA_RewardAdv_MainThread : MonoBehaviour
    {
        private RewardedAdLoader rewardedAdLoader;
        private RewardedAd rewardedAd;

        public void Setup()
        {
            rewardedAdLoader = new RewardedAdLoader();
            rewardedAdLoader.OnAdLoaded += HandleAdLoaded;
            rewardedAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
        }

        public void RequestAd()
        {
            //Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(YG2.infoYG.common.yandexMobSetAgeRestrictedUser);

            string adUnitId = YG2.infoYG.common.yandexMobAdsTestingMode
                ? "demo-rewarded-yandex"
                : YG2.infoYG.common.yandexMobRewardAdID;

            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
            }

            AdRequestConfiguration config = new AdRequestConfiguration.Builder(adUnitId).Build();
            rewardedAdLoader.LoadAd(config);
        }

        public void ShowRewardedAd()
        {
            if (rewardedAd == null) return;

            rewardedAd.OnAdClicked += HandleAdClicked;
            rewardedAd.OnAdShown += HandleAdShown;
            rewardedAd.OnAdFailedToShow += HandleAdFailedToShow;
            rewardedAd.OnAdDismissed += HandleAdDismissed;
            rewardedAd.OnRewarded += HandleRewarded;

            rewardedAd.Show();
        }

        #region Rewarded callback handlers

        private void HandleAdLoaded(object sender, RewardedAdLoadedEventArgs args)
        {
            rewardedAd = args.RewardedAd;
            YG2.optionalPlatform.onLoadedRewardedAdv?.Invoke();
        }

        private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) => YGInsides.ErrorRewardedAdv();
        private void HandleAdClicked(object sender, EventArgs args) => YG2.optionalPlatform.onClickedRewardedAdv?.Invoke();

        private void HandleAdShown(object sender, EventArgs args) => YGInsides.OpenRewardedAdv();

        public void HandleRewarded(object sender, Reward args) => YGInsides.RewardAdv();

        private void HandleAdDismissed(object sender, EventArgs args)
        {
            DestroyAd();
            YGInsides.CloseRewardedAdv();
        }

        private void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            DestroyAd();
            YGInsides.ErrorRewardedAdv();
        }

        private void DestroyAd()
        {
            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
                rewardedAd = null;
            }
        }

        #endregion
    }
}
#endif