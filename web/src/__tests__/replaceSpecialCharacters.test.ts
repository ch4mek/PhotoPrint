import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';

describe('Helper function replaceSpecialCharacters', () => {
	it('Should add pair for every single quote', () => {
		expect(replaceSpecialCharacters("'")).toBe("''");
		expect(replaceSpecialCharacters("D'Amore Lon")).toBe("D''Amore Lon");
		expect(replaceSpecialCharacters("Ema's book and Ema's pen")).toBe(
			"Ema''s book and Ema''s pen"
		);
		expect(replaceSpecialCharacters("Ema''s book and Ema''s pen")).toBe(
			"Ema''''s book and Ema''''s pen"
		);
	});

	it("Should replace '%' with it's special ASCII code '%25'", () => {
		expect(replaceSpecialCharacters('%')).toBe('%25');
		expect(replaceSpecialCharacters('ab%c')).toBe('ab%25c');
		expect(replaceSpecialCharacters('ab%c d%e')).toBe('ab%25c d%25e');
		expect(replaceSpecialCharacters('ab%%c')).toBe('ab%25%25c');
	});

	it("Should replace '+' with it's special ASCII code '%2B'", () => {
		expect(replaceSpecialCharacters('+')).toBe('%2B');
		expect(replaceSpecialCharacters('ab+c')).toBe('ab%2Bc');
		expect(replaceSpecialCharacters('ab+c d+e')).toBe('ab%2Bc d%2Be');
		expect(replaceSpecialCharacters('ab++c')).toBe('ab%2B%2Bc');
	});

	it("Should replace '/' with it's special ASCII code '%2F'", () => {
		expect(replaceSpecialCharacters('/')).toBe('%2F');
		expect(replaceSpecialCharacters('ab/c')).toBe('ab%2Fc');
		expect(replaceSpecialCharacters('ab/c d/e')).toBe('ab%2Fc d%2Fe');
		expect(replaceSpecialCharacters('ab//c')).toBe('ab%2F%2Fc');
	});

	it("Should replace '?' with it's special ASCII code '%3F'", () => {
		expect(replaceSpecialCharacters('?')).toBe('%3F');
		expect(replaceSpecialCharacters('ab?c')).toBe('ab%3Fc');
		expect(replaceSpecialCharacters('ab?c d?e')).toBe('ab%3Fc d%3Fe');
		expect(replaceSpecialCharacters('ab??c')).toBe('ab%3F%3Fc');
	});

	it("Should replace '#' with it's special ASCII code '%23'", () => {
		expect(replaceSpecialCharacters('#')).toBe('%23');
		expect(replaceSpecialCharacters('ab#c')).toBe('ab%23c');
		expect(replaceSpecialCharacters('ab#c d#e')).toBe('ab%23c d%23e');
		expect(replaceSpecialCharacters('ab##c')).toBe('ab%23%23c');
	});

	it("Should replace '&' with it's special ASCII code '%26'", () => {
		expect(replaceSpecialCharacters('&')).toBe('%26');
		expect(replaceSpecialCharacters('ab&c')).toBe('ab%26c');
		expect(replaceSpecialCharacters('ab&c d&e')).toBe('ab%26c d%26e');
		expect(replaceSpecialCharacters('ab&&c')).toBe('ab%26%26c');
	});

	it('Should replace special characters in different variations', () => {
		expect(replaceSpecialCharacters("'%+/?#&")).toBe("''%25%2B%2F%3F%23%26");
		expect(replaceSpecialCharacters("'#user&+?user?'")).toBe(
			"''%23user%26%2B%3Fuser%3F''"
		);
		expect(replaceSpecialCharacters("%%''??##&&++")).toBe(
			"%25%25''''%3F%3F%23%23%26%26%2B%2B"
		);
	});
});
