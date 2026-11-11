import * as yup from 'yup';

import {
	FIELD_IS_REQUIRED,
	getInvalidFieldErrorMessage,
	getMaxLengthErrorMessage,
	getMinLengthErrorMessage,
	getPleaseEnterAValidFieldErrorMessage,
} from '@/constants/errors';
import { checkIfNoLettersOrNumbers } from '@/forms/utils';
import { getTrimedString } from '@/helpers/getTrimedString';
import { trimmedEmptyValidityTest } from '@/helpers/trimmedEmptyValidityTest';

const DEFAULT_MIN_CHARACTERS = 2;
const DEFAULT_MAX_CHARACTERS = 250;

export const textType4Schema = (
	fieldName: string,
	isMandatory: boolean = true,
	minCharacters = DEFAULT_MIN_CHARACTERS,
	maxCharacters = DEFAULT_MAX_CHARACTERS,
	isShortMessage = false
) => {
	const validityMessage = isShortMessage
		? getInvalidFieldErrorMessage(fieldName)
		: getPleaseEnterAValidFieldErrorMessage(fieldName);

	const afterTransform = (schema: yup.StringSchema) => {
		const withTrimAndEmptyValidityCheck = schema
			.nullable()
			.transform((value) => (value ? getTrimedString(value) : value))
			.test(
				'trimmed-empty-validity',
				validityMessage,
				trimmedEmptyValidityTest
			);

		const withRequiredCheck = isMandatory
			? withTrimAndEmptyValidityCheck.required(FIELD_IS_REQUIRED)
			: withTrimAndEmptyValidityCheck;

		return withRequiredCheck
			.test('only-symbols-check', validityMessage, (value) => {
				if (!value) {
					return true;
				}

				if (checkIfNoLettersOrNumbers(value)) {
					return false;
				}

				return true;
			})
			.min(minCharacters, getMinLengthErrorMessage(minCharacters))
			.max(maxCharacters, getMaxLengthErrorMessage(maxCharacters));
	};

	return afterTransform(yup.string());
};
