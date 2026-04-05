using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
namespace selenium_practice;


public class Tests
{
// 1. Структура теста — есть Setup и Teardown, авторизация вынесена в отдельный метод
// 2. Переиспользование кода — повторяющиеся блоки вынесены в отдельные методы
// 3. Нет лишних UI-действий — например, используем переход по URL вместо клика по кнопкам меню,  если этого не требуется для проверки в рамках теста
// 4. Понятные сообщения в ассертах — при падении теста сразу ясно, что пошло не так
// 5. Человекочитаемые названия тестов — проверяющий понимает, что именно тестируется
// 6. Уникальные локаторы — используются там, где это возможно
// 7. Явные или неявные ожидания — тесты не падают из-за гонки с интерфейсом

//определяем поля класса
public IWebDriver driver;
public WebDriverWait wait;

[SetUp]
public void Setup() //выполняется перед каждым из тестов
    {
        driver = new ChromeDriver();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(5000);
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/");
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
    }

[TearDown]
public void Teardown() //выполняется после завешения каждого из тестов
    {
        driver.Quit();
        driver.Dispose();
    }

public void SignIn() //авторизация на сайте
    {
        //ввести логин
        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys("totoevdanil57@gmail.com");
        //ввести пароль
        var password = driver.FindElement(By.Id("Password"));
        password.SendKeys("EE2-Lrt-SpU-9R6");
        //нажать кнопку ввойти
        var button = driver.FindElement(By.Name("button"));
        button.Click();
        //Явное ожидание. Ждем когда url поменяется на ожидаемый
        wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/news"));
    }

[Test]
public void Authorization_test() //тестирование авторизации
    {     
        SignIn();

        Assert.That(driver.Url, Is.EqualTo("https://staff-testing.testkontur.ru/news"), 
        "Адрес в поисковой строке не поменялся на https://staff-testing.testkontur.ru/news - авторизация не прошла");
    }

[Test]
public void NavigateToComments_test() //тестирование открытия раздела "Комментарии"
    {
        SignIn();
        wait.Until(ExpectedConditions.TitleIs("Новости"));
        var burgerButton = driver.FindElement(By.CssSelector("[data-tid='SidebarMenuButton']"));
        burgerButton.Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='SidePage__container']")));

        //очень тонкий момент. SidePage__container - это панель которая открывается по нажатии на бургер
        //SidePageBody - это контйнер, в котором лежат все разделы меню. Именно тут нам нужно
        //искать наш "Раздел" Комментарии
        var sidebar = driver.FindElement(By.CssSelector("[data-tid='SidePageBody']"));   
        var comments = sidebar.FindElement(By.CssSelector("[data-tid='Comments']"));
        comments.Click();
        wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/comments"));
        var pageTitle = driver.FindElement(By.CssSelector("[data-tid='Title']"));

        Assert.That(pageTitle.Text, Does.Contain("Комментарии"), 
        "Заголовок раздела не 'Комментарии' - переход на страницу не удался");
    }
}
