import { convertToChartData } from '@/helpers/convertToChartData';
import { getFullName } from '@/helpers/formatName';
import { PieChartResponseFull } from '@/types/pieChart';

const mockArr: PieChartResponseFull[] = [
	{
		firstName: 'John',
		lastName: 'Doe',
		color: '#FF0000',
		totalTimeInMinutes: 120,
		name: 'John Doe',
	},
	{
		firstName: 'Jane',
		lastName: 'Smith',
		color: '#00FF00',
		totalTimeInMinutes: 90,
		name: '',
	},
	{
		firstName: 'Alice',
		lastName: 'Johnson',
		color: '#0000FF',
		totalTimeInMinutes: 45,
		name: 'Alice J.',
	},
];

describe('Helper function convertToChartData', () => {
	it('Should convert array of PieChartResponseFull to chart data correctly', () => {
		const result = convertToChartData(mockArr);

		expect(result).toHaveLength(mockArr.length);

		expect(result[0].name).toBe(mockArr[0].name);

		expect(result[1].name).toBe(
			getFullName(mockArr[1].firstName, mockArr[1].lastName)
		);

		expect(result[0].value).toBe(mockArr[0].totalTimeInMinutes);
		expect(result[1].value).toBe(mockArr[1].totalTimeInMinutes);

		expect(result[0].color).toBe(mockArr[0].color);
		expect(result[1].color).toBe(mockArr[1].color);
	});

	it('Should handle undefined input and return empty array', () => {
		const result = convertToChartData(undefined);
		expect(result).toEqual([]);
	});

	it('Should use getFullName if name is missing, null or empty string', () => {
		const testArr: PieChartResponseFull[] = [
			{
				firstName: 'Tom',
				lastName: 'Hanks',
				color: '#111111',
				totalTimeInMinutes: 50,
				name: '',
			},
			{
				firstName: 'Emma',
				lastName: 'Stone',
				color: '#222222',
				totalTimeInMinutes: 60,
				name: null,
			},
			{
				firstName: 'Ryan',
				lastName: 'Gosling',
				color: '#333333',
				totalTimeInMinutes: 70,
				name: undefined,
			},
		];

		const result = convertToChartData(testArr);

		expect(result[0].name).toBe(getFullName('Tom', 'Hanks'));
		expect(result[1].name).toBe(getFullName('Emma', 'Stone'));
		expect(result[2].name).toBe(getFullName('Ryan', 'Gosling'));
	});

	it('Should preserve the color field in the output', () => {
		const result = convertToChartData(mockArr);
		expect(result.map((r) => r.color)).toEqual(mockArr.map((r) => r.color));
	});

	it('Should correctly map totalTimeInMinutes to value', () => {
		const result = convertToChartData(mockArr);
		expect(result.map((r) => r.value)).toEqual(
			mockArr.map((r) => r.totalTimeInMinutes)
		);
	});

	it('Should handle empty array input and return empty array', () => {
		const result = convertToChartData([]);
		expect(result).toEqual([]);
	});
});
