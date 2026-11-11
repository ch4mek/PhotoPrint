import { RateHistoryGridRow } from '../types';

import dayjs from '@/lib/dayjs';
import { GetRateHistoryResponse } from '@/types/rates';

import { formatRateHistoryDate } from './formatRateHistoryDate';

export const convertRateHistoryItemsIntoRows = (
	items: GetRateHistoryResponse['projectRates'],
	handleEdit?: (itemId?: string | number) => VoidFunction
): RateHistoryGridRow[] =>
	[...items]
		.sort(
			(first, second) =>
				dayjs(second.rateFrom).valueOf() - dayjs(first.rateFrom).valueOf()
		)
		.map(
			({
				id,
				rateFrom,
				hourlyRate,
				currencyId,
				canRateBeManagedByCurrentUser,
			}) => ({
				id,
				date: formatRateHistoryDate(rateFrom),
				effectiveDate: rateFrom,
				rate: hourlyRate,
				currency: currencyId,
				isParent: true,
				handleEditClick: canRateBeManagedByCurrentUser ? handleEdit : undefined,
			})
		);
