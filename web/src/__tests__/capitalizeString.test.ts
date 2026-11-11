import { capitalizeString } from '@/helpers/capitalizeString';

describe('Helper function capitalizeString', () => {
	it('Should retun string with capitalized first letter', () => {
		expect(capitalizeString('test')).toBe('Test');
	});

	it('Should returt empty string is value is empty string', () => {
		expect(capitalizeString('')).toBe('');
	});

	it('Should return trimmed string with capitalized first letter', () => {
		expect(capitalizeString('       test            ')).toBe('Test');
	});
});
