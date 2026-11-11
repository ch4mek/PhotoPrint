import * as yup from 'yup';

import {
	textType4Schema,
	longTextType1Schema,
	dateType1Schema,
	timeSchema,
} from '../schemas';
import { ddlType1Schema } from '../schemas/types/ddlType1';

export const schema = yup.object({
	user: ddlType1Schema(),
	specialization: textType4Schema('specialization'),
	client: textType4Schema('client', false),
	jiraTask: longTextType1Schema('jira task'),
	type: ddlType1Schema(false),
	status: ddlType1Schema(false),
	date: dateType1Schema(false),
	time: timeSchema('time'),
	saleManager: ddlType1Schema(false),
	reasonForRefusal: textType4Schema('reason for refusal', false),
	note: longTextType1Schema('note'),
});
