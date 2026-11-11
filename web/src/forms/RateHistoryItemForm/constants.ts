import { getCurrentMonthDate } from '@/helpers/monthYearDate';

import { RateHistoryItemFormField, RateHistoryItemFormValues } from './types';

export const RATE_HISTORY_ITEM_FORM_POPUP_MAX_WIDTH = 480;
export const RATE_HISTORY_ITEM_ENTITY_NAME = 'Rate details';
export const FORM_NAME = 'Rate history item form';

export const rateHistoryItemFormFields = {
	date: {
		label: 'Date',
		name: RateHistoryItemFormField.Date,
	},
	rate: {
		label: 'Rate',
		name: RateHistoryItemFormField.Rate,
	},
	currency: {
		label: 'Currency',
		name: RateHistoryItemFormField.CurrencyField,
	},
} as const;

export const rateHistoryItemFormInitialValues: RateHistoryItemFormValues = {
	[RateHistoryItemFormField.Date]: getCurrentMonthDate(),
	[RateHistoryItemFormField.Rate]: '',
	[RateHistoryItemFormField.CurrencyField]: null,
};
