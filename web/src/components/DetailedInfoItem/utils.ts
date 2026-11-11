import { minutesToHours } from '@/helpers/minutesToHours';

export const getDetailedInfoItemTime = (valueTime: number) => {
	const prefixes = ['h', 'm'];

	if (valueTime) {
		return minutesToHours(valueTime)
			.split(':')
			.map((time, timeIndex) => `${time} ${prefixes[timeIndex]}`)
			.join(' ');
	}

	return '';
};
