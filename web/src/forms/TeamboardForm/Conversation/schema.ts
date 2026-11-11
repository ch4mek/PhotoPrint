import * as yup from 'yup';

import { DATE_FORMAT, MIN_FULL_DATE } from '@/constants/date';
import {
	FIELD_IS_REQUIRED,
	getPleaseEnterAValidFieldErrorMessage,
} from '@/constants/errors';
import { dateType1Schema, longTextType1Schema } from '@/forms/schemas';
import { linkSchema } from '@/forms/schemas/types/linkType';
import { CONVERSATION_TEXT_MAX_LENGTH } from '@/forms/TeamboardForm/Conversation/constants';
import {
	ConversationFormField,
	ConversationType,
} from '@/forms/TeamboardForm/Conversation/types';
import dayjs, { Dayjs } from '@/lib/dayjs';

const isPerformanceReview = (val: ConversationType) =>
	val === ConversationType.PerformanceReview;

const isConversationType = (val: ConversationType) =>
	val === ConversationType.Conversation;

const isConversationOrHRConversationType = (val: ConversationType) =>
	val === ConversationType.Conversation ||
	val === ConversationType.HRConversation;

const conversationRequired = (
	type: ConversationType | ((val: ConversationType) => boolean),
	schemaFn: yup.BaseSchema
) =>
	schemaFn.when(`${ConversationFormField.TYPE}.id`, {
		is: type,
		then: (schema: yup.BaseSchema) => schema.required(FIELD_IS_REQUIRED),
	});

const dateRangeValidationSchema = (
	fieldName: string,
	isMandatory = true,
	shouldCheckValidation = false,
	minDate: Dayjs = MIN_FULL_DATE
) => {
	const dateFieldErrorMessage =
		getPleaseEnterAValidFieldErrorMessage(fieldName);

	const schema = dateType1Schema(isMandatory).when(
		`${ConversationFormField.TYPE}.id`,
		{
			is: (type: ConversationType) =>
				isPerformanceReview(type) || shouldCheckValidation,
			then: (schemas) =>
				schemas.test('is-in-range', dateFieldErrorMessage, (value) => {
					const date = dayjs(value, DATE_FORMAT);

					return date.isValid() && date.isSameOrAfter(minDate);
				}),
			otherwise: (schemas) => schemas.notRequired(),
		}
	);

	return schema;
};

const conversationLongTextSchema = (fieldName: string) =>
	longTextType1Schema(
		fieldName,
		false,
		2,
		true,
		false,
		CONVERSATION_TEXT_MAX_LENGTH
	);

export const conversationValidationSchema = yup.object({
	date: dateRangeValidationSchema('date', true, true),
	conversationType: yup.mixed().required(FIELD_IS_REQUIRED),
	performanceReviewDate: dateRangeValidationSchema('date', true),
	conversationResult: conversationRequired(
		isConversationOrHRConversationType,
		yup.mixed()
	).nullable(),
	passed: conversationRequired(isPerformanceReview, yup.mixed()).nullable(),
	theses: conversationRequired(
		isConversationType,
		conversationLongTextSchema('theses')
	),
	employeeTheses: conversationRequired(
		isConversationType,
		conversationLongTextSchema('employee’s theses')
	),
	agreements: conversationRequired(
		isConversationType,
		conversationLongTextSchema('agreements')
	),
	documentLinks: yup.array().of(linkSchema()),
});
