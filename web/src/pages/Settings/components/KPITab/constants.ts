import { Theme } from '@emotion/react';

import {
	KPIColorFormSingleValue,
	KPIColorsFormFields,
	KPIColorsFormValues,
} from './types';

export const KPI_COLOR_KEYS: Record<
	KPIColorsFormFields,
	keyof Theme['palette']
> = {
	minColor: 'feedback-danger',
	middleColor: 'text-accent',
	maxColor: 'feedback-success',
};

const emptyKPIColorValue: KPIColorFormSingleValue = {
	id: 0,
	name: '',
	rightIntervalBorder: '',
};

export const emptyKPIColorsFormValues: KPIColorsFormValues = {
	minColor: emptyKPIColorValue,
	middleColor: emptyKPIColorValue,
	maxColor: emptyKPIColorValue,
};

export const PERCENT_SYMBOL = '%';
export const SAVE_KPI_COLORS_BUTTON_LABEL = 'Save';
export const KPI_VALUES_UPDATED_MESSAGE =
	'KPI value intervals have been successfully updated';
export const FORM_NAME = 'Settings KPI form';
