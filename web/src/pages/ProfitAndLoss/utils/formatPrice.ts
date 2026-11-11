import { PRICE_DECIMAL_DIVIDER, PRICE_DECIMAL_PRECISION } from '../constants';

export const formatPrice = (value?: number | null) => {
	if (value === undefined || value === null) {
		return '';
	}

	return new Intl.NumberFormat('ru-RU', {
		minimumFractionDigits: PRICE_DECIMAL_PRECISION,
		maximumFractionDigits: PRICE_DECIMAL_PRECISION,
	})
		.format(value)
		.replace(',', PRICE_DECIMAL_DIVIDER);
};
