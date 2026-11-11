import { CurrencyItemProps } from '../components/CurrencyItem/types';
import { EXCHANGE_RATE_FRACTION_DIGITS } from '../constants';

import { ExchangeRate } from '@/types/exchangeRates';

export const mapExchangeRateToCurrencyItemProps = ({
	fromCurrency,
	toCurrency,
	currencyMultiplier,
	rate,
}: ExchangeRate): CurrencyItemProps => ({
	code: fromCurrency.currencyCode,
	name: fromCurrency.name,
	baseAmount: currencyMultiplier,
	rate: rate.toFixed(EXCHANGE_RATE_FRACTION_DIGITS),
	toCurrencyCode: toCurrency.currencyCode,
});
