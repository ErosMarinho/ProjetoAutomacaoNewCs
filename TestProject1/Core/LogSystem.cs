using OpenQA.Selenium;
using System.Text;

namespace ProjetoAutomacaoCEFCs.Core
{
    public class LogSystem : GlobalVariables
    {
        #region Variables

        public StringBuilder log = new(); string? logResult;

        string
            header = File.ReadAllText(@"..\..\..\Core\DataBase\base64logo.txt"),
            htmlStart = "<html><head><meta charset='utf-8'><title>Avivatec QA Team - Evidência</title></head><body style='background-color: #FFFFFF;'><p><font face = Verdana size = 2>",
            htmlEnd = "</p></font></body></html>", testOk = "<b><br>FIM DO TESTE – OK!",
            testNok = "<b><br>FIM DO TESTE – <font color = red>NOK!<p align=center>";

        public string sysMsgErr = "</b><p><font color = black><b>System Message Error: </b><i>";

        #endregion

        #region Log Funcion

        public void Log(string text)
        {
            if (testPassed) log.Append($"{text}<br>");
            else log.Append($"<font color = red><b>{text}");
        }

        #endregion

        #region TestTime Function

        public string TestTime()
        {
            TimeSpan ts = DateTime.Now - startTest;
            if (ts.TotalSeconds < 60)
                return "</i><p>___________________________<br>Teste executado em " + string.Format("<b>{0:ss}</b> segundos", ts);
            else
                return "</i><p>_________________________<br>Teste executado em " + string.Format("<b>{0:mm}</b>min<b>{0:ss}</b>s", ts);
        }

        #endregion

        #region SaveLog

        public void SaveLog()
        {
            if (cicdMode)
                filePath = $"{TestContext.CurrentContext.TestDirectory}\\{TestContext.CurrentContext.Test.MethodName}";
            else
            {
                string folderDate = DateTime.Now.ToString("yyyy-MM-dd");
                filePath = $@"..\..\..\..\Logs\{folderDate}\";
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            }

            if (logFileName != null) fileName = logFileName;
            else fileName = GetType().Name;
            string file = $"{filePath}{fileName}.html";

            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
            string testTime = TestTime(), image = $"<img src='data:/image/png;base64, {ss} 'width='972' height='435'/>";

            if (testPassed) logResult = $"{htmlStart}{header}{log}{testTime}{testOk}{htmlEnd}";
            else logResult = $"{htmlStart}{header}{log}{testTime}{testNok}{image}{htmlEnd}";
            File.WriteAllText(file, logResult); testPassed = true;

            if (cicdMode) TestContext.AddTestAttachment(file);
        }

        #endregion
    }
}
