import * as yup from 'yup';

import {
	emailSchema,
	textType1Dot1Schema,
	phoneNumberType1Schema,
	textType2Schema,
} from '../schemas';

export const validationSchema = yup.object().shape({
	fullName: textType1Dot1Schema('full name'),
	position: textType2Schema('position'),
	phone: phoneNumberType1Schema(),
	email: emailSchema(),
	website: yup.string(),
});
