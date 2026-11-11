const MINUTES_IN_HOUR = 60;
const MAX_SINGLE_SIGN_HOUR = 10;

export function minutesToHours(
	minutes: number | undefined,
	isFullHours: boolean | undefined = false
) {
	if (!minutes) {
		return '';
	}

	const hours = Math.floor(minutes / MINUTES_IN_HOUR);
	const remainderHours = minutes % MINUTES_IN_HOUR;

	const formatedHours =
		hours < MAX_SINGLE_SIGN_HOUR && !isFullHours ? `0${hours}` : hours;

	return `${formatedHours}:${
		remainderHours < MAX_SINGLE_SIGN_HOUR
			? `0${Math.floor(remainderHours) || '0'}`
			: remainderHours
	}`;
}

export function hoursToMinutes(time: string) {
	if (!time) {
		return 0;
	}
	const [hours, minutes] = time.split(':');

	const convertedMinutes =
		minutes && minutes.length === 1
			? Number(minutes) * MAX_SINGLE_SIGN_HOUR
			: Number(minutes);

	return Number(hours) * MINUTES_IN_HOUR + (convertedMinutes || 0);
}
