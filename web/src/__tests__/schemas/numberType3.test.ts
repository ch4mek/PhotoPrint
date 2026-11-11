import * as yup from 'yup';

import { numberType3Schema } from '@/forms/schemas/types/numberType3';

describe('numberType3Schema', () => {
	const schema = numberType3Schema();

	test('validates a valid number string', async () => {
		await expect(schema.validate('123')).resolves.toBe('123');
	});

	test('validates zero as a valid input', async () => {
		await expect(schema.validate('0')).resolves.toBe('0');
	});

	test('rejects negative numbers', async () => {
		await expect(schema.validate('-123')).rejects.toThrow(yup.ValidationError);
	});

	test('rejects non-numeric strings', async () => {
		await expect(schema.validate('abc')).rejects.toThrow(yup.ValidationError);
	});

	test('rejects empty strings', async () => {
		await expect(schema.validate('')).rejects.toThrow(yup.ValidationError);
	});

	test('rejects strings with invalid characters', async () => {
		await expect(schema.validate('123abc')).rejects.toThrow(
			yup.ValidationError
		);
	});
});
