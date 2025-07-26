
# SnipeItAutomation by AKASHDEEP

This project implements automated UI testing for the Snipe-IT demo asset management system using .NET 8, Microsoft Playwright, and NUnit. 

##  Objective

The automation script performs the following steps:

1. Logs into the Snipe-IT demo site at [https://demo.snipeitapp.com/login](https://demo.snipeitapp.com/login)
2. Creates a new asset: **Macbook Pro 13"**
3. Sets the asset status to **Ready to Deploy**
4. Checks out the asset to a **random user and location**
5. Verifies the asset exists in the asset list
6. Opens the asset details page and confirms key information
7. Navigates to the **History** tab and validates expected entries


##  Tech Stack

- .NET 8 SDK 
- Microsoft.Playwright
- NUnit
- NUnit3TestAdapter / Microsoft.NET.Test.Sdk 

## Prerequisites

Make sure the following tools are installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Microsoft Playwright CLI and browsers:


dotnet tool install --global Microsoft.Playwright.CLI

playwright install



## How to Run Tests

1. Clone the repository:


git clone https://github.com/akash271291/SnipeItAutomation.git
cd SnipeItAutomation


2. Restore dependencies:


dotnet restore

3. Run the tests:


dotnet test




## Credentials Used (Demo Site)

The demo login credentials used in the script are:

- Username: admin
- Password: password


##  Notes

 -The asset name is dynamically generated with a timestamp to ensure uniqueness during each test run.
 
 -The tests run in **headed mode** (`Headless = false`) so you can observe the browser as it performs the steps.
