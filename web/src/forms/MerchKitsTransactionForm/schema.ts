import * as yup from 'yup';

import { numberType3Schema } from '../schemas/types/numberType3';

import {
	ddlType1Schema,
	longTextType1Schema,
	numberType2Schema,
} from '@/forms/schemas';

const sizeCountsValidation = yup
	.object()
	.shape({
		count: numberType3Schema(),
	})
	.nullable();

export const merchKitValidationSchema = yup.object().shape({
	warehouseId: numberType2Schema('warehouseId'),
	recipient: yup.mixed().when('transactionType', {
		is: 'Receipt',
		then: ddlType1Schema(false),
		otherwise: ddlType1Schema(),
	}),
	transactionType: yup.string().required(),
	amount: yup.number().required(),
	notes: longTextType1Schema('notes'),
	sizeCounts: yup.array().of(sizeCountsValidation),
});
