<script lang="ts">
 import { Button } from "$lib/components/ui/button";
 import { Separator } from "$lib/components/ui/separator";
 import StockTreeMap from "$lib/components/StockTreeMap.svelte";
 import type { TreemapDataDto } from "$lib/Abstractions/Treemap";
 import { MapMetric } from "$lib/Abstractions/IState";
 import MetricSelect from '$lib/components/metric-select.svelte';
 import NavBarRoot from '$lib/components/NavBarRoot.svelte';
 import Legend from '$lib/components/Legend.svelte';

 let currentYear = new Date().getFullYear();

 let currentMapMetric: MapMetric | null = $state(MapMetric.Pe);

 let stockData: TreemapDataDto = {
  "sectors": [
   {
    "sectorName": "Technology",
    "totalMarketCap": 8583127302144,
    "stocks": [
     {
      "tickerSymbol": "AMD",
      "sector": "Technology",
      "fullName": "Advanced Micro Devices, Inc.",
      "marketCap": 911008399360,
      "rectangle": { "x": 0, "y": 0, "width": 0, "height": 0 },
      "regularMarketChangePercent": 7.9802046,
      "regularMarketPrice": 558.695,
      "preMarketChangePercent": 0,
      "preMarketPrice": 0,
      "postMarketChangePercent": 0,
      "postMarketPrice": 0,
      "marketState": "REGULAR",
      "currency": "USD",
      "volume": 10915199,
      "dividendDate": "1970-01-01T00:00:00+00:00",
      "exDividendDate": "1995-04-27T00:00:00+00:00",
      "earningsDate": "2026-08-04T20:00:00+00:00",
      "dividendYield": undefined,
      "beta": 2.469,
      "pe": 186.23167,
      "forwardPe": 42.32775,
      "shortRatio": 1.46,
      "analystRecommendationMean": 1.45098,
      "analystRecommendationKey": "strong_buy",
      "profitMargins": 0.13374,
      "earningsQuarterlyGrowth": 0.951,
      "trailingEps": 3,
      "forwardEps": 13.19926,
      "pegRatio": 1.32,
      "oneYearChange": 2.589137,
      "targetHighPrice": 700,
      "targetLowPrice": 320,
      "targetMeanPrice": 512.2708,
      "targetMedianPrice": 500,
      "totalDebt": 3871000064,
      "freeCashflow": 7173374976,
      "earningsGrowth": 0.912,
      "revenueGrowth": 0.378
     },
     {
      "tickerSymbol": "MSFT",
      "sector": "Technology",
      "fullName": "Microsoft Corporation",
      "marketCap": 2802971312128,
      "rectangle": { "x": 0, "y": 0, "width": 0, "height": 0 },
      "regularMarketChangePercent": -1.5678014,
      "regularMarketPrice": 377.33,
      "preMarketChangePercent": 0,
      "preMarketPrice": 0,
      "postMarketChangePercent": 0,
      "postMarketPrice": 0,
      "marketState": "REGULAR",
      "currency": "USD",
      "volume": 8796072,
      "dividendDate": "2026-09-10T00:00:00+00:00",
      "exDividendDate": "2026-08-20T00:00:00+00:00",
      "earningsDate": "2026-07-29T20:00:00+00:00",
      "dividendYield": 0.0095,
      "beta": 1.13,
      "pe": 22.473494,
      "forwardPe": 19.49387,
      "shortRatio": 2.54,
      "analystRecommendationMean": 1.32143,
      "analystRecommendationKey": "strong_buy",
      "profitMargins": 0.39341998,
      "earningsQuarterlyGrowth": 0.231,
      "trailingEps": 16.79,
      "forwardEps": 19.35634,
      "pegRatio": 1.19,
      "oneYearChange": -0.23558265,
      "targetHighPrice": 870,
      "targetLowPrice": 400,
      "targetMeanPrice": 559.9331,
      "targetMedianPrice": 550,
      "totalDebt": 125431996416,
      "freeCashflow": 37011251200,
      "earningsGrowth": 0.234,
      "revenueGrowth": 0.183
     },
     {
      "tickerSymbol": "NVDA",
      "sector": "Technology",
      "fullName": "NVIDIA Corporation",
      "marketCap": 4869147590656,
      "rectangle": { "x": 0, "y": 0, "width": 0, "height": 0 },
      "regularMarketChangePercent": -1.5138136,
      "regularMarketPrice": 201.03,
      "preMarketChangePercent": 0,
      "preMarketPrice": 0,
      "postMarketChangePercent": 0,
      "postMarketPrice": 0,
      "marketState": "REGULAR",
      "currency": "USD",
      "volume": 45355662,
      "dividendDate": "2026-06-26T00:00:00+00:00",
      "exDividendDate": "2026-06-04T00:00:00+00:00",
      "earningsDate": "2026-08-26T20:00:00+00:00",
      "dividendYield": 0.0049,
      "beta": 2.211,
      "pe": 30.785604,
      "forwardPe": 15.749974,
      "shortRatio": 1.72,
      "analystRecommendationMean": 1.29508,
      "analystRecommendationKey": "strong_buy",
      "profitMargins": 0.62966,
      "earningsQuarterlyGrowth": 2.106,
      "trailingEps": 6.53,
      "forwardEps": 12.76383,
      "pegRatio": 0.6,
      "oneYearChange": 0.24387574,
      "targetHighPrice": 500,
      "targetLowPrice": 180,
      "targetMeanPrice": 301.6207,
      "targetMedianPrice": 294,
      "totalDebt": 12814000128,
      "freeCashflow": 46335873024,
      "earningsGrowth": 2.145,
      "revenueGrowth": 0.852
     }
    ],
    "rectangle": { "x": 0, "y": 0, "width": 0, "height": 0 }
   },
   {
    "sectorName": "Communication Services",
    "totalMarketCap": 4304701489152,
    "stocks": [
     {
      "tickerSymbol": "GOOGL",
      "sector": "Communication Services",
      "fullName": "Alphabet Inc.",
      "marketCap": 4304701489152,
      "rectangle": { "x": 0, "y": 0, "width": 0, "height": 0 },
      "regularMarketChangePercent": -2.5281897,
      "regularMarketPrice": 352.77,
      "preMarketChangePercent": 0,
      "preMarketPrice": 0,
      "postMarketChangePercent": 0,
      "postMarketPrice": 0,
      "marketState": "REGULAR",
      "currency": "USD",
      "volume": 7515872,
      "dividendDate": "2026-06-15T00:00:00+00:00",
      "exDividendDate": "2026-06-08T00:00:00+00:00",
      "earningsDate": "2026-07-22T20:00:00+00:00",
      "dividendYield": 0.0023999999,
      "beta": 1.247,
      "pe": 26.929007,
      "forwardPe": 24.232168,
      "shortRatio": 2.79,
      "analystRecommendationMean": 1.4375,
      "analystRecommendationKey": "strong_buy",
      "profitMargins": 0.37919,
      "earningsQuarterlyGrowth": 0.812,
      "trailingEps": 13.1,
      "forwardEps": 14.55792,
      "pegRatio": 1.44,
      "oneYearChange": 1.0376084,
      "targetHighPrice": 515,
      "targetLowPrice": 340,
      "targetMeanPrice": 432.28735,
      "targetMedianPrice": 430,
      "totalDebt": 95875997696,
      "freeCashflow": 27921750016,
      "earningsGrowth": 0.82,
      "revenueGrowth": 0.218
     }
    ],
    "rectangle": { "x": 0, "y": 0, "width": 0, "height": 0 }
   },
   {
    "sectorName": "Consumer Cyclical",
    "totalMarketCap": 2589774053376,
    "stocks": [
     {
      "tickerSymbol": "AMZN",
      "sector": "Consumer Cyclical",
      "fullName": "Amazon.com, Inc.",
      "marketCap": 2589774053376,
      "rectangle": { "x": 0, "y": 0, "width": 0, "height": 0 },
      "regularMarketChangePercent": -1.1780622,
      "regularMarketPrice": 240.75,
      "preMarketChangePercent": 0,
      "preMarketPrice": 0,
      "postMarketChangePercent": 0,
      "postMarketPrice": 0,
      "marketState": "REGULAR",
      "currency": "USD",
      "volume": 8867340,
      "dividendDate": "1970-01-01T00:00:00+00:00",
      "exDividendDate": undefined,
      "earningsDate": "2026-04-29T20:00:00+00:00",
      "dividendYield": undefined,
      "beta": 1.461,
      "pe": 31.266235,
      "forwardPe": 24.338045,
      "shortRatio": 2.38,
      "analystRecommendationMean": 1.34848,
      "analystRecommendationKey": "strong_buy",
      "profitMargins": 0.12224,
      "earningsQuarterlyGrowth": 0.767,
      "trailingEps": 7.7,
      "forwardEps": 9.89192,
      "pegRatio": 1.83,
      "oneYearChange": 0.09610367,
      "targetHighPrice": 370,
      "targetLowPrice": 207,
      "targetMeanPrice": 312.90628,
      "targetMedianPrice": 315,
      "totalDebt": 235540004864,
      "freeCashflow": 9806624768,
      "earningsGrowth": 0.748,
      "revenueGrowth": 0.166
     }
    ],
    "rectangle": { "x": 0, "y": 0, "width": 0, "height": 0 }
   }
  ],
  "totalMarketCap": 15477602844672
 };
