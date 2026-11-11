import { monthsToYearsAndMonthsObject } from '@/helpers/translateDateUtils';

describe('Helper function translateDateUtils', () => {
	it('should return 0 years and 0 months for 0 input', () => {
		const result = monthsToYearsAndMonthsObject(0);

		expect(result).toEqual({ years: 0, months: 0 });
	});

	it('should return correct years and months for exact year', () => {
		const result = monthsToYearsAndMonthsObject(12);

		expect(result).toEqual({ years: 1, months: 0 });
	});

	it('should return correct years and months for partial year', () => {
		const result = monthsToYearsAndMonthsObject(15);

		expect(result).toEqual({ years: 1, months: 3 });
	});

	it('should return only months when less than a year', () => {
		const result = monthsToYearsAndMonthsObject(8);

		expect(result).toEqual({ years: 0, months: 8 });
	});

	it('should handle multiple years correctly', () => {
		const result = monthsToYearsAndMonthsObject(30);

		expect(result).toEqual({ years: 2, months: 6 });
	});

	it('should handle edge case of 11 months', () => {
		const result = monthsToYearsAndMonthsObject(11);

		expect(result).toEqual({ years: 0, months: 11 });
	});

	it('should handle large numbers', () => {
		const result = monthsToYearsAndMonthsObject(100);

		expect(result).toEqual({ years: 8, months: 4 });
	});
});
