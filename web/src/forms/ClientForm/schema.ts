import * as yup from 'yup';

import { longTextType1Schema, textType4Schema } from '../schemas';
import { ddlType1Schema } from '../schemas/types/ddlType1';

import { FIELD_IS_REQUIRED } from '@/constants/errors';

export const validationSchema = yup.object({
	name: textType4Schema('client name'),
	legalEntityName: textType4Schema('legal entity name', false),
	comments: longTextType1Schema('notes'),
	saleManager: ddlType1Schema(false),
	registeredLocation: yup.mixed().required(FIELD_IS_REQUIRED),
});
