export type AmountWithCurrencyFields<TRow> = {
	amountField: keyof TRow;
	currencyField: keyof TRow;
};
