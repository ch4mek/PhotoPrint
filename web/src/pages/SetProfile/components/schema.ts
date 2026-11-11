import * as yup from 'yup';

import { FIELD_IS_REQUIRED } from '@/constants/errors';

const schema = {
	firstName: yup.string().nullable(),
	lastName: yup.string().nullable(),
	location: yup
		.string()
		.test('all-spaces-check', FIELD_IS_REQUIRED, (location) => location !== '0')
		.required(FIELD_IS_REQUIRED)
		.nullable(),
	departmentId: yup.string().required(FIELD_IS_REQUIRED).nullable(),
};

export const validationSchemaCEO = yup.object().shape({
	...schema,
	departmentId: yup.string().nullable(),
});

export const validationSchema = yup.object().shape({
	...schema,
	location: yup
		.string()
		.test('all-spaces-check', FIELD_IS_REQUIRED, (location) => location !== '0')
		.required(FIELD_IS_REQUIRED)
		.nullable(),
});
