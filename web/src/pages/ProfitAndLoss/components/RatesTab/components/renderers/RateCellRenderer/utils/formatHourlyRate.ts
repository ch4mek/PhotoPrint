import { HOURLY_RATE_FRACTION_DIGITS } from '../constants';

export const formatHourlyRate = (rate: number) =>
	rate.toFixed(HOURLY_RATE_FRACTION_DIGITS);
