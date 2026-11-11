import { convertFileToBase64 } from '@/helpers/convertFileToBase64';

async function fetchAsDataUrl(absoluteUrl: string): Promise<string | null> {
	try {
		const res = await fetch(absoluteUrl);

		if (!res.ok) {
			return null;
		}

		const blob = await res.blob();

		return await convertFileToBase64(blob);
	} catch {
		return null;
	}
}

export async function inlineImageSourcesInHtml(html: string): Promise<string> {
	const doc = new DOMParser().parseFromString(html, 'text/html');
	const images = doc.querySelectorAll('img[src]');
	const cache = new Map<string, string>();

	await Promise.all(
		Array.from(images).map(async (img) => {
			const src = img.getAttribute('src');

			if (!src?.trim() || src.startsWith('data:') || src.startsWith('blob:')) {
				return;
			}

			let absolute: string;
			try {
				absolute = new URL(src, window.location.href).href;
			} catch {
				return;
			}

			let dataUrl = cache.get(absolute);

			if (!dataUrl) {
				const fetched = await fetchAsDataUrl(absolute);

				if (fetched) {
					cache.set(absolute, fetched);
					dataUrl = fetched;
				}
			}

			if (dataUrl) {
				img.setAttribute('src', dataUrl);
			}
		})
	);

	return doc.body.innerHTML;
}
