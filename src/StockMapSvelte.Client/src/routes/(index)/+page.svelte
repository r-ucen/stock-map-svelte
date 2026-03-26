<script lang="ts">
 import { Button } from "$lib/components/ui/button";
 import * as Card from "$lib/components/ui/card";
 import { Separator } from "$lib/components/ui/separator";
 import SunIcon from '@lucide/svelte/icons/sun';
 import MoonIcon from '@lucide/svelte/icons/moon';
 import { toggleMode } from "mode-watcher";
 
 let currentYear = new Date().getFullYear();
</script>

<svelte:head>
 <title>Equmap - Visual Stock Market Data</title>
</svelte:head>

<div class="min-h-screen flex flex-col bg-background text-foreground">
 <header class="sticky top-0 z-50 w-full border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
  <div class="container mx-auto px-4 md:px-8 h-16 flex items-center justify-between">
   <a href="/" class="flex items-center gap-2 transition-opacity hover:opacity-80">
    
    <span class="font-bold text-xl tracking-tight">Equmap</span>
   </a>

   <nav class="flex items-center gap-4">
    <Button onclick={toggleMode} variant="outline" size="icon">
     <SunIcon
       class="h-[1.2rem] w-[1.2rem] scale-100 rotate-0 !transition-all dark:scale-0 dark:-rotate-90"
     />
     <MoonIcon
       class="absolute h-[1.2rem] w-[1.2rem] scale-0 rotate-90 !transition-all dark:scale-100 dark:rotate-0"
     />
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
   <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 max-w-[1000px] mx-auto">
    <Card.Root>
     <Card.Header>
      <Card.Title class="text-2xl">Near Real-Time Data</Card.Title>
     </Card.Header>
     <Card.Content>
      <p class="text-muted-foreground">
       Stay updated with live market feeds and metrics changes visualized on our dynamic map.
      </p>
     </Card.Content>
    </Card.Root>

    <Card.Root>
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