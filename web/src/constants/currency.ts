export enum Currency {
	USD = 'USD',
	EUR = 'EUR',
	PLN = 'PLN',
	BYN = 'BYN',
	RUB = 'RUB',
	KZT = 'KZT',
}

export const priorityCurrencyCodes = [
	Currency.USD,
	Currency.EUR,
	Currency.RUB,
	Currency.PLN,
	Currency.BYN,
] as const;

export const isCurrency = (value?: string | null): value is Currency =>
	!!value && (Object.values(Currency) as string[]).includes(value);
