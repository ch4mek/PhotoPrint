import * as yup from 'yup';

import { FIELD_IS_REQUIRED } from '@/constants/errors';
import { textType1Schema } from '@/forms/schemas';

export const validationSchema = yup.object().shape({
	currentLocation: textType1Schema('current location', true).nullable(),
	registeredLocation: yup.mixed().required(FIELD_IS_REQUIRED),
	location: yup.mixed().required(FIELD_IS_REQUIRED),
});
