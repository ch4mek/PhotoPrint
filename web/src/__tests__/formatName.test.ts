import { getFullName, formatFullName } from '@/helpers/formatName';

describe('Helper function getFullName', () => {
	it('Should return string with combined firtname and lastname', () => {
		expect(getFullName('Andrew', 'Tate')).toBe('Tate Andrew');
		expect(getFullName('', 'Tate')).toBe('Tate ');
	});
});

describe('formatFullName', () => {
	it('should return full name with last name and first name', () => {
		const lastName = 'Doe';
		const firstName = 'John';
		const fullName = formatFullName(lastName, firstName);
		expect(fullName).toBe('Doe John');
	});

	it('should handle missing last name', () => {
		const lastName = '';
		const firstName = '';
		const fullName = formatFullName(firstName, lastName);
		expect(fullName).toBe('- ');
	});
});
