import * as yup from 'yup';

import { StatusEquipment } from '../constants';

import {
	FIELD_IS_REQUIRED,
	getMaxLengthErrorMessage,
	getMinLengthErrorMessage,
	getPleaseEnterAValidFieldErrorMessage,
} from '@/constants/errors';
import {
	currencyType1Schema,
	dateType1Schema,
	longTextType1Schema,
	textType3Schema,
	textType4Schema,
} from '@/forms/schemas';
import { TEXT_TYPE_3_REG_EXP } from '@/forms/schemas/constants';
import { ddlType1Schema } from '@/forms/schemas/types/ddlType1';
import { checkIfNoLettersOrNumbers } from '@/forms/utils';
import { getTrimedString } from '@/helpers/getTrimedString';

import { MAIN_FORM_FIELDS } from './constants';

const repairPlaceValidationSchema = (fieldName: string) => {
	const NOT_VALID_FIELD = getPleaseEnterAValidFieldErrorMessage(fieldName);
	const MIN_CHARACTERS = 2;
	const MAX_CHARACTERS = 250;

	return yup
		.string()
		.nullable()
		.transform((value: string) => (value ? getTrimedString(value) : value))
		.notRequired()
		.when({
			is: (value: string) => !!value,
			then: yup
				.string()
				.min(MIN_CHARACTERS, getMinLengthErrorMessage(MIN_CHARACTERS))
				.max(MAX_CHARACTERS, getMaxLengthErrorMessage(MAX_CHARACTERS))
				.test(
					'only-symbols-check',
					NOT_VALID_FIELD,
					(value) => !checkIfNoLettersOrNumbers(value)
				)
				.matches(TEXT_TYPE_3_REG_EXP, NOT_VALID_FIELD),
			otherwise: yup.string().nullable(),
		});
};

export const getValidationSchema = (isEditing: boolean = false) =>
	yup.object().shape({
		name: textType4Schema('equipment name'),
		equipmentType: ddlType1Schema(),
		status: yup.mixed().required(FIELD_IS_REQUIRED),
		serialNumber: textType4Schema('serial number'),
		inventoryNumber: isEditing
			? textType4Schema(
					'inventory number',
					undefined,
					undefined,
					undefined,
					true
			  )
			: textType4Schema(
					'inventory number',
					undefined,
					undefined,
					undefined,
					true
			  ).notRequired(),
		invoiceNumber: textType4Schema('invoice number'),
		purchasePrice: currencyType1Schema('purchase price'),
		purchaseCurrency: yup.string(),
		purchasePriceUsd: currencyType1Schema('purchase price USD'),
		purchasePriceUsdCurrency: yup.string(),
		purchaseDate: dateType1Schema(),
		purchasePlace: textType3Schema('purchase place'),
		guaranteeDate: dateType1Schema(),
		characteristics: longTextType1Schema('сharacteristics')
			.required(FIELD_IS_REQUIRED)
			.nullable(),
		jiraTask: longTextType1Schema('jira task')
			.required(FIELD_IS_REQUIRED)
			.nullable(),
		approver: ddlType1Schema(),
		storage: yup.mixed().required(FIELD_IS_REQUIRED),
		location: ddlType1Schema(),
		comment: longTextType1Schema('notes'),
		repairPlace: yup.string().when(MAIN_FORM_FIELDS.status.name, {
			is: (value: StatusEquipment) => value === StatusEquipment.RENOVATION,
			then: textType3Schema('repair place').nullable(),
			otherwise: repairPlaceValidationSchema('repair place'),
		}),
	});
