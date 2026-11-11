export const formatToFixedPrecision = (
	value: number,
	fractionDigits: number = 2
) => (value % 1 === 0 ? value : parseFloat(value.toFixed(fractionDigits)));
