import * as yup from 'yup'; // Import yup for schema validation

import { removeExtraSpaces } from '@/helpers/removeExtraSpaces';

const schema = yup.object({
	name: yup.string().required(),
	description: yup.string(),
	age: yup.number().integer().positive(),
});

const values = {
	name: '   John   ',
	description: '   A   description   with   extra   spaces   ',
	age: 25,
};

describe('Helpers function removeExtraSpaces', () => {
	const result = removeExtraSpaces(values, schema);

	it('Should remove extra spaces in string values', () => {
		expect(result).toEqual({
			name: 'John',
			description: 'A description with extra spaces',
			age: 25,
		});
	});
});
