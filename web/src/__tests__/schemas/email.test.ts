import { emailSchema } from '@/forms/schemas';

const MIN_CHARACTERS = 25;
const MAX_CHARACTERS = 550;

const OVERFLOW_VALUE = 'f'.repeat(MAX_CHARACTERS + 1);
const SHORT_VALUE = 'f'.repeat(MIN_CHARACTERS - 1);

const VALID_DOMAIN = 'modsen-software.com';
const VALID_VALUE = `example@${VALID_DOMAIN}`;

const DIGITS = '1234567890';

describe('Schema emailSchema', () => {
	it('Should return true for valid email value', () => {
		const validationValue = emailSchema().validateSync(VALID_VALUE);
		expect(validationValue).toEqual(VALID_VALUE);
	});

	it('Should return true if email has wrong domain', () => {
		const validationValue = emailSchema().isValidSync('example@gmail.com');
		expect(validationValue).toEqual(false);
	});

	it('Should return false if email is too short', () => {
		const validationValue = emailSchema().isValidSync(SHORT_VALUE);
		expect(validationValue).toEqual(false);
	});

	it('Should return false if email is too long', () => {
		const validationValue = emailSchema().isValidSync(OVERFLOW_VALUE);
		expect(validationValue).toEqual(false);
	});

	it('Should return false if email has digits', () => {
		DIGITS.split('').forEach((char) => {
			const value = `text${char}`;
			expect(emailSchema().isValidSync(value)).toEqual(false);
		});
	});
});
