import { useEffect, useRef } from 'react';
import { useLocation } from 'react-router-dom';

import {
	FAVICON_CONFIGS,
	ROUTE_TO_ICON_FOLDER_MAP,
	DEFAULT_ICON_FOLDER,
} from '@/constants/favicon';

export function useDynamicFavicon() {
	const { pathname } = useLocation();
	const basePathRef = useRef<string | null>(null);

	useEffect(() => {
		if (typeof document === 'undefined') {
			return;
		}

		if (basePathRef.current === null) {
			const getLinkHref = (selector: string) =>
				document.head
					.querySelector<HTMLLinkElement>(selector)
					?.getAttribute('href');

			const originalHref =
				getLinkHref('link[rel="manifest"]') ||
				getLinkHref('link[rel="apple-touch-icon"][sizes="180x180"]') ||
				getLinkHref('link[rel="icon"]') ||
				'/';

			const baseUrl = new URL(originalHref, document.baseURI);
			basePathRef.current = baseUrl.pathname.replace(/\/[^/]*$/, '');
		}

		const basePath = basePathRef.current;
		const routes = Object.keys(ROUTE_TO_ICON_FOLDER_MAP).sort(
			(a, b) => b.length - a.length
		);
		const matchingRoute = routes.find((route) => pathname.startsWith(route));
		const iconFolder = matchingRoute
			? ROUTE_TO_ICON_FOLDER_MAP[matchingRoute]
			: DEFAULT_ICON_FOLDER;

		FAVICON_CONFIGS.forEach((config) => {
			const selector = `link[rel="${config.rel}"]${
				config.sizes ? `[sizes="${config.sizes}"]` : ''
			}${config.type ? `[type="${config.type}"]` : ''}`;

			let linkElement = document.head.querySelector<HTMLLinkElement>(selector);

			if (!linkElement) {
				linkElement = document.createElement('link');
				document.head.appendChild(linkElement);
			}

			const iconPath =
				`${basePath}/icons/${iconFolder}/${config.filename}`.replace(
					/\/+/g,
					'/'
				);

			linkElement.setAttribute('rel', config.rel);
			linkElement.setAttribute('href', iconPath);

			if (config.type) {
				linkElement.setAttribute('type', config.type);
			} else {
				linkElement.removeAttribute('type');
			}

			if (config.sizes) {
				linkElement.setAttribute('sizes', config.sizes);
			} else {
				linkElement.removeAttribute('sizes');
			}
		});
	}, [pathname]);
}
