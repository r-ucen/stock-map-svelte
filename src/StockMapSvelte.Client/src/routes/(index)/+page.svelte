<script lang="ts">
 import { Button } from "$lib/components/ui/button";
 import * as Card from "$lib/components/ui/card";
 import { Separator } from "$lib/components/ui/separator";
 import SunIcon from '@lucide/svelte/icons/sun';
 import MoonIcon from '@lucide/svelte/icons/moon';
 import { toggleMode } from "mode-watcher";

 import StockTreeMap from "$lib/components/StockTreeMap.svelte";
 import type { TreemapDataDto } from "$lib/Abstractions/Treemap";
 import { MapMetric } from "$lib/Abstractions/IState";
 import MetricSelect from '$lib/components/metric-select.svelte';
 import { SvelteDate } from 'svelte/reactivity';

 let currentYear = new Date().getFullYear();

 let currentMapMetric: MapMetric | null = $state(MapMetric.RegularMarketChangePercent);

 const getDynamicDate = (daysOffset: number, timeString: string = "00:00:00") => {
  const d = new SvelteDate();
  d.setDate(d.getDate() + daysOffset);
  const datePart = d.toISOString().split("T")[0];
  return `${datePart}T${timeString}+00:00`;
 };

 let stockData: TreemapDataDto = {
  sectors:[
   {
    sectorName: "Technology",
    totalMarketCap: 3716958388224,
    stocks:[
     {
      tickerSymbol: "AAPL",
      sector: "Technology",
      fullName: "Apple Inc.",
      marketCap: 3716958388224,
      rectangle: { x: 0, y: 0, width: 0, height: 0 },
      regularMarketChangePercent: 0.106881596,
      regularMarketPrice: 252.89,
      preMarketChangePercent: 0,
      preMarketPrice: 0,
      postMarketChangePercent: 0.462651,
      postMarketPrice: 254.06,
      marketState: "POST",
      currency: "USD",
      volume: 41331888,
      dividendDate: getDynamicDate(14),
      exDividendDate: getDynamicDate(-11),
      earningsDate: getDynamicDate(0, "21:00:00"),
      dividendYield: 0.0041,
      beta: 1.116,
      pe: 31.970922,
      forwardPe: 27.148628,
      shortRatio: 3.17
     }
    ],
    rectangle: { x: 0, y: 0, width: 0, height: 0 }
   },
   {
    sectorName: "Communication Services",
    totalMarketCap: 3398289588224,
    stocks:[
     {
      tickerSymbol: "GOOGL",
      sector: "Communication Services",
      fullName: "Alphabet Inc.",
      marketCap: 3398289588224,
      rectangle: { x: 0, y: 0, width: 0, height: 0 },
      regularMarketChangePercent: -3.4406834,
      regularMarketPrice: 280.92,
      preMarketChangePercent: 0,
      preMarketPrice: 0,
      postMarketChangePercent: 0.1352607,
      postMarketPrice: 281.3,
      marketState: "POST",
      currency: "USD",
      volume: 38906505,
      dividendDate: getDynamicDate(30),
      exDividendDate: getDynamicDate(20),
      earningsDate: getDynamicDate(0, "11:00:00"),
      dividendYield: 0.0029,
      beta: 1.112,
      pe: 25.963034,
      forwardPe: 20.930489,
      shortRatio: 2.64
     }
    ],
    rectangle: { x: 0, y: 0, width: 0, height: 0 }
   },
   {
    sectorName: "Consumer Cyclical",
    totalMarketCap: 1396317356032,
    stocks:[
     {
      tickerSymbol: "TSLA",
      sector: "Consumer Cyclical",
      fullName: "Tesla, Inc.",
      marketCap: 1396317356032,
      rectangle: { x: 0, y: 0, width: 0, height: 0 },
      regularMarketChangePercent: 3.5859637,
      regularMarketPrice: 372.11,
      preMarketChangePercent: 0,
      preMarketPrice: 0,
      postMarketChangePercent: 0.5482273,
      postMarketPrice: 374.15,
      marketState: "POST",
      currency: "USD",
      volume: 54835418,
      dividendDate: "1970-01-01T00:00:00+00:00",
      exDividendDate: undefined,
      earningsDate: getDynamicDate(2, "21:00:00"),
      dividendYield: undefined,
      beta: 1.926,
      pe: 344.54626,
      forwardPe: 132.40321,
      shortRatio: 1.05
     }
    ],
    rectangle: { x: 0, y: 0, width: 0, height: 0 }
   },
   {
    sectorName: "Consumer Defensive",
    totalMarketCap: 321477214208,
    stocks:[
     {
      tickerSymbol: "KO",
      sector: "Consumer Defensive",
      fullName: "The Coca-Cola Company",
      marketCap: 321477214208,
      rectangle: { x: 0, y: 0, width: 0, height: 0 },
      regularMarketChangePercent: 0.744183,
      regularMarketPrice: 74.69,
      preMarketChangePercent: 0,
      preMarketPrice: 0,
      postMarketChangePercent: 0.3628278,
      postMarketPrice: 74.961,
      marketState: "POST",
      currency: "USD",
      volume: 8211393,
      dividendDate: getDynamicDate(45),
      exDividendDate: getDynamicDate(25),
      earningsDate: getDynamicDate(-15, "13:30:00"),
      dividendYield: 0.0274,
      beta: 0.332,
      pe: 24.56908,
      forwardPe: 21.576853,
      shortRatio: 3.17
     }
    ],
    rectangle: { x: 0, y: 0, width: 0, height: 0 }
   }
  ],
  totalMarketCap: 8833042546688
 };
