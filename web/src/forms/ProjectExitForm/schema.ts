import * as yup from 'yup';

import { dateType1Schema, ddlType1Schema, ddlType2Schema } from '../schemas';

export const schema = yup.object({
	user: ddlType1Schema(),
	client: ddlType1Schema(),
	project: ddlType1Schema(),
	closingDate: dateType1Schema(),
	closingReason: ddlType2Schema('closing reason'),
});
