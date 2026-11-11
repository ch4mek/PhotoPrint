import { getFullName } from '@/helpers/utils';

describe('getFullName', () => {
	it('should return english name when no russian names provided', () => {
		expect(getFullName('John', 'Doe')).toBe('Doe John');
	});

	it('should return russian name with english in parentheses when both provided', () => {
		expect(getFullName('John', 'Doe', 'Иван', 'Петров')).toBe(
			'Петров Иван (Doe John)'
		);
	});

	it('should return only russian name when only firstNameRu provided', () => {
		expect(getFullName('John', 'Doe', 'Иван', null)).toBe('Иван (Doe John)');
	});

	it('should return only russian name when only lastNameRu provided', () => {
		expect(getFullName('John', 'Doe', null, 'Петров')).toBe(
			'Петров (Doe John)'
		);
	});

	it('should return english name when russian names are empty strings', () => {
		expect(getFullName('John', 'Doe', '', '')).toBe('Doe John');
	});
});
