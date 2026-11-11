import * as yup from 'yup';

import {
	FIELD_IS_REQUIRED,
	getPleaseEnterAValidFieldErrorMessage,
} from '@/constants/errors';
import {
	PERCENT_RANGE_INVALID_MESSAGE,
	validationFieldsTitles,
} from '@/forms/Discount/DiscountForm/constants';
import { notRequiredTextType4Schema } from '@/forms/Discount/schema';
import { numberType1Schema, textType4Schema } from '@/forms/schemas';
import { ddlType1Schema } from '@/forms/schemas/types/ddlType1';
import { linkSchema } from '@/forms/schemas/types/linkType';

const MAX_PERCENT_NUMBER = 99;

const percentSchema = (label: string) =>
	numberType1Schema(label).test(
		'not-starts-with-zero',
		getPleaseEnterAValidFieldErrorMessage(label),
		(value) => (value ? !value.startsWith('0') : true)
	);

const validatePercentRange = (percent: number, percentTo: number) => {
	if (
		percent &&
		percentTo &&
		percent <= MAX_PERCENT_NUMBER &&
		percentTo <= MAX_PERCENT_NUMBER
	) {
		return percent < percentTo;
	}

	return true;
};

export const validationSchema = yup.object({
	discountCategory: ddlType1Schema(),
	name: textType4Schema(validationFieldsTitles.name),
	discountPercent: percentSchema(validationFieldsTitles.discountPercent)
		.test(
			'less-than-to',
			PERCENT_RANGE_INVALID_MESSAGE,
			(percent, testContext) =>
				validatePercentRange(
					Number(percent),
					Number(testContext.parent.discountPercentTo)
				)
		)
		.required(FIELD_IS_REQUIRED),
	discountPercentTo: percentSchema(validationFieldsTitles.discountPercentTo)
		.test(
			'more-than-percent',
			PERCENT_RANGE_INVALID_MESSAGE,
			(percentTo, testContext) =>
				validatePercentRange(
					Number(testContext.parent.discountPercent),
					Number(percentTo)
				)
		)
		.optional(),
	companyType: notRequiredTextType4Schema(validationFieldsTitles.companyType),
	website: linkSchema(),
	phoneNumber: notRequiredTextType4Schema(validationFieldsTitles.phoneNumber),
	location: notRequiredTextType4Schema(validationFieldsTitles.location),
	tags: yup.array(),
	cities: yup.array().min(1, FIELD_IS_REQUIRED).required(FIELD_IS_REQUIRED),
	mediaLinks: yup.array().of(linkSchema()),
});
