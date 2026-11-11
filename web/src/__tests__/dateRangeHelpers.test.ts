import { getStartOfDayUTC, getEndOfDayUTC } from '@/helpers/dateRangeHelpers';

jest.mock('@/helpers/date', () => ({
	...jest.requireActual('@/helpers/date'),
	generateYearsCompanyExistence: jest.fn(() => []),
}));

describe('Helpers getStartOfDayUTC and getEndOfDayUTC', () => {
	const sampleDate = new Date('2025-10-27T14:30:00Z');
	const expectedStart = '2025-10-27T00:00:00Z';
	const expectedEnd = '2025-10-27T23:59:59Z';

	it('Should return UTC start of the day for given date', () => {
		expect(getStartOfDayUTC(sampleDate)).toBe(expectedStart);
	});

	it('Should return UTC end of the day for given date', () => {
		expect(getEndOfDayUTC(sampleDate)).toBe(expectedEnd);
	});

	it('Should correctly handle local time zones', () => {
		const localDate = new Date('2025-10-27T23:30:00+03:00');
		expect(getStartOfDayUTC(localDate)).toBe(expectedStart);
		expect(getEndOfDayUTC(localDate)).toBe(expectedEnd);
	});
});
