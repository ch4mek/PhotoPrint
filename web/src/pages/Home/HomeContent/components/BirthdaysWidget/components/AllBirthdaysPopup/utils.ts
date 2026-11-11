import { TFunction } from 'i18next';

import { scrollbarWidths } from '@/constants/commonStyles';
import {
	popUpSizes,
	popUpPaddings,
	POPUP_HEADER_HEIGHT,
} from '@/constants/popUp';
import { defaultSearchOptions } from '@/constants/search';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';
import { AllBirthdaysResponseItem } from '@/types/home';

import { GetPopUpStylesConfigProps } from './types';

export const getPopUpStylesConfig = ({
	theme,
	windowHeight,
	windowWidth,
}: GetPopUpStylesConfigProps) => ({
	container: {
		width: popUpSizes.m,
		height: '100%',

		'& > div': {
			height: `calc(100% - ${POPUP_HEADER_HEIGHT}px)`,
		},
	},
	content: {
		...getAdaptivePopUpContentStylesConfig({
			theme,
			windowHeight,
			windowWidth,
		}),

		padding: popUpPaddings.default,
		paddingRight: `calc(${popUpPaddings.default} - ${scrollbarWidths.s}px)`,
		scrollbarGutter: 'stable',
		gap: theme.spaces.l,
	},
});

export const countResults = (
	birthdays: AllBirthdaysResponseItem[] | undefined,
	searchValue: string
) => {
	const trimmed = searchValue.trim();

	if (
		!birthdays ||
		!trimmed ||
		trimmed.length > defaultSearchOptions.maxCharacters
	) {
		return null;
	}

	return birthdays.reduce(
		(result, { birthdaysCount }) => result + birthdaysCount,
		0
	);
};
export const getResultsString = (
	amount: number,
	t: TFunction<'home', 'widgets.birthdays'>
) => t('allBirthdaysPopup.searchResults', { count: amount });
