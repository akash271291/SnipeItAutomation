using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace SnipeItAutomation.Tests
{
    // Login Page class
    public class LoginPage
    {
        private readonly IPage _page;

        public LoginPage(IPage page) => _page = page;

        public async Task NavigateAsync() =>
            await _page.GotoAsync("https://demo.snipeitapp.com/login");

    // Login Credentials
        public async Task LoginAsync(string username, string password)
        {
            await _page.FillAsync("#username", username);
            await _page.FillAsync("#password", password);
            await _page.ClickAsync("button[type='submit']");
        }
    }

    // Asset Creation Page class
    public class AssetCreationPage
    {
        private readonly IPage _page;
        private readonly Random _random = new();

        public AssetCreationPage(IPage page) => _page = page;

        public async Task NavigateToNewAssetAsync()
        {
            await _page.ClickAsync("a.dropdown-toggle:has-text(\"Create New\")");
            await _page.ClickAsync("ul.dropdown-menu >> text=Asset");
        }

        public async Task FillDetailsAsync(string modelName)
        {
            // Select company
            await _page.ClickAsync("#select2-company_select-container");
            await _page.WaitForSelectorAsync("input.select2-search__field");
            await _page.FillAsync("input.select2-search__field", "");
            var companies = _page.Locator(".select2-results__option");
            await companies.Nth(_random.Next(await companies.CountAsync())).ClickAsync();

            // Select model
            await _page.ClickAsync("#select2-model_select_id-container");
            await _page.FillAsync("input.select2-search__field", modelName);
            await _page.ClickAsync($"li.select2-results__option:has-text('{modelName}')");

            // Set status to "Ready to Deploy"
            await _page.SelectOptionAsync("#status_select_id", new[] { "1" });

            // Assign to user
            await _page.EvaluateAsync("document.getElementById('assignto_selector').style.display = 'block'");
            await _page.EvaluateAsync("document.getElementById('assigned_user').style.display = 'block'");
            await _page.CheckAsync("input[name='checkout_to_type'][value='user']");
            await _page.ClickAsync("#select2-assigned_user_select-container");
            await _page.FillAsync("input.select2-search__field", "");
            await _page.ClickAsync(".select2-results__option");

            // Select location
            await _page.ClickAsync("#select2-rtd_location_id_location_select-container");
            await _page.FillAsync("input.select2-search__field", "");
            await _page.ClickAsync(".select2-results__option");
        }

        public async Task<string> GetAssetTagAsync() =>
            await _page.InputValueAsync("#asset_tag");

        public async Task SubmitAsync() =>
            await _page.ClickAsync("button[id='submit_button']");
    }

    // Asset List Page class
    public class AssetListPage
    {
        private readonly IPage _page;

        public AssetListPage(IPage page) => _page = page;

        public async Task SearchAssetAsync(string tag)
        {
            await _page.GotoAsync("https://demo.snipeitapp.com/hardware");
            await _page.WaitForSelectorAsync("input.search-input");

            await _page.EvaluateAsync($@"() => {{
                const input = document.querySelector('input.search-input');
                input.focus();
                input.value = '{tag}';
                input.dispatchEvent(new Event('input', {{ bubbles: true }}));
                input.dispatchEvent(new KeyboardEvent('keydown', {{ key: 'Enter', bubbles: true }}));
                input.dispatchEvent(new KeyboardEvent('keyup', {{ key: 'Enter', bubbles: true }}));
            }}");

            await _page.WaitForTimeoutAsync(1500);
        }

        public async Task OpenAssetAndVerifyHistoryAsync(string tag)
        {
            await _page.ClickAsync($"a:has-text('{tag}')");
            await _page.ClickAsync("a[data-toggle='tab'][href='#history']");
            await _page.WaitForSelectorAsync("li.active a[href='#history']");
            await _page.WaitForSelectorAsync("div.tab-pane#history", new() { State = WaitForSelectorState.Visible });
        }
    }

    // Main Test Class
    [TestFixture]
    public class SnipeItTests
    {
        private IPlaywright _playwright = null!;
        private IBrowser _browser = null!;
        private IPage _page = null!;

        [SetUp]
        public async Task Setup()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });
            _page = await _browser.NewPageAsync();
        }

        [Test]
        public async Task CreateAndVerifyMacbookProAsset()
        {
            var login = new LoginPage(_page);
            var assetForm = new AssetCreationPage(_page);
            var assetList = new AssetListPage(_page);

            var assetName = $"Macbook Pro 13 - {Guid.NewGuid():N}".Substring(0, 20);

            await login.NavigateAsync();
            await login.LoginAsync("admin", "password");

            await assetForm.NavigateToNewAssetAsync();
            await assetForm.FillDetailsAsync("Macbook Pro");
            var assetTag = await assetForm.GetAssetTagAsync();
            Console.WriteLine($"Asset Tag: {assetTag}");

            await assetForm.SubmitAsync();

            await assetList.SearchAssetAsync(assetTag);
            await assetList.OpenAssetAndVerifyHistoryAsync(assetTag);

            Console.WriteLine($"Asset '{assetName}' created and verified successfully.");
        }

        [TearDown]
        public async Task TearDown()
        {
            await _browser.CloseAsync();
            _playwright.Dispose();
        }
    }
}
