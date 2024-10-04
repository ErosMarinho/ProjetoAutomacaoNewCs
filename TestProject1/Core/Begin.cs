

#region Bibliotecas
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;
using System.Diagnostics;
using OpenQA.Selenium.Chrome;
#endregion

namespace ProjetoAutomacaoCEFCs.Core
{
    public class Begin : DSL
    {
        #region Abre navegador
        public void AbreNavegador()
        {
            // Headless Mode
            var hm = new ChromeOptions();
            hm.AddArgument("disable-application-cache");
            hm.AddArgument("window-size=1920x1080");
            hm.AddUserProfilePreference("download.default_directory", downloadsPath);
            hm.AddArgument("inprivate");
            hm.AddArgument("no-sandbox");
            hm.AddArgument("disable-gpu");
            hm.AddArgument("headless");

            // Development Mode
            var dm = new ChromeOptions();
            dm.AddArgument("disable-application-cache");
            dm.AddArgument("inprivate");
            dm.AddArgument("no-sandbox");
            dm.AddArgument("start-maximized");

            if (headless) driver = new ChromeDriver(hm);
            else driver = new ChromeDriver(dm);

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }
        #endregion

        #region Define o acesso ao sistema

        public void AcessaSistema()
        {
            try { AbreNavegador(); driver.Navigate().GoToUrl(url2); startTest = DateTime.Now; }
            catch { new DriverManager().SetUpDriver(new EdgeConfig()); }
        }

        #endregion

        #region Encerra o teste
        public void EncerraTeste()
        {
            SaveLog(); if (driverQuit) driver.Quit();
            else foreach (var process in Process.GetProcessesByName("chromedriver")) process.Kill();
        }
        #endregion

        #region SetUp

        [SetUp]
        public void Start() => AcessaSistema();
        #endregion

        #region TearDown
        [TearDown]
        public void EndOfTest() => EncerraTeste();
        #endregion
    }
}
