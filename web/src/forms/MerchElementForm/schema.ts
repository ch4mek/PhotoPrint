import * as yup from 'yup';

import { notRequiredTextType4Schema } from '../Discount/schema';
import { numberType3Schema } from '../schemas/types/numberType3';

import { longTextType1Schema, textType4Schema } from '@/forms/schemas';

const minAmountValidation = yup
	.object()
	.shape({
		minAmount: numberType3Schema(),
	})
	.nullable();

export const merchElementsValidationSchema = yup.object().shape({
	name: textType4Schema('name'),
	supplier: notRequiredTextType4Schema('supplier'),
	description: longTextType1Schema('description'),
	sizeMinAmounts: yup.array().of(minAmountValidation),
});
