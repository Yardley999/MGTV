﻿using NotificationsExtensions.BadgeContent;
using NotificationsExtensions.TileContent;
using System;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace MGTV.LiveTile
{
    public class LiveTileHelper
    {
        public static async Task PinSecondaryTileAsync(string id, string displayName, string arguments, TileSize tileSize)
        {
            if(IsPinned(id))
            {
                return;
            }

            Uri logo = new Uri("ms-appx:///assets/Logo.scale-100.png");
            Uri wideLogo = new Uri("ms-appx:///assets/WideLogo.scale-100.png");

            SecondaryTile secondaryTile = new SecondaryTile(id,
                                                            displayName,
                                                            "MGTV://" + arguments,
                                                            logo,
                                                            tileSize);

            secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
            secondaryTile.VisualElements.ShowNameOnWide310x150Logo = true;
            secondaryTile.VisualElements.ShowNameOnSquare310x310Logo = true;

            secondaryTile.VisualElements.Wide310x150Logo = wideLogo;
            secondaryTile.VisualElements.Square150x150Logo = logo;

            secondaryTile.VisualElements.ForegroundText = ForegroundText.Dark;

            await secondaryTile.RequestCreateAsync();
        }

        public static bool IsPinned(string id)
        {
            return SecondaryTile.Exists(id);
        }

        public static void UpdateSecondaryTileAsync(string id, string text, TimeSpan duration)
        {
            if(IsPinned(id))
            {

                ITileSquare150x150Text04 squareContent = TileContentFactory.CreateTileSquare150x150Text04();
                squareContent.TextBodyWrap.Text = text;

                ITileWide310x150ImageAndText02 tileContent = TileContentFactory.CreateTileWide310x150ImageAndText02();
                tileContent.TextCaption1.Text = text;
                tileContent.TextCaption2.Text = text + "123123123123";
                tileContent.Image.Src = "ms-appx:///Assets/test.jpg";
                tileContent.Square150x150Content = squareContent;

                var notify = tileContent.CreateNotification();
                notify.ExpirationTime = DateTime.Now.AddSeconds(duration.TotalSeconds);

                TileUpdateManager.CreateTileUpdaterForSecondaryTile(id).Update(notify);
                SecondaryTile secondaryTile = new SecondaryTile(id);
            }
        }

        public static void UpdateBadgeNumber(string id, uint number)
        {
            if(IsPinned(id))
            {
                BadgeNumericNotificationContent badgeContent = new BadgeNumericNotificationContent(number);
                BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(id).Update(badgeContent.CreateNotification());
            }
        }
    }
}
