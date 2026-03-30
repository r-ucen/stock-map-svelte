<script lang="ts">
	import NavBarRoot from '$lib/components/NavBarRoot.svelte';

	const lastUpdated = new Intl.DateTimeFormat('en-US', {
		month: 'long',
		day: '2-digit',
		year: 'numeric'
	}).format(new Date());
</script>

<NavBarRoot />

<div class="container mx-auto py-10 px-4 max-w-4xl">
	<div class="space-y-8">
		<div>
			<h1 class="scroll-m-20 text-4xl font-semibold tracking-tight lg:text-5xl mb-2">Privacy Policy</h1>
		</div>

		<section>
			<h2 class="scroll-m-20 border-b pb-2 text-3xl font-semibold tracking-tight transition-colors first:mt-0 mb-4">
				Data Collection and Usage
			</h2>
			<p class="leading-7 [&:not(:first-child)]:mt-6">
				We collect only the data necessary for operating and improving this service:
			</p>
			<ul class="my-6 ml-6 list-disc [&>li]:mt-2">
				<li><strong>Account Information:</strong> Email address, username, and authentication credentials (stored securely as a hashed password)</li>
				<li><strong>Technical Data:</strong> Session data for security and functionality purposes</li>
			</ul>
			<p class="leading-7 [&:not(:first-child)]:mt-6">We do not sell, share, or market your data to third parties.</p>
		</section>

		<section>
			<h2 class="scroll-m-20 border-b pb-2 text-3xl font-semibold tracking-tight transition-colors first:mt-0 mb-4">
				Data Security
			</h2>
			<ul class="my-6 ml-6 list-disc [&>li]:mt-2">
				<li>Your password is never stored in plain text. When using local accounts, passwords are stored only as secure, salted hashes following industry-standard practices.</li>
			</ul>
		</section>

		<section>
			<h2 class="scroll-m-20 border-b pb-2 text-3xl font-semibold tracking-tight transition-colors first:mt-0 mb-4">
				Email Communications
			</h2>
			<p class="leading-7 [&:not(:first-child)]:mt-6">We will send you emails only for:</p>
			<ul class="my-6 ml-6 list-disc [&>li]:mt-2">
				<li>Email verification</li>
				<li>Password reset requests</li>
			</ul>
		</section>

		<section>
			<h2 class="scroll-m-20 border-b pb-2 text-3xl font-semibold tracking-tight transition-colors first:mt-0 mb-4">
				Contact info
			</h2>
			<p class="leading-7 [&:not(:first-child)]:mt-6">To contact us, use the following e-mail address: <a href="mailto:stockmapsup@gmail.com" class="font-medium text-primary underline underline-offset-4">stockmapsup@gmail.com</a></p>
		</section>

		<section>
			<h2 class="scroll-m-20 border-b pb-2 text-3xl font-semibold tracking-tight transition-colors first:mt-0 mb-4">
				Policy Updates
			</h2>
			<p class="leading-7 [&:not(:first-child)]:mt-6">Any changes to this Privacy Policy will be posted on this page. We encourage you to review this page periodically. Continued use of the service after updates constitutes acceptance of the revised policy.</p>
		</section>

		<div class="pt-8 text-sm text-muted-foreground">
			<p><em>Last updated: {lastUpdated}</em></p>
		</div>
	</div>
</div>
