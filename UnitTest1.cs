using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
namespace selenium_practice;

public class Tests
{
    [SetUp]
public void Setup()
    {
    }

[Test]
public void Authorization_test()
    {
        //зайти в браузер
        var driver = new ChromeDriver();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(5000);
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/");
        //ввести логин
        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys("totoevdanil57@gmail.com");
        //ввести пароль
        var password = driver.FindElement(By.Id("Password"));
        password.SendKeys("EE2-Lrt-SpU-9R6");
        //нажать кнопку ввойти
        var button = driver.FindElement(By.Name("button"));
        button.Click();
        //Явное ожидание
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
        wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/news"));
        wait.Until(ExpectedConditions.TitleContains("Новости"));
        Assert.That(driver.Title, Does.Contain("Новости"));
        //закрыть браузер
        driver.Quit();
    }

[Test]
public void Test_2()
    {
        Assert.Pass();
    }
}
