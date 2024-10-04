namespace ProjetoAutomacaoCEFCs.Core
{
    public class GlobalObjects : Begin
    {

        #region Variáveis
        string
            logMenu = "Menu > Type of insurance",
            logRegister = "Register for Tricentis";


        #endregion
        #region Métodos
        public void LogHeaderMenu() => LogHeaderGen(logMenu, logRegister);
        #endregion

        #region Métodos Comuns
        String
             btnMenuAutomobile = "//*[@name='Navigation Automobile']",
             btnMenuTruck = "//*[@name='Navigation Truck']",
             btnMenuMotorcycle = "//*[@name='Navigation Motorcycle']",
             btnMenuCamper = "//*[@name='Navigation Camper']";
        public void ClicaBtnMenuAutomobile() => ClicaElemento(btnMenuAutomobile, "button menu Automobile");
        public void ClicaBtnMenuTruck() => ClicaElemento(btnMenuTruck, "button menu Truck");
        public void ClicaBtnMenuMotorcycle() => ClicaElemento(btnMenuMotorcycle, "button menu Motorcycle");
        public void ClicaBtnMenuCamper() => ClicaElemento(btnMenuCamper, "button menu Camper");
        #endregion

        #region Valida Mensagem
        String
            messageSucesso = "Sending e-mail success!",
            buttonOk = "//*[@class='confirm']";
        public void ValidaMensagemSucesso()
        {

            ValidaDados($"//*[text()='{messageSucesso}']", messageSucesso, GetAttrib.text, $"mensagem {messageSucesso} ");
        }
        public void ClicaBtnOk()
        {
            Wait(5000);
            ClicaElemento(buttonOk, "button OK");
        }

        #endregion
    }
}
