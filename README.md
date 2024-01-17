# TestCrypto

The task that I have implemented: I had to create a program that displays various information related to cryptocurrencies. This involves using one (or several) open APIs: • CoinCap: https://docs.coincap.io/ (information about several APIs is available here) The use of ready-made libraries for working with APIs, as well as libraries for working with HTTP (except for the standard HttpClient), is prohibited. The use of template mechanisms to create a project is also prohibited. Libraries for drawing diagrams, working with JSON, Inversion of Control, and MVVM are allowed. The program should support the following functions: • It should be a multi-page program with navigation. • The main page displays the first N currencies by popularity on a certain market (or the top 10 currencies returned by the API). • A page with the ability to view detailed information about a currency: price, volume, price change, on which markets it can be purchased and at what price (the ability to navigate to the currency page on the market is a plus). • The ability to search for a currency by name or code.• A page where you can convert one currency to another (ignoring the method and possible commission). • Light/dark theme support. • Support for multiple localizations.

In this project, I used the following APIs: https://api.coincap.io/v2/rates https://api.coincap.io/v2/assets

I also had to use https://api.coincap.io/v2/candles, but for some reason, when I followed the link, there was simply no data.

So in the application I developed, there are several windows that interact with each other. When you first launch the program, you will encounter the main menu (MainWindowView). The left panel in it is clickable, so you can interact with it. The central panel, I couldn't hide the previous UserControls, and they overlapped each other, so I decided to comment out the transition code.
The next page is CurrencyOverviewView, where you can search for currency, view information, and visit the website. I also added error displays so that the user can better understand the issue.
The next pages, MarketLeadersView and TradingVolumeView, are very similar. They differ only in that on the first one, the top-ranking displays any number of first N places in the ranking. The data is retrieved from the API. On the second page, a candlestick chart is displayed, and the data is taken from JSON, as the API website does not contain information. The user enters a specific quantity in the respective field. Errors are also present for the user to better understand the issue.
The next page is TransactionView. Here, the user can enter data in a text field or select from a combobox. They also enter the amount in the designated field and perform the conversion. It is also worth noting that if the user enters data in both ComboBox and TextBox simultaneously, then we will take the data that was entered last. Errors are also present for the user to better understand the issue.
And the last page is SettingView. Here, the user can set any localization and choose the application theme. After selecting certain settings, it is necessary to press Save to save the settings. Errors are also present for the user to better understand the issue.
There is also a login form, but I believe it is not necessary for this application, so I commented out all related data.
