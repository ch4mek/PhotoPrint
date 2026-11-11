import { MAXIMUM_HOURS_REQUEST, MIN_HOURS_VALUE } from '../constants';

export const getClampedHoursValue = (value: number) => {
	if (value > MAXIMUM_HOURS_REQUEST) {
		return String(MAXIMUM_HOURS_REQUEST);
	}

	if (value < MIN_HOURS_VALUE) {
		return String(MIN_HOURS_VALUE);
	}

	return String(value);
};
