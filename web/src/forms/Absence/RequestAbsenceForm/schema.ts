import * as yup from 'yup';

import { FIELD_IS_REQUIRED } from '@/constants/errors';
import {
	dateType1Schema,
	longTextType1Schema,
	numberType1Schema,
} from '@/forms/schemas';
import {
	dateType2FromDate,
	dateType2ToDate,
} from '@/forms/schemas/types/dateType2';
import { ddlType1Schema } from '@/forms/schemas/types/ddlType1';
import { validateDateRangeWithinMaxYears } from '@/forms/utils';

import { fieldsNames, MAX_LENGTH_HOURS } from './constants';

const { dateFrom, dateTo, hours, notes } = fieldsNames;

const getDateSchema = (isAgreementMandatory: boolean, field: string) => {
	if (!isAgreementMandatory) {
		return dateType1Schema();
	}

	if (field === dateFrom) {
		const schema = dateType2FromDate(dateTo, undefined, undefined, true);

		return schema;
	}

	return dateType2ToDate(
		dateFrom,
		undefined,
		undefined,
		true,
		validateDateRangeWithinMaxYears()
	);
};

export const getValidationSchema = (isAgreementMandatory: boolean) =>
	yup.object({
		absenceName: ddlType1Schema(),
		dateFrom: getDateSchema(isAgreementMandatory, dateFrom),
		dateTo: getDateSchema(isAgreementMandatory, dateTo),
		hours: numberType1Schema(hours, MAX_LENGTH_HOURS, undefined, '', true).when(
			[],
			{
				is: () => isAgreementMandatory,
				then: (schema) => schema.nullable().optional(),
				otherwise: (schema) => schema.required(FIELD_IS_REQUIRED),
			}
		),
		notes: longTextType1Schema(notes),
	});
