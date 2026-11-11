import * as yup from 'yup';

import {
	FIELD_IS_REQUIRED,
	getInvalidFormatFieldMessage,
	getMaxLengthErrorMessage,
} from '@/constants/errors';

const MAX_CHARACTERS = 550;
const LINK_FILE_TITLE = 'link';

export const linkSchema = (isMandatory = false) => {
	const schema = yup
		.string()
		.nullable()
		.transform((value) =>
			!isMandatory && (value === '' || value === null)
				? null
				: String(value).trim()
		)
		.required(FIELD_IS_REQUIRED)
		.max(MAX_CHARACTERS, getMaxLengthErrorMessage(MAX_CHARACTERS))
		.url(getInvalidFormatFieldMessage(LINK_FILE_TITLE));

	if (!isMandatory) {
		return schema.notRequired();
	}

	return schema;
};
