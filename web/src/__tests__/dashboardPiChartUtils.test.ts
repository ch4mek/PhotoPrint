import { getDuration, getValues } from '@/helpers/homePieChartUtils';
import { ManagersPieChartResponse } from '@/types/pieChart';

const testData: ManagersPieChartResponse[] = [
	{ firstName: 'John', lastName: 'Doe', color: 'blue', trackedTime: 150 },
	{ firstName: 'Jane', lastName: 'Smith', color: 'green', trackedTime: 180 },
	{ firstName: 'Alice', lastName: 'Johnson', color: 'red', trackedTime: 120 },
];

describe('Helper function getDuration', () => {
	it('Should return formatted duration in HH:MM format', () => {
		const result = getDuration(testData);
		expect(result).toBe('07:30');
	});

	it('Should return empty string when input array is empty', () => {
		const result = getDuration([]);
		expect(result).toBe('');
	});

	it('Should return empty string when input array is undefined', () => {
		const result = getDuration(undefined);
		expect(result).toBe('');
	});
});

describe('Helper function getValues', () => {
	it('Should return array of corect objects', () => {
		const result = getValues(testData);
		expect(result[0]).toEqual({
			name: 'Doe John',
			firstName: 'John',
			lastName: 'Doe',
			totalTimeInMinutes: 150,
			value: 150,
			color: 'blue',
		});
		expect(result[1]).toEqual({
			name: 'Smith Jane',
			firstName: 'Jane',
			lastName: 'Smith',
			value: 180,
			totalTimeInMinutes: 180,
			color: 'green',
		});
		expect(result[2]).toEqual({
			name: 'Johnson Alice',
			firstName: 'Alice',
			lastName: 'Johnson',
			value: 120,
			totalTimeInMinutes: 120,
			color: 'red',
		});
	});

	it('Should return empty array if input is undefined', () => {
		expect(getValues(undefined)).toEqual([]);
	});
});
