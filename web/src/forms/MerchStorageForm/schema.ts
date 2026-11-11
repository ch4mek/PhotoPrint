import * as yup from 'yup';

import { numberType3Schema } from '../schemas/types/numberType3';

import {
	ddlType1Schema,
	longTextType1Schema,
	numberType2Schema,
} from '@/forms/schemas';

const minAmountValidation = yup
	.object()
	.shape({
		count: numberType3Schema(),
	})
	.nullable();

export const merchStorageValidationSchema = yup.object().shape({
	warehouseId: numberType2Schema('warehouseId'),
	recipient: yup.mixed().when('transactionType', {
		is: 'Receipt',
		then: ddlType1Schema(false),
		otherwise: ddlType1Schema(),
	}),

	transactionType: ddlType1Schema(),
	notes: longTextType1Schema('notes'),
	sizeCounts: yup.array().of(minAmountValidation),
});
