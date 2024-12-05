# Подключение работы через прокси

1.Подключить модуль Common.Common

2.В сервисе, где будет происходить вызов.
2.1. Необходим в конструкторе передать:
IHttpClientFactory httpClientFactory
2.2.В конструкторе необходимо:
_httpClient = httpClientFactory.CreateClient(ExtensionСonstants.ProxyClient);

3. В API или JOB в Program.cs или Startup.cs необходимо подключить данный модуль добавить строку:
services.AddProxy();

