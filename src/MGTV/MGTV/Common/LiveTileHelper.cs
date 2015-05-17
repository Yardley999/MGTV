using System;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace MGTV.Common
{
    public class LiveTileHelper
    {
        public static async void Create()
        {
            //var tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare310x310SmallImagesAndTextList05);

            //var tileTextAttributes = tileXml.GetElementsByTagName("text");
            //tileTextAttributes[0].InnerText = (new Random()).NextDouble().ToString();
            //tileTextAttributes[1].InnerText = "adfasdfa";

            //var tileNotification = new TileNotification(tileXml)
            //{
            //    ExpirationTime = DateTimeOffset.UtcNow.AddDays(1)
            //};
            //TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);

            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150ImageAndText01);

            XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");
            tileTextAttributes[0].InnerText = "Hello World! My very own tile notification";

            XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///assets/defaultTileImage.png");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "red graphic");

            XmlDocument squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text04);
            XmlNodeList squareTileTextAttributes = squareTileXml.GetElementsByTagName("text");
            squareTileTextAttributes[0].AppendChild(squareTileXml.CreateTextNode("Hello World! My very own tile notification"));
            IXmlNode node = tileXml.ImportNode(squareTileXml.GetElementsByTagName("binding").Item(0), true);
            tileXml.GetElementsByTagName("visual").Item(0).AppendChild(node);

            TileNotification tileNotification = new TileNotification(tileXml);

            tileNotification.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(1000000);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }
    }
}
