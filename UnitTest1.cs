using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Interactions;
using System.Linq;

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
public void AuthorizationTest() //тестирование авторизации
    {     
        SignIn();

        Assert.That(driver.Url, Is.EqualTo("https://staff-testing.testkontur.ru/news"), 
        "Адрес в поисковой строке не поменялся на https://staff-testing.testkontur.ru/news - авторизация не прошла");
    }

[Test]
public void NavigateToCommentTest() //тестирование открытия раздела "Комментарии"
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

[Test]
public void SearchTest() //тестирование поисковой строки
    {
        SignIn();
        wait.Until(ExpectedConditions.TitleIs("Новости"));
        var searchBar = driver.FindElement(By.CssSelector("[data-tid='SearchBar']")); 
        searchBar.Click();
        var searchInput = driver.FindElement(By.CssSelector("[placeholder='Поиск сотрудника, подразделения, сообщества, мероприятия']"));
        searchInput.SendKeys("Тотоев Данил Алексеевич");

        Assert.That(searchInput.GetDomAttribute("value").Contains("Тотоев Данил Алексеевич"), "Поле поиска должно содержать введеный текст");
    }

[Test]
public void SendCommentTest() //тестирование отправки комментария в обсуждении "Для домашки DevTools"
    {
        SignIn();
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities/612a7485-7f49-48c9-8fe1-ee49b4435111?tab=discussions&id=66892117-a81f-4b3a-9e64-e09cedc18dc2");
        var addCommentButton = driver.FindElement(By.CssSelector("[data-tid='AddComment']"));
        addCommentButton.Click();
        var commentInput = driver.FindElement(By.CssSelector("[placeholder='Комментировать...']"));
        var commentsText = "autotest comment by Danil";
        commentInput.SendKeys(commentsText);
        
        //используем табуляцию для смещения фокуса на кнопку для отправки
        new Actions(driver).SendKeys(Keys.Tab).SendKeys(Keys.Enter).Perform(); 
        var commentsList = driver.FindElement(By.CssSelector("[data-tid='CommentsList']"));
        var comments = commentsList.FindElements(By.CssSelector("[data-tid='TextComment']"));
        var myComment = comments.Last();

        Assert.That(myComment.Text, Does.Contain(commentsText),
    $"Вместо введенного текста: '{commentsText}'. Отображается: '{myComment}'");
        Thread.Sleep(5000);
    }
}
