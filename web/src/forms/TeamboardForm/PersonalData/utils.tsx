import { Theme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { ReactElement } from 'react';

import { popUpBreakpoints, popUpSizes } from '@/constants/popUp';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';
import { getStringOrFallback } from '@/helpers/getStringOrFallback';

import { PERSONAL_DATA_FORM_FIELDS } from './constants';
import { PersonalDataFormValues, UpdatePersonalData } from './types';

const POPUP_DEFAULT_WIDTH = 660;

export const getPopUpStylesConfig = (theme: Theme, windowHeight: number) => ({
	content: getAdaptivePopUpContentStylesConfig({
		theme,
		hasContols: true,
		windowHeight,
	}),
	container: {
		width: POPUP_DEFAULT_WIDTH,

		[theme.breakpoints.down(popUpBreakpoints.tabletL)]: {
			width: popUpSizes.s,
		},
	},
});

export const withTooltip = (element: ReactElement, text: string) => (
	<Tooltip text={text}>
		<span>{element}</span>
	</Tooltip>
);

export const getErrorLabel = (field: keyof typeof PERSONAL_DATA_FORM_FIELDS) =>
	PERSONAL_DATA_FORM_FIELDS[field].label.toLowerCase();

export const getPersonalDataSubmitPayload = (
	values: PersonalDataFormValues,
	userId: number
): UpdatePersonalData => ({
	...values,
	userId,
	corporateEmail: getStringOrFallback(values.corporateEmail),
	personalEmail: getStringOrFallback(values.personalEmail),
});
