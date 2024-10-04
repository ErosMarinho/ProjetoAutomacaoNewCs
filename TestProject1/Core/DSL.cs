using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;

namespace ProjetoAutomacaoCEFCs.Core
{
    public class DSL : LogSystem
    {
        #region Funções de manipulação
        public static void Wait(int ms) => Thread.Sleep(ms);
        public void ClickOut() => ClicaElemento("//html");
        public void OpenNewTab(int tab, [Optional] string url)
        {
            ((IJavaScriptExecutor)driver)
                .ExecuteScript(string.Format("window.open('', '_blank');")); ChangeTab(tab);
            if (!string.IsNullOrEmpty(url)) driver.Navigate().GoToUrl(url);

        }
        public void ChangeTab(int tab) => driver.SwitchTo().Window(driver.WindowHandles[tab]);
        public void RefreshPage() => driver.Navigate().Refresh();
        public void LimpaDados(string xpath)
        {
            var element = driver.FindElement(By.XPath(xpath)); element.Clear();
            Actions act = new(driver); act.DoubleClick(element).Perform(); element.SendKeys(Keys.Delete);
            element.SendKeys(Keys.Control + "a"); element.SendKeys(Keys.Delete);
            var text = CapturaDados(xpath); ClicaElemento(xpath, null, 100); element.SendKeys(Keys.End);
            for (int i = 0; i < text.Length; i++) element.SendKeys(Keys.Backspace);
        }
        public void EsperaElemento(string xpath, int seconds = 120)
        {
            try
            {
                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(seconds));
                wait.Until((d) => d.FindElement(By.XPath(xpath)));
            }
            catch
            { testPassed = false; Log("Erro ao validar elemento esperado"); Assert.Fail(); }
        }
        public void EsperaElementoSumir(string xpath, int seconds = 120)
        {
            try
            {
                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(seconds));
                wait.Until(d => d.FindElements(By.XPath(xpath)).Count == 0);
            }
            catch
            { testPassed = false; Log("Erro ao validar a ausência do elemento esperado"); Assert.Fail(); }
        }

        public bool ValidaElementoExistente(string xpath)
        {
            try { driver.FindElement(By.XPath(xpath)); return true; }
            catch (NoSuchElementException) { return false; }
        }

        #endregion

        #region Funções de interação

        public void ClicaElemento(string xpath, [Optional] string? description, int ms = 1000)
        {
            try
            {
                driver.FindElement(By.XPath(xpath)).Click(); Wait(ms);
                if (description != null) Log($"Clicou em {description}");
            }
            catch (Exception ex)
            {
                testPassed = false; var msgErr = $"Erro ao clicar em {description}";
                if (description != null) Log($"{msgErr} {sysMsgErr} {ex.Message}"); Assert.Fail(msgErr);
            }
        }
        public void EscreveTexto(string xpath, string value, [Optional] string description)
        {
            try
            {
                driver.FindElement(By.XPath(xpath)).SendKeys(value);
                if (description != null) Log($"Preencheu {description}");
            }
            catch (Exception ex)
            {
                testPassed = false; var msgErr = $"Erro ao preencher {description}";
                if (description != null) Log($"{msgErr} {sysMsgErr} {ex.Message}"); Assert.Fail(msgErr);
            }
        }
        public void MenuDropDown(string xpath, string value, [Optional] string description, bool absolutValue = true)
        {
            try
            {
                string xpathValue = string.Empty;
                if (absolutValue) xpathValue = $"//*[text()='{value}']";
                else xpathValue = $"//*[contains(text(),'{value}')]";
                driver.FindElement(By.XPath(xpath)).Click(); EsperaElemento(xpathValue, 10);
                driver.FindElement(By.XPath(xpathValue)).Click();
                if (description != null) Log($"Selecionou menu Dropdown {description}");
            }
            catch (Exception ex)
            {
                testPassed = false; var msgErr = $"Erro ao selecionar menu Dropdown {description}";
                if (description != null) Log($"{msgErr} {sysMsgErr} {ex.Message}"); Assert.Fail(msgErr);
            }
        }
        public void MenuDropDownByXpath(string xpath, string valueXpath, [Optional] string description)
        {
            try
            {
                driver.FindElement(By.XPath(xpath)).Click(); EsperaElemento(valueXpath, 10);
                driver.FindElement(By.XPath(valueXpath)).Click();
                if (description != null) Log($"Selecionou menu Dropdown {description}");
            }
            catch (Exception ex)
            {
                testPassed = false; var msgErr = $"Erro ao selecionar menu Dropdown {description}";
                if (description != null) Log($"{msgErr} {sysMsgErr} {ex.Message}"); Assert.Fail(msgErr);
            }
        }
        public void ValidaDados(string xpath, string value, [Optional] string description)
        {
            try
            {
                Assert.That(driver.FindElement(By.XPath(xpath)).Text, Does.Contain(value));
                if (description != null) Log($"Validou {description}");
            }
            catch (Exception ex)
            {
                testPassed = false; var msgErr = $"Erro ao validar {description}";
                if (description != null) Log($"{msgErr} {sysMsgErr} {ex.Message}"); Assert.Fail(msgErr);
            }
        }
        public void ValidaDados(string xpath, string value, GetAttrib ga, [Optional] string description)
        {
            try
            {
                if (ga == GetAttrib.value)
                    Assert.That(driver.FindElement(By.XPath(xpath)).GetAttribute("value"), Does.Contain(value));
                if (description != null) Log($"Validou {description}");
            }
            catch (Exception ex)
            {
                testPassed = false; var msgErr = $"Erro ao validar {description}";
                if (description != null) Log($"{msgErr} {sysMsgErr} {ex.Message}"); Assert.Fail(msgErr);
            }
        }
        public void ValidaValorVariavelParcial(string variable, string value, [Optional] string description)
        {
            try
            {
                Assert.That(variable, Does.Contain(value));
                if (description != null) Log($"Validou {description}");
            }
            catch (Exception ex)
            {
                testPassed = false; var msgErr = $"Erro ao validar {description}";
                if (description != null) Log($"{msgErr} {sysMsgErr} {ex.Message}"); Assert.Fail(msgErr);
            }
        }
        public void ValidaValorVariavelAbsoluto(string var_x, string var_y, [Optional] string description)
        {
            try
            {
                Assert.That(var_x, Does.Match(var_y));
                if (description != null) Log($"Validou {description}");
            }
            catch (Exception ex)
            {
                testPassed = false; var msgErr = $"Erro ao validar {description}";
                if (description != null) Log($"{msgErr} {sysMsgErr} {ex.Message}"); Assert.Fail(msgErr);
            }
        }
        public void ValidaElementoAusente(string xpath, [Optional] string description)
        {
            var elementDisplayed = driver.FindElements(By.XPath(xpath));
            if (elementDisplayed.Count == 0)
            {
                if (description != null) Log($"Elemento {description} <b>não</b> está presente. Ok!");
            }
            else
            {
                testPassed = false; var msgErr = $"Elemento {description} está presente. Falhou!";
                if (description != null) Log(msgErr); Assert.Fail(msgErr);
            }
        }
        public void ValidaElementoPresente(string xpath, [Optional] string description)
        {
            var elementDisplayed = driver.FindElements(By.XPath(xpath));
            if (elementDisplayed.Count != 0)
            {
                if (description != null) Log($"Elemento {description} está presente.");
            }
            else
            {
                testPassed = false; var msgErr = $"Elemento {description} <b>não</b> está presente.";
                if (description != null) Log(msgErr); Assert.Fail(msgErr);
            }
        }
        public void ValidaElementoDesabilitado(string xpath, [Optional] string description)
        {
            var elementEnabled = driver.FindElement(By.XPath(xpath)).GetAttribute("disabled");
            if (elementEnabled == "true")
            {
                if (description != null) Log($"Elemento {description} está desabilitado. Ok!");
            }
            else
            {
                testPassed = false; var msgErr = $"Elemento {description} está habilitado. Falhou!";
                if (description != null) Log(msgErr); Assert.Fail(msgErr);
            }
        }
        public void ValidaElementoDesabilitadoSemAtributo(string xpath, [Optional] string description)
        {
            var elementEnabled = driver.FindElement(By.XPath(xpath)).GetAttribute("disabled");
            if (elementEnabled != "")
            {
                if (description != null) Log($"Elemento {description} está desabilitado. Ok!");
            }
            else
            {
                testPassed = false; var msgErr = $"Elemento {description} está habilitado. Falhou!";
                if (description != null) Log(msgErr); Assert.Fail(msgErr);
            }
        }
        public void ValidaMaxLengthCampo(string xpath, string capturedValue)
        {
            try
            {
                string maxLength = driver.FindElement(By.XPath(xpath)).GetAttribute("maxlength");
                Assert.That(maxLength, Does.Match(capturedValue));
                Log($"Validou max length de {capturedValue} caracteres");
            }
            catch (Exception ex)
            {
                testPassed = false; var msgErr = $"Erro ao validar max length de {capturedValue} caracteres";
                Log($"{msgErr} {sysMsgErr} {ex.Message}"); Assert.Fail(msgErr);
            }
        }
        public void ValidaMaxLengthCampo(string xpath, string capturedValue, [Optional] string description)
        {
            try
            {
                string maxLength = driver.FindElement(By.XPath(xpath)).GetAttribute("maxlength");
                Assert.That(maxLength, Does.Match(capturedValue));
                Log($"Validou max length de {capturedValue} caracteres {description}");
            }
            catch (Exception ex)
            {
                testPassed = false; var msgErr = $"Erro ao validar max length de {capturedValue} caracteres {description}";
                Log($"{msgErr} {sysMsgErr} {ex.Message}"); Assert.Fail(msgErr);
            }
        }

        public void UploadArquivo(string input, string path, [Optional] string description)
        {
            try
            {
                driver.FindElement(By.XPath(input)).SendKeys(path);
                if (description != null) Log($"Selecionou o arquivo {description} para upload com sucesso!");
            }
            catch (Exception ex)
            {
                testPassed = false; var msgErr = $"Erro ao selecionar o arquivo {description} para upload";
                if (description != null) Log($"{msgErr} {sysMsgErr} {ex.Message}"); Assert.Fail(msgErr);
            }
        }
        public void ValidaDownload([Optional] string downloadedFileName, [Optional] string description, bool deleteFile = true, int time = 90)
        {
            if (!cicdMode)
            {
                try
                {
                    string fullPathFile;
                    if (downloadedFileName == null)
                    {
                        var path = new DirectoryInfo(downloadsPath);
                        var files = path.GetFiles();
                        var orderedFiles = files.OrderBy(x => x.CreationTime);
                        Wait(1000); var lastFile = orderedFiles.Last();
                        var lastFileName = lastFile.Name;
                        fullPathFile = path + lastFileName;
                        Log($"<i>Nome do arquivo não foi informado!<br>Arquivo capturado no diretório: <b>{lastFileName}</b></i>");
                    }
                    else
                        fullPathFile = downloadsPath + downloadedFileName;

                    bool fileExists = false;
                    WebDriverWait wait = new(driver, TimeSpan.FromSeconds(30));
                    wait.Until(x => fileExists = File.Exists(fullPathFile));

                    var length = new FileInfo(fullPathFile).Length;
                    for (var i = 0; i < time; i++)
                    {
                        Wait(1000); var newLength = new FileInfo(fullPathFile).Length;
                        if (newLength == length && length != 0) break; length = newLength;
                    }
                    if (deleteFile) File.Delete(fullPathFile);
                    if (description != null) Log($"Download do arquivo {description} concluído com sucesso!");
                }
                catch (Exception ex)
                {
                    testPassed = false; var msgErr = $"Erro ao fazer o download do arquivo {description}";
                    if (description != null) Log($"{msgErr} {sysMsgErr} {ex.Message}"); Assert.Fail(msgErr);
                }
            }
        }
        public void EsperaElementoSerClicavel(string xpath, [Optional] string description)
        {
            try
            {
                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(30));
                var elementClick = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(xpath)));
                elementClick.Click(); if (description != null) Log($"Clicou em {description}");
            }
            catch (Exception ex)
            {
                testPassed = false; var msgErr = $"Erro ao clicar em {description}";
                if (description != null) Log($"{msgErr} {sysMsgErr} {ex.Message}"); Assert.Fail(msgErr);
            }
        }
        public void ValidaCondicaoVerdadeira(bool boolean, [Optional] string description)
        {
            try
            {
                Assert.That(boolean);
                if (description != null) Log($"Validou {description}");
            }
            catch (Exception ex)
            {
                testPassed = false; var msgErr = $"Erro ao validar {description}";
                if (description != null) Log($"{msgErr} {sysMsgErr} {ex.Message}"); Assert.Fail(msgErr);
            }
        }

        public void ValidaCheckboxSelecionado(string xpath, [Optional] string description)
        {
            var checkbox = driver.FindElement(By.XPath(xpath));
            if (checkbox.Selected == true)
            {
                if (description != null) Log($"Checkbox {description} está Selecionado. Ok!");
            }
            else
            {
                testPassed = false; var msgErr = $"Checkbox {description} não está selecionado. Falhou!";
                if (description != null) Log(msgErr); Assert.Fail(msgErr);
            }
        }

        public void ValidaCheckboxDesmarcado(string xpath, [Optional] string description)
        {
            var checkbox = driver.FindElement(By.XPath(xpath));
            if (checkbox.Selected == false)
            {
                if (description != null) Log($"Checkbox {description} está Desmarcado. Ok!");
            }
            else
            {
                testPassed = false; var msgErr = $"Checkbox {description} não está desmarcado. Falhou!";
                if (description != null) Log(msgErr); Assert.Fail(msgErr);
            }
        }

        public void ValidaElementoNumerico(string xpath)
        {
            var value = driver.FindElement(By.XPath(xpath)).Text.Trim();
            bool result = int.TryParse(value, out _);
            try
            {
                Assert.That(result); Log($"Elemento {value} é numérico. Ok!");
            }
            catch
            {
                testPassed = false; var msgErr = $"Elemento {value} não é numérico. Falhou!";
                Log(msgErr); Assert.Fail(msgErr);
            }
        }
        public void ValidaElementoNumerico(string xpath, GetAttrib ga)
        {
            var value = string.Empty;
            if (ga == GetAttrib.value)
                value = driver.FindElement(By.XPath(xpath)).GetAttribute("value");
            bool result = int.TryParse(value, out _);
            try
            {
                Assert.That(result); Log($"Elemento {value} é numérico. Ok!");
            }
            catch
            {
                testPassed = false; var msgErr = $"Elemento {value} não é numérico. Falhou!";
                Log(msgErr); Assert.Fail(msgErr);
            }
        }

        #endregion

        #region Funções de atribuição
        public void LogHeaderGen(string logMenu, string logDescription) =>
            Log(@$"
                <b>________________________________________</b><br><br>
                <b>MENU:</b> {logMenu}<br>
                <b>TESTE:</b> 
        {logDescription}<br>
                <b>DATA:</b> {dateNow:dd/MM/yyyy '<b>HORA: </b>' HH'h'mm'min'}<br>
                <b>________________________________________</b><br>");

        public string CapturaDados(string xpath)
        {
            try
            {
                string value = driver.FindElement(By.XPath(xpath)).Text.Trim();
                if (string.IsNullOrEmpty(value))
                    value = driver.FindElement(By.XPath(xpath)).GetAttribute("value");
                return value;
            }
            catch (Exception ex)
            {
                testPassed = false; var msgErr = $"Erro ao capturar elemento {xpath}";
                Log($"{msgErr} {sysMsgErr} {ex.Message}"); Assert.Fail(msgErr);
                return string.Empty;
            }
        }
        public string CapturaDados(string xpath, GetAttrib ga)
        {
            try
            {
                return ga switch
                {
                    GetAttrib.innerText => (string)((IJavaScriptExecutor)driver).ExecuteScript($"return document.evaluate(\"{xpath}\", " +
                        $"document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.value || document.evaluate(\"{xpath}\", " +
                        $"document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.textContent;"),
                    GetAttrib.value => driver.FindElement(By.XPath(xpath)).GetAttribute("value"),
                    GetAttrib.mask => driver.FindElement(By.XPath(xpath)).GetAttribute("mask"),
                    GetAttrib.maxlength => driver.FindElement(By.XPath(xpath)).GetAttribute("maxlength"),
                    _ => string.Empty
                };
            }
            catch (Exception ex)
            {
                testPassed = false; var msgErr = $"Erro ao capturar elemento {xpath}";
                Log($"{msgErr} {sysMsgErr} {ex.Message}"); Assert.Fail(msgErr);
                return string.Empty;
            }
        }
        
        public static string GeraStringPorTamanho(int times, string value)
        {
            string s = string.Empty;
            for (int i = 0; i < times; i++)
                s = string.Concat(s, value);
            return s;
        }

        public string PegaUrlAtual() => driver.Url;

        public string PegaTituloPagina() => driver.Title;

        public string PdfReader(string fileName, bool deleteFile = true)
        {
            string dataResult = string.Empty;
            var pdfFile = downloadsPath + fileName;
            var pdfReader = new PdfReader(pdfFile);
            var pdfDoc = new PdfDocument(pdfReader);

            for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
            {
                ITextExtractionStrategy data = new SimpleTextExtractionStrategy();
                dataResult += PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), data);
            }
            pdfDoc.Close(); Wait(500);
            if (deleteFile) File.Delete(pdfFile);
            return dataResult;
        }
        public string ExcelReader(string excelFileName, bool deleteFile = true)
        {
            string resultData = string.Empty;
            var fullPathFile = downloadsPath + excelFileName;
            var wbook = new XLWorkbook(fullPathFile);
            var ws = wbook.Worksheets.First();
            var range = ws.RangeUsed();

            for (int i = 1; i < 0; i++)
            {
                for (int j = 1; j < 0; j++)
                {
                    resultData += ws.Cell(i, j).GetValue<string>() + " ";
                }
            }
            if (deleteFile) File.Delete(fullPathFile);
            return resultData;
        }
        public string TextReader(string fileName, bool deleteFile = false)
        {
            var txtFile = downloadsPath + fileName;
            var resultData = File.ReadAllText(txtFile);
            if (deleteFile) File.Delete(txtFile); return resultData;
        }
        public string FileReaderByLine(string fileName, int line, bool deleteFile = true)
        {
            var txtFile = downloadsPath + fileName;
            var resultData = File.ReadLines(txtFile).ElementAtOrDefault(line);
            if (deleteFile) File.Delete(txtFile); 
            return fileName;
        }

        public void TiraScreenShot()
        {
            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
            var ssBase64 = ss.AsBase64EncodedString;
            var ssHtml = $"<img src='data:image/png;base64, {ssBase64}' width='370' height='166'/>";
            log.Append($"<br>{ssHtml}<br><br>");
        }
        #endregion

        #region Funções de simulação de teclas
        public void Tab(int timer = 100)
        {
            Actions act = new(driver);
            act.SendKeys(Keys.Tab).Perform(); Wait(timer);
        }
        public void Enter(int timer = 100)
        {
            Actions act = new(driver);
            act.SendKeys(Keys.Enter).Perform(); Wait(timer);
        }
        public void Delete(int timer = 100)
        {
            Actions act = new(driver);
            act.SendKeys(Keys.Delete).Perform(); Wait(timer);
        }
        public void PageDown(int timer = 100)
        {
            Actions act = new(driver);
            act.SendKeys(Keys.PageDown).Perform(); Wait(timer);
        }
        public void Backspace(int timer = 100)
        {
            Actions act = new(driver);
            act.SendKeys(Keys.Backspace).Perform(); Wait(timer);
        }
        #endregion

        #region Geradores de Dados Aleatórios

        #region Variáveis
        static Random rnd = new();
        static string dbPath = @"..\..\..\Core\DataBase";
        static string[] nomes = File.ReadAllLines($@"{dbPath}\nomes.txt");
        static string[] sobrenomes = File.ReadAllLines($@"{dbPath}\sobrenomes.txt");
        static string[] dominios = File.ReadAllLines($@"{dbPath}\dominios.txt");
        static string[] ceps = File.ReadAllLines($@"{dbPath}\ceps.txt");
        #endregion

        public static string GeraNomeAleatorio()
        {
            string nomeCompleto = $"{nomes[rnd.Next(nomes.Length)]} {sobrenomes[rnd.Next(sobrenomes.Length)]}";
            return nomeCompleto;
        }

        public static string GeraEmailAleatorio()
        {
            int num = rnd.Next(1, 9999);
            string email = $"{nomes[rnd.Next(nomes.Length)]}{sobrenomes[rnd.Next(sobrenomes.Length)]}{num}"
                + $"{dominios[rnd.Next(dominios.Length)]}"; return email.ToLower();
        }

        public static string GeraDataNascimento()
        {
            int dia = rnd.Next(1, 28); int mes = rnd.Next(1, 12);
            int ano = rnd.Next(1970, 2000); string data = dia.ToString().PadLeft(2, '0')
                + mes.ToString().PadLeft(2, '0') + ano; return data;
        }

        public static string GeraTelefoneFixo()
        {
            string digit = string.Empty; for (int i = 0; i < 10; i++)
                digit = string.Concat(digit, rnd.Next(10)); return digit;
        }

        public static string GeraCelular()
        {
            string digit = string.Empty; for (int i = 0; i < 11; i++)
                digit = string.Concat(digit, rnd.Next(10)); return digit;
        }

        public static string GeraCPF()
        {
            int soma = 0, resto; string cpf = string.Empty;

            for (int i = 0; i < 9; i++) cpf += rnd.Next(0, 9);
            for (int i = 0; i < 9; i++) soma += int.Parse(cpf[i].ToString()) * (10 - i);

            resto = soma % 11; if (resto < 2) cpf += "0"; else cpf += (11 - resto);
            soma = 0; for (int i = 0; i < 10; i++) soma += int.Parse(cpf[i].ToString()) * (11 - i);
            resto = soma % 11; if (resto < 2) cpf += "0"; else cpf += 11 - resto; return cpf;
        }

        public static string GeraCNPJ()
        {
            int[] times1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
            int[] times2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

            string cnpj = rnd.Next(10000000, 99999999).ToString() + "0001";

            int soma = 0; for (int i = 0; i < 12; i++) soma += int.Parse(cnpj[i].ToString()) * times1[i];
            int div = soma % 11; if (div < 2) div = 0; else div = 11 - div; cnpj += div; soma = 0;

            for (int i = 0; i < 13; i++) soma += int.Parse(cnpj[i].ToString()) * times2[i];
            div = soma % 11; if (div < 2) div = 0; else div = 11 - div; return cnpj + div.ToString();
        }

        public static string GeraCEP() { return ceps[rnd.Next(ceps.Length)]; }

        public static string GeraStringAlfanumericoAleatorio(int tamanho)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new string(
                Enumerable.Repeat(chars, tamanho)
                .Select(s => s[rnd.Next(s.Length)]).ToArray()); Wait(500);
            return result;
        }
        public static string GeraStringNumericaAleatoria(int tamanho)
        {
            var chars = "123456789";
            var result = new string(
            Enumerable.Repeat(chars, tamanho)
                .Select(s => s[rnd.Next(s.Length)]).ToArray()); Wait(500);
            return result;
        }

        #endregion

        #region Geradores de Arquivos

        public void SaveFileTxt(string filePath)
        {
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            string file = $"{filePath}{fileName}.txt";
            File.WriteAllText(file, fileData);
        }

        public void SaveFileHtml(string filePath)
        {
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            string file = $"{filePath}{fileName}.html";
            File.WriteAllText(file, fileData);
        }

        #endregion
    }
}