</script>

<svelte:head>
 <title>Equmap - Visual Stock Market Data</title>
 <meta name="description" content="Equmap - Visualize the stock market with interactive maps and near real-time data." />
</svelte:head>

<div class="min-h-screen flex flex-col bg-background text-foreground">
 <NavBarRoot />

 <main class="flex-1">

  <section class="container mx-auto px-4 flex flex-col items-center justify-center text-center min-h-[50vh] py-20 md:py-32">
   <div class="max-w-[800px] flex flex-col items-center">
    <h1 class="text-primary leading-tighter mb-6 text-4xl font-semibold tracking-tight text-balance lg:leading-[1.1] lg:font-semibold xl:text-5xl xl:tracking-tighter max-w-4xl">
     Visualize the Market
    </h1>
    <p class="text-lg sm:text-xl text-muted-foreground mb-10 max-w-[600px] text-balance">
     See market metrics in interactive maps. Track your portfolio's performance with near real-time data visualizations.
    </p>

    <div class="flex flex-col sm:flex-row items-center justify-center w-full sm:w-auto gap-4">

     <div class="flex flex-col items-center gap-4">
      <Button href="/register">
       Get started
      </Button>

      <Button href="/login" variant="link" class="text-muted-foreground">
       Already have an account? Log in
      </Button>
     </div>
    </div>
   </div>
  </section>

  <section class="container mx-auto px-4 py-16 md:py-24">
   <Separator class="mb-16" />

   <div class="max-w-5xl mx-auto">
    <div class="flex flex-col sm:flex-row sm:items-end sm:justify-between gap-6 mb-8">
     <div>
      <h2 class="text-2xl font-semibold tracking-tight">Market Map</h2>
      <p class="text-muted-foreground mt-1 max-w-md">
       Rectangle size is market cap. Color represents the selected metric.
      </p>
     </div>
     <MetricSelect bind:selectedMetric={currentMapMetric} />
    </div>

    <div class="relative min-h-[420px] w-full">
     <div class="absolute inset-0">
      <StockTreeMap treemapData={stockData} selectedMetric={currentMapMetric} />
     </div>
    </div>
    <Legend selectedMetric={currentMapMetric} />
   </div>
  </section>
 </main>

 <footer class="mt-auto">
  <Separator />
  <div class="container mx-auto px-4 py-8 flex flex-col md:flex-row items-center justify-between gap-4">
   <p class="text-sm text-muted-foreground text-center md:text-left">
    &copy; {currentYear} Equmap. All rights reserved.
   </p>

   <ul class="flex gap-4">
    <li>
     <a href="/privacy-policy" class="text-sm text-muted-foreground hover:text-foreground hover:underline transition-colors">
      Privacy Policy
     </a>
    </li>
   </ul>
  </div>
 </footer>
</div>