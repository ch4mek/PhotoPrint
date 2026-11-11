import * as yup from 'yup';

import { longTextType1Schema, textType4Schema } from '../schemas';
import { numberType3Schema } from '../schemas/types/numberType3';

const minAmountValidation = yup
	.object()
	.shape({
		minimumAmount: numberType3Schema(),
	})
	.nullable();

const compoundValidation = yup
	.object()
	.shape({
		amountInKit: numberType3Schema(),
		itemId: yup.number().required(),
	})
	.nullable();

export const merchKitsValidationSchema = yup.object().shape({
	name: textType4Schema('name'),
	description: longTextType1Schema('description'),
	minAmounts: yup.array().of(minAmountValidation),
	compounds: yup.array().of(compoundValidation),
});
