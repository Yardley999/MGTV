using System;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace MGTV.Common
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
                                                            TileSize.Wide310x150);

            secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
            secondaryTile.VisualElements.ShowNameOnWide310x150Logo = true;
            secondaryTile.VisualElements.ShowNameOnSquare310x310Logo = true;
            secondaryTile.VisualElements.Wide310x150Logo = wideLogo;

            secondaryTile.VisualElements.ForegroundText = ForegroundText.Dark;

            await secondaryTile.RequestCreateAsync();
        }

        public static bool IsPinned(string id)
        {
            return SecondaryTile.Exists(id);
        }
    }
}
