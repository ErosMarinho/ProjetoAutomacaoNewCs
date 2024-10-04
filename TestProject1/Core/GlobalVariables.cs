using OpenQA.Selenium;

namespace ProjetoAutomacaoCEFCs.Core
{
    public class GlobalVariables
    {
        #region Global Variables

        // Define 'driver' como trigger para os WebElements
        public required IWebDriver driver;

        // Define 'Fechar navegador ao final do teste' como padrão
        public bool driverQuit = false;

        // Habilita | Desabilita modo Headless
        public bool headless = false;

        // Executa | Não executa parâmetros específicos para execução em CI/CD
        public bool cicdMode = false;

        // Define 'true' como padrão para o fluxo dos testes
        public bool testPassed = true;

        // Determina nome de arquivo log diferente do nome da classe
        public string? logFileName = null;

        // Variáveis de manupulação de arquivos
        public required string fileName, fileData, filePath;

        // Define lista de atributos de elementos web
        public enum GetAttrib { text, innerText, value, mask, maxlength }

        // Define formatos para datas
        public enum DateFormat { DiaMesAno, DiaMesAnoComBarra, AnoMesDia, AnoMesDiaComBarra }

        // Define datas 
        public DateTime startTest, dateNow = DateTime.Now;
        public string
            dateToday = DateTime.Now.ToString("dd/MM/yyyy"),
            dateYesterday = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"),
            dateTomorrow = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");

        #endregion

        #region Paths

        // Url do sistema
        public string url = "https://www2.correios.com.br/sistemas/buscacep/resultadoBuscaCepEndereco.cfm";
        public string url2 = "https://sampleapp.tricentis.com/101/index.php";

        // Define path padrão para download dos arquivos
        public string downloadsPath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Downloads\";

        #endregion
    }
}
