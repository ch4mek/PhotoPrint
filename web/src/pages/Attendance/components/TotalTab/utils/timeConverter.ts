import {
	MINUTES_IN_HOUR,
	TIME_PAD_CHAR,
	TIME_PAD_LENGTH,
	TIME_SEPARATOR,
} from '../constants';

export const converMinutesToTime = (timeToConvert: number) =>
	`${String(Math.floor(timeToConvert / MINUTES_IN_HOUR)).padStart(
		TIME_PAD_LENGTH,
		TIME_PAD_CHAR
	)}${TIME_SEPARATOR}${String(timeToConvert % MINUTES_IN_HOUR).padStart(
		TIME_PAD_LENGTH,
		TIME_PAD_CHAR
	)}`;

export const convertTimeToMinutes = (time: string): number => {
	const [hours, minutes] = time.split(':').map(Number);

	return hours * MINUTES_IN_HOUR + minutes;
};
