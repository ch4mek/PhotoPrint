import { RateHistoryGridRow } from '../components/RateHistoryGrid/types';

import { RateHistoryItemFormItem } from '@/forms/RateHistoryItemForm/types';

export const getRateHistoryItemFormItemFromRow = (
	row: RateHistoryGridRow
): RateHistoryItemFormItem => ({
	id: row.id,
	date: row.effectiveDate,
	rate: row.rate,
	currency: row.currency,
});
