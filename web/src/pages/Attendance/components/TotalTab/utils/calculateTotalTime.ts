import {
	MINUTES_IN_HOUR,
	TIME_PAD_LENGTH,
	TIME_PAD_CHAR,
	TIME_SEPARATOR,
} from '../constants';
import { AttendanceTotalGridRow } from '../types';

export function calculateTotalTime(rows: AttendanceTotalGridRow[]): string {
	const totalMinutes = rows.reduce((sum, row) => {
		const [hours, minutes] = row.time.split(TIME_SEPARATOR).map(Number);

		return sum + (hours * MINUTES_IN_HOUR + minutes);
	}, 0);

	const totalHours = Math.floor(totalMinutes / MINUTES_IN_HOUR);
	const remainingMinutes = totalMinutes % MINUTES_IN_HOUR;

	return `${totalHours
		.toString()
		.padStart(
			TIME_PAD_LENGTH,
			TIME_PAD_CHAR
		)}${TIME_SEPARATOR}${remainingMinutes
		.toString()
		.padStart(TIME_PAD_LENGTH, TIME_PAD_CHAR)}`;
}
