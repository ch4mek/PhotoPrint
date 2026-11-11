import { getFormattedTime } from '@/helpers/bookingFormatters';
import {
	generateFromTimeOptions,
	generateToTimeOptions,
	getDayEndHour,
	getDayStartHour,
} from '@/helpers/bookingTime';

describe('generateFromTimeOptions helper tests', () => {
	it('should return correct options for working hours', () => {
		const isWorkingHours = true;
		const options = generateFromTimeOptions(isWorkingHours);
		const firstOption = getFormattedTime(getDayStartHour(isWorkingHours), 0);
		const lastOption = getFormattedTime(getDayEndHour(isWorkingHours), 45);

		expect(options.at(0)).toEqual({
			id: firstOption,
			name: firstOption,
		});
		expect(options.at(-1)).toEqual({
			id: lastOption,
			name: lastOption,
		});
	});

	it('should return correct options for non working hours', () => {
		const isWorkingHours = false;
		const options = generateFromTimeOptions(isWorkingHours);
		const firstOption = getFormattedTime(getDayStartHour(isWorkingHours), 0);
		const lastOption = getFormattedTime(getDayEndHour(isWorkingHours), 45);

		expect(options.at(0)).toEqual({
			id: firstOption,
			name: firstOption,
		});
		expect(options.at(-1)).toEqual({
			id: lastOption,
			name: lastOption,
		});
	});
});

describe('generateToTimeOptions helper tests', () => {
	it('should return correct options for working hours', () => {
		const isWorkingHours = true;
		const options = generateToTimeOptions(isWorkingHours);
		const firstOption = getFormattedTime(getDayStartHour(isWorkingHours), 15);
		const lastOption = getFormattedTime(
			(getDayEndHour(isWorkingHours) + 1) % 24,
			0
		);

		expect(options.at(0)).toEqual({
			id: firstOption,
			name: firstOption,
		});
		expect(options.at(-1)).toEqual({
			id: lastOption,
			name: lastOption,
		});
	});

	it('should return correct options for non working hours', () => {
		const isWorkingHours = false;
		const options = generateToTimeOptions(isWorkingHours);
		const firstOption = getFormattedTime(getDayStartHour(isWorkingHours), 15);
		const lastOption = getFormattedTime(
			(getDayEndHour(isWorkingHours) + 1) % 24,
			0
		);

		expect(options.at(0)).toEqual({
			id: firstOption,
			name: firstOption,
		});
		expect(options.at(-1)).toEqual({
			id: lastOption,
			name: lastOption,
		});
	});
});
