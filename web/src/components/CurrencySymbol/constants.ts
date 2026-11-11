import { Currency } from '@/constants/currency';

export const CURRENCY_SYMBOL_FONT_FAMILY = 'CurrencySymbols';

export const BYN_CURRENCY_SYMBOL = '\uE901';

export const currencySymbolCharacters: Record<
	Exclude<Currency, Currency.BYN>,
	string
> = {
	[Currency.USD]: '$',
	[Currency.EUR]: '€',
	[Currency.PLN]: 'zł',
	[Currency.RUB]: '₽',
	[Currency.KZT]: '₸',
};
