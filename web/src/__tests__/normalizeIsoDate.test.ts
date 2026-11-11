import { normalizeIsoDate } from '@/helpers/normalizeIsoDate';

describe('helper function normalizeIsoDate', () => {
	it.each([
		['2025-07-02T12:34:56.1Z', '2025-07-02T12:34:56.100000000Z'],
		['2025-07-02T12:34:56.12Z', '2025-07-02T12:34:56.120000000Z'],
		['2025-07-02T12:34:56.123Z', '2025-07-02T12:34:56.123000000Z'],
		['2025-07-02T12:34:56.123456Z', '2025-07-02T12:34:56.123456000Z'],
		['2025-07-02T12:34:56.12345678Z', '2025-07-02T12:34:56.123456780Z'],
	])(
		'Should add 0 when the fractional part is shorter than 9 digits',
		(value, expected) => {
			expect(normalizeIsoDate(value)).toBe(expected);
		}
	);

	it('Should return the original string unchanged when the fractional part already has 9 digits', () => {
		const value = '2025-07-02T12:34:56.123456789Z';
		expect(normalizeIsoDate(value)).toBe(value);
	});

	it.each([
		'2025-07-02T12:34:56Z',
		'2025-07-02T12:34:56.123+03:00',
		'2025-07-02T12:34:56.123ZPUPUPU',
	])(
		'Should return the input unchanged when the pattern does not match',
		(value) => {
			expect(normalizeIsoDate(value)).toBe(value);
		}
	);
});