</script>

<svelte:head>
 <title>Equmap - Visual Stock Market Data</title>
 <meta name="description" content="Equmap - Visualize the stock market with interactive maps and near real-time data." />
</svelte:head>

<div class="min-h-screen flex flex-col bg-background text-foreground">
 <header class="sticky top-0 z-50 w-full border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
  <div class="container mx-auto px-4 md:px-8 h-16 flex items-center justify-between">
   <a href="/" class="flex items-center gap-2 transition-opacity hover:opacity-80">
    
    <span class="font-bold text-xl tracking-tight">Equmap</span>
   </a>

   <nav class="flex items-center gap-4">
    <Button onclick={toggleMode} variant="ghost" size="icon" class="relative">
     <SunIcon class="h-[1.2rem] w-[1.2rem] transition-all scale-100 rotate-0 dark:-rotate-90 dark:scale-0" />
     <MoonIcon class="absolute h-[1.2rem] w-[1.2rem] transition-all scale-0 rotate-90 dark:rotate-0 dark:scale-100" />
     <span class="sr-only">Toggle theme</span>
    </Button>
   </nav>
  </div>
 </header>

 <main class="flex-1">

  <section class="container mx-auto px-4 flex flex-col items-center justify-center text-center min-h-[50vh] py-20 md:py-32">
   <div class="max-w-[800px] flex flex-col items-center">
    <h1 class="text-4xl sm:text-5xl md:text-6xl font-extrabold tracking-tight mb-6 text-balance">
     Visualize the Market
    </h1>
    <p class="text-lg sm:text-xl text-muted-foreground mb-10 max-w-[600px] text-balance">
     See market metrics in interactive maps. Track your portfolio's performance with near real-time data visualizations.
    </p>

    <div class="flex flex-col sm:flex-row items-center justify-center w-full sm:w-auto gap-4">
     
     <div class="flex flex-col sm:flex-row items-center gap-4 w-full sm:w-auto">
      <Button href="/login" variant="outline" size="lg" class="w-full sm:w-auto px-8">
       Login
      </Button>

      <span class="text-sm font-semibold text-muted-foreground uppercase tracking-widest px-2">
							OR
						</span>

      <Button href="/register" variant="secondary" size="lg" class="w-full sm:w-auto px-8">
       Register
      </Button>
     </div>
    </div>
   </div>
  </section>

  <section class="container mx-auto px-4 py-16 md:py-24">
   <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 max-w-[1200px] mx-auto">

    <Card.Root class="lg:col-span-2 overflow-hidden flex flex-col border-muted bg-card shadow-sm">
     <Card.Header class="bg-muted/30 border-b">
      <Card.Title class="text-2xl">Market Map Overview</Card.Title>
      <Card.Description>
       Select the metric to visualize on the map. The size of each rectangle represents the market capitalization of the stock, while the color intensity indicates the selected metric's value.
       <div class="mt-4">
        <MetricSelect bind:selectedMetric={currentMapMetric} />
       </div>
      </Card.Description>
     </Card.Header>
     <Card.Content class="relative flex-1 min-h-[420px] p-0 bg-background">
      <div class="absolute inset-0 w-full h-full">
       <StockTreeMap treemapData={stockData} selectedMetric={currentMapMetric} />
      </div>
     </Card.Content>
    </Card.Root>

    <Card.Root class="border-muted bg-card shadow-sm">
     <Card.Header>
      <Card.Title class="text-2xl">Near Real-Time Data</Card.Title>
     </Card.Header>
     <Card.Content>
      <p class="text-muted-foreground">
       Stay updated with live market feeds and metrics changes visualized on our dynamic map.
      </p>
     </Card.Content>
    </Card.Root>

    <Card.Root class="border-muted bg-card shadow-sm">
     <Card.Header>
      <Card.Title class="text-2xl">Dedicated portfolios</Card.Title>
     </Card.Header>
     <Card.Content>
      <p class="text-muted-foreground">
       Choose only the equities you want to keep an eye on.
      </p>
     </Card.Content>
    </Card.Root>
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
     <a href="/privacy" class="text-sm text-muted-foreground hover:text-foreground hover:underline transition-colors">
      Privacy Policy
     </a>
    </li>
   </ul>
  </div>
 </footer>
</div>