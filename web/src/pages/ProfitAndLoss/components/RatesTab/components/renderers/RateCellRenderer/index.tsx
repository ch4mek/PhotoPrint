import { RenderCellProps } from 'react-data-grid';

import { CurrencySymbol } from '@/components/CurrencySymbol';
import { Currency, isCurrency } from '@/constants/currency';

import {
	RateCellContent,
	RateCellValue,
	RateCurrencyPart,
	RateValuePart,
} from './styles';
import { RateCellRow } from './types';
import { formatHourlyRate } from './utils/formatHourlyRate';

export function RateCellRenderer<T extends RateCellRow>({
	row,
}: RenderCellProps<T>) {
	const { rate, currency, disabled, isParent } = row;
	const isChildRow = !isParent;
	const isByn = currency === Currency.BYN;

	if (rate === null || currency === null) {
		return <RateCellContent {...{ disabled }}>-</RateCellContent>;
	}

	return (
		<RateCellContent {...{ disabled }}>
			<RateCellValue {...{ isChildRow }}>
				<RateCurrencyPart {...{ isChildRow, isByn }}>
					{isCurrency(currency) ? (
						<CurrencySymbol {...{ currency }} />
					) : (
						currency
					)}
				</RateCurrencyPart>
				<RateValuePart {...{ isChildRow }}>
					{formatHourlyRate(rate)}
				</RateValuePart>
			</RateCellValue>
		</RateCellContent>
	);
}
