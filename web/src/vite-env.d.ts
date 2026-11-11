/// <reference types="vite/client" />

interface ImportMetaEnv {
	readonly MODE: string;
	readonly VITE_API_URL: string;
	readonly VITE_ENV: string;
	readonly VITE_PUBLIC_PATH?: string;
	readonly VITE_INSIGHTS_CONNECTION_STRING?: string;
	readonly VITE_VERCEL_ENV?: string;
}

interface ImportMeta {
	readonly env: ImportMetaEnv;
}
