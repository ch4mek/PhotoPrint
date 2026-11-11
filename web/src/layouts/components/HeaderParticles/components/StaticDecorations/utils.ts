import { CSSObject, Theme } from '@emotion/react';
import { breakpoints as themeBreakpoints } from '@mms/mms-ui-library/theme';

import { SortDirection } from '@/constants/sort';
import {
	DeviceBreakpointKey,
	ResponsiveValue,
	ThemeBreakpointKey,
} from '@/types/breakpoints';

const deviceBreakpointAliases = {
	desktop: null,
	tablet: 'desktopXS',
	mobile: 'tabletXS',
} as const;

function getMaxWidth(theme: Theme, key: string) {
	if (key === 'desktop') {
		return null;
	}

	if (key in deviceBreakpointAliases) {
		const alias = deviceBreakpointAliases[key as DeviceBreakpointKey];

		return alias ? theme.breakpoints[alias] : null;
	}

	if (key in themeBreakpoints) {
		return themeBreakpoints[key as ThemeBreakpointKey];
	}

	const pixelValue = Number(key);

	return Number.isNaN(pixelValue) ? null : pixelValue;
}

function getEntries<T>(
	theme: Theme,
	value?: ResponsiveValue<T>,
	sortOrder = SortDirection.DESC
) {
	if (!value) {
		return [];
	}

	return Object.entries(value)
		.reduce<Array<{ maxWidth: number | null; value: T }>>(
			(entries, [key, val]) => {
				if (val === undefined) {
					return entries;
				}

				const maxWidth = getMaxWidth(theme, key);

				if (maxWidth === null && key !== 'desktop') {
					return entries;
				}

				entries.push({ maxWidth, value: val as T });

				return entries;
			},
			[]
		)
		.sort((a, b) => {
			const diff = (a.maxWidth ?? Infinity) - (b.maxWidth ?? Infinity);

			return sortOrder === SortDirection.ASC ? diff : -diff;
		});
}

export function getResponsiveStyles(
	theme: Theme,
	value?: ResponsiveValue<CSSObject | string | number>,
	property?: string
) {
	return getEntries(theme, value).reduce<CSSObject>(
		(styles, { maxWidth, value: val }) => {
			const styleProperty = property ? { [property]: val } : (val as CSSObject);

			if (!styleProperty) {
				return styles;
			}

			if (maxWidth === null) {
				Object.assign(styles, styleProperty);
			} else {
				styles[`@media (max-width: ${maxWidth}px)`] = styleProperty;
			}

			return styles;
		},
		{}
	);
}

export function getResponsivePictureSources(
	theme: Theme,
	images: ResponsiveValue<string>
) {
	return getEntries(theme, images, SortDirection.ASC).flatMap(
		({ maxWidth, value }) =>
			maxWidth === null ? [] : [{ maxWidth, srcSet: value }]
	);
}
