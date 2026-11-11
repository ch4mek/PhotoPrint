import { PRICE_DECIMAL_PRECISION } from '../constants';

export const formatHourlyRate = (rate: number) =>
	rate.toFixed(PRICE_DECIMAL_PRECISION);
