import { Dayjs } from '@/lib/dayjs';
import { FormAutoselectOption } from '@/types/form';

export enum RateHistoryItemFormField {
	Date = 'date',
	Rate = 'rate',
	CurrencyField = 'currency',
}

export interface RateHistoryItemFormValues {
	[RateHistoryItemFormField.Date]: Dayjs | null;
	[RateHistoryItemFormField.Rate]: string;
	[RateHistoryItemFormField.CurrencyField]: FormAutoselectOption<string> | null;
}

export interface RateHistoryItemFormItem {
	id?: number;
	date: string;
	rate: number;
	currency: string;
}

export interface RateHistoryItemFormProps {
	onClose: VoidFunction;
	userId: number;
	projectId: number;
	rateItem?: RateHistoryItemFormItem;
}

export type ValidatedRateHistoryItemFormValues = {
	[Key in keyof RateHistoryItemFormValues]: NonNullable<
		RateHistoryItemFormValues[Key]
	>;
};
