import { TIME_PERIOD, months, quarters } from '@/constants/date';
import { generateInitialData } from '@/helpers/generateInitialData';

describe('Helper function generateInitialData', () => {
	const currentDate = new Date();

	it('Should generate initial data for Year', () => {
		const result = generateInitialData(TIME_PERIOD.Year);
		expect(result).toEqual({ year: currentDate.getUTCFullYear() });
	});

	it('Should generate initial data for Month', () => {
		const result = generateInitialData(TIME_PERIOD.Month);
		expect(result).toEqual({
			year: currentDate.getUTCFullYear(),
			month: months[currentDate.getMonth()],
		});
	});

	it('Should generate initial data for Quarter', () => {
		const result = generateInitialData(TIME_PERIOD.Quarter);
		expect(result).toEqual({
			year: currentDate.getUTCFullYear(),
			quarter: quarters[Math.floor(currentDate.getUTCMonth() / 3)],
		});
	});

	it('Should generate default initial data if title is not recognized', () => {
		const result = generateInitialData('InvalidPeriod');
		expect(result).toEqual({ year: currentDate.getUTCFullYear() });
	});
});
