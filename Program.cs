using Cimber.Scraper;
using Cimber.Scraper.Models;

const string LOGO =
    "  ___                              ___  \r\n (o o)                            (o o) \r\n(  V  ) CIMBER SCRAPER v1.0.1 (  V  )\r\n--m-m------------------------------m-m--\n";

Console.WriteLine(LOGO);
Console.Write(
    "Please choose a website to scrape:\n1 - Gidonline (RU)\n2 - Kinogo (RU)\n3 - Kinokrad (RU)\n4 - Kinoprofi (RU)\n5 - Uakino (UA)\n6 - Uakinogo (UA)\n7 - Uafilm (UA)\n-> "
);
var website = Console.ReadLine();
Scraper scraper;

if (website!.Contains("1"))
{
    scraper = new Scraper(Website.GIDONLINE);
}
else if (website!.Contains("2"))
{
    scraper = new Scraper(Website.KINOGO);
}
else if (website!.Contains("3"))
{
    scraper = new Scraper(Website.KINOKRAD);
}
else if (website!.Contains("4"))
{
    scraper = new Scraper(Website.KINOPROFI);
}
else if (website!.Contains("5"))
{
    scraper = new Scraper(Website.UAKINO);
}
else if (website!.Contains("6"))
{
    scraper = new Scraper(Website.UAKINOGO);
}
else if (website!.Contains("7"))
{
    scraper = new Scraper(Website.UAFILM);
}
else
{
    scraper = new Scraper(Website.GIDONLINE);
}

Logger.InitLogger();
scraper.Start();
Console.ReadLine();
