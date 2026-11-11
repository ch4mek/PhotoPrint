interface ExchangeRateCurrency {
	id: string;
	currencyCode: string;
	name: string;
	currencySymbol: string | null;
}

export interface ExchangeRate {
	currencyMultiplier: number;
	rate: number;
	toCurrency: ExchangeRateCurrency;
	fromCurrency: ExchangeRateCurrency;
}

export interface ExchangeRatesResponse {
	id: number;
	exchangeRateDate: string | null;
	exchangeRates: ExchangeRate[];
}
