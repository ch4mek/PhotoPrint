import numeral from 'numeral';

import { DateFormat } from '@/constants/date';
import { NUMERAL_PRICE_FORMAT } from '@/constants/equipment';
import dayjs from '@/lib/dayjs';
import { CurrencyLookupItem } from '@/types/currency';
import { FormAutoselectOption } from '@/types/form';
import {
	CreateProjectRateParams,
	ProjectRateInput,
	UpdateProjectRateParams,
} from '@/types/rates';

import { rateHistoryItemFormInitialValues } from './constants';
import {
	RateHistoryItemFormField,
	RateHistoryItemFormItem,
	RateHistoryItemFormValues,
	ValidatedRateHistoryItemFormValues,
} from './types';

export const mapCurrenciesToFormOptions = (
	currencies: CurrencyLookupItem[]
): FormAutoselectOption<string>[] =>
	currencies.map(({ id, currencyCode }) => ({
		id,
		name: currencyCode,
	}));

export const getInitialFormValues = (
	rateItem: RateHistoryItemFormItem | undefined,
	currencyOptions: FormAutoselectOption<string>[]
): RateHistoryItemFormValues => {
	if (!rateItem) {
		return rateHistoryItemFormInitialValues;
	}

	const { date, rate, currency } = rateItem;

	return {
		[RateHistoryItemFormField.Date]: dayjs(date).startOf('month'),
		[RateHistoryItemFormField.Rate]: numeral(rate).format(NUMERAL_PRICE_FORMAT),
		[RateHistoryItemFormField.CurrencyField]: currencyOptions.find(
			({ id }) => id === currency
		) ?? {
			id: currency,
			name: currency,
		},
	};
};

const mapRateHistoryItemFormValuesToRateInput = ({
	date,
	rate,
	currency,
}: ValidatedRateHistoryItemFormValues): ProjectRateInput => ({
	hourlyRate: numeral(rate).value() ?? 0,
	currencyId: currency.id,
	rateFrom: date.startOf('month').format(DateFormat.YYYY_MM_DD),
});

export const mapRateHistoryItemFormValuesToCreateParams = (
	values: ValidatedRateHistoryItemFormValues,
	userId: number,
	projectId: number
): CreateProjectRateParams => ({
	userId,
	projectId,
	...mapRateHistoryItemFormValuesToRateInput(values),
});

export const mapRateHistoryItemFormValuesToUpdateParams = (
	values: ValidatedRateHistoryItemFormValues,
	id: number
): UpdateProjectRateParams => ({
	id,
	...mapRateHistoryItemFormValuesToRateInput(values),
});
