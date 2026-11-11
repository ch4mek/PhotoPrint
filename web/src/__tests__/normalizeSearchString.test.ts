import { normalizeSearchString } from '@/helpers/normalizeSearchString';

describe('Helper function normalizeSearchString', () => {
	it("Should add correct add \\ before \\'", () => {
		expect(normalizeSearchString("Ema's book")).toBe("Ema\\'s book");
		expect(normalizeSearchString("Ema''s book")).toBe("Ema\\'\\'s book");
	});

	it('Should add correct add \\ before \\&', () => {
		expect(normalizeSearchString('Ema&s book')).toBe('Ema\\&s book');
		expect(normalizeSearchString('Ema&&s book')).toBe('Ema\\&\\&s book');
	});

	it("Shoudn\\'t affect string with out\\& and \\'", () => {
		expect(normalizeSearchString('test')).toBe('test');
		expect(normalizeSearchString('test test test')).toBe('test test test');
	});
});
