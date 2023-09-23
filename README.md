# BitcoinPriceAggregator
Example for building a functional microservice with two REST endpoints: 
- one for getting a single aggregated price from local storage, or external data sources, if not yet cached
- one for getting cached price points over a specified period of time

Installation instructions:
- download BitcoinPriceAggregatorApi.zip from latest release
- extract in C:/inetpub/wwwroot
- configure http site in IIS pointing to C:/inetpub/wwwroot/BitcoinPriceAggregatorApi
- click "Browse ..." option in right pane of IIS
- in newly opened browser, append "swagger/index.html" to the url - it will provide details on available calls

or

- checkout solution on local environment
- open solution in Visual Studio and run it 