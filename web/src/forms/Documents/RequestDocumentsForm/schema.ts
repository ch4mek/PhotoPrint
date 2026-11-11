import * as Yup from 'yup';

import { FIELD_IS_REQUIRED } from '@/constants/errors';

export const getValidationSchema = () =>
	Yup.object().shape({
		documentType: Yup.object().required(FIELD_IS_REQUIRED).nullable(),
		notes: Yup.string().min(2).max(1000).nullable(),
	});
