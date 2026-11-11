import { DATE_FORMAT } from '@/constants/date';
import { dateType1Schema } from '@/forms/schemas';
import dayjs from '@/lib/dayjs';

const schema = dateType1Schema();

describe('dateType1Schema', () => {
	test('valid date string passes validation', () => {
		const validDateString = dayjs('31.12.2023', DATE_FORMAT);
		const isValid = schema.isValidSync(validDateString);
		expect(isValid).toEqual(true);
	});

	test('null value fails validation', () => {
		const isValid = schema.isValidSync(null);
		expect(isValid).toBe(false);
	});

	test('invalid date string fails validation', () => {
		const invalidDateString = '32.13.2023';
		const isValid = schema.isValidSync(invalidDateString);
		expect(isValid).toBe(false);
	});

	test('valid Dayjs object passes validation', () => {
		const validDayjs = dayjs('31.12.2023', DATE_FORMAT);
		const isValid = schema.isValidSync(validDayjs);
		expect(isValid).toBe(true);
	});

	test('invalid Dayjs object fails validation', () => {
		const invalidDayjs = dayjs('32.13.2023', DATE_FORMAT, true);

		const isValid = schema.isValidSync(invalidDayjs);
		expect(isValid).toBe(false);
	});

	test('valid string longer than max characters fails validation', () => {
		const tooLongDateString = '31.12.2023extra';
		const isValid = schema.isValidSync(tooLongDateString);
		expect(isValid).toBe(false);
	});

	test('valid string with fewer than max characters passes validation', () => {
		const shortDateString = '31.12';

		const isValid = schema.isValidSync(shortDateString);
		expect(isValid).toBe(false);
	});
});
