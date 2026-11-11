import { RenderCellProps } from 'react-data-grid';

import {
	RateCellContent,
	RateCellValue,
	RateCurrencyPart,
	RateValuePart,
} from '../RateCellRenderer/styles';

import { CurrencySymbol } from '@/components/CurrencySymbol';
import { Currency, isCurrency } from '@/constants/currency';
import { formatHourlyRate } from '@/pages/ProfitAndLoss/utils/formatHourlyRate';

import { AmountWithCurrencyFields } from './types';

export function createAmountWithCurrencyCellRenderer<
	TRow extends {
		disabled?: boolean;
		isParent: boolean;
	}
>({ amountField, currencyField }: AmountWithCurrencyFields<TRow>) {
	return function AmountWithCurrencyCellRenderer({
		row,
	}: RenderCellProps<TRow>) {
		const amount = row[amountField] as number | null;
		const currency = row[currencyField] as string | null;
		const { disabled, isParent } = row;
		const isChildRow = !isParent;
		const isByn = currency === Currency.BYN;

		if (amount === null) {
			return <RateCellContent {...{ disabled }}>-</RateCellContent>;
		}

		if (currency === null) {
			return (
				<RateCellContent {...{ disabled }}>
					<RateValuePart {...{ isChildRow }}>
						{formatHourlyRate(amount)}
					</RateValuePart>
				</RateCellContent>
			);
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
						{formatHourlyRate(amount)}
					</RateValuePart>
				</RateCellValue>
			</RateCellContent>
		);
	};
}
