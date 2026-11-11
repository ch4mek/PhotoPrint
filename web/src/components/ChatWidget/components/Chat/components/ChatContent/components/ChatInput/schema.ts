import * as yup from 'yup';

import { longTextType1Schema } from '@/forms/schemas';

import { MESSAGE_FIELD } from './constants';

const MIN_COUNT_OF_CHARACTERS = 1;

export const validationSchema = yup.object({
	[MESSAGE_FIELD]: longTextType1Schema(
		'message',
		undefined,
		MIN_COUNT_OF_CHARACTERS,
		false
	),
});
