import { Currency } from '@/constants/currency';

import { currencySymbolCharacters, BYN_CURRENCY_SYMBOL } from './constants';
import { BYNCurrencySymbol, CurrencySymbolText } from './styles';
import { CurrencySymbolProps } from './types';

export function CurrencySymbol({ currency, color }: CurrencySymbolProps) {
	if (currency === Currency.BYN) {
		return (
			<BYNCurrencySymbol {...{ color }}>
				{BYN_CURRENCY_SYMBOL}
			</BYNCurrencySymbol>
		);
	}

	return (
		<CurrencySymbolText {...{ color }}>
			{currencySymbolCharacters[currency]}
		</CurrencySymbolText>
	);
}
