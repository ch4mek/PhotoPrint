import { Theme } from '@emotion/react';
import { lightPalette } from '@mms/mms-ui-library/theme';

import { tagColors } from '@/components/DateTag/constants';
import { TagTypes, TagViews } from '@/components/DateTag/types';
import { DATE_SEPARATOR } from '@/constants/date';

export const convertDateToTagDate = (date: string) => {
	const [dateWithoutTime] = date.split('T');

	return dateWithoutTime.split('-').reverse().join(DATE_SEPARATOR);
};

const getThemePaletteValue = (theme: Theme, color: string) => {
	const { palette } = theme;

	return palette[color as keyof typeof palette] ?? color;
};

export const getTagViews = (theme: Theme): TagViews => {
	const tagTypes = Object.keys(TagTypes) as (keyof typeof TagTypes)[];
	const { palette } = theme;

	return tagTypes.reduce((views, tagType) => {
		const { color, backgroundColor } = tagColors[tagType];

		return {
			...views,
			[tagType]: {
				color: getThemePaletteValue(theme, color ?? lightPalette['text-black']),
				backgroundColor: getThemePaletteValue(theme, backgroundColor),
				border:
					tagType === TagTypes.Future
						? `1px solid ${palette['graphics-input-stroke']}`
						: 'none',
			},
		};
	}, {} as TagViews);
};
