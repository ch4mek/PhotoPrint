import * as yup from 'yup';

import { currencyType1Schema, dateType1Schema } from '@/forms/schemas';
import { ddlType1Schema } from '@/forms/schemas/types/ddlType1';

import { RateHistoryItemFormField } from './types';

export const validationSchema = yup.object().shape({
	date: dateType1Schema(),
	rate: currencyType1Schema(RateHistoryItemFormField.Rate),
	currency: ddlType1Schema(),
});
