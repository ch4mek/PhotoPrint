import { homeTimesOfDayOptions } from '../constants';

const { morning, afternoon, evening } = homeTimesOfDayOptions;

export const getDayTimeTitle = (hour: number) => {
	if (hour >= morning.start && hour < morning.end) {
		return morning;
	}

	if (hour >= afternoon.start && hour < afternoon.end) {
		return afternoon;
	}

	return evening;
};
