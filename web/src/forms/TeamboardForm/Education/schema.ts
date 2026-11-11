import * as yup from 'yup';

import {
	longTextType1Schema,
	numberType1Schema,
	textType4Schema,
} from '../../schemas';

import { FIELD_IS_REQUIRED } from '@/constants/errors';

import { EDUCATION_FORM_FIELDS } from './constants';

export const validationSchema = yup.object().shape({
	educationLevel: yup.mixed().required(FIELD_IS_REQUIRED).nullable(),
	educationalInstitution: textType4Schema(
		EDUCATION_FORM_FIELDS.educationalInstitution.label.toLowerCase()
	),
	faculty: textType4Schema(EDUCATION_FORM_FIELDS.faculty.label.toLowerCase()),
	specialization: textType4Schema(
		EDUCATION_FORM_FIELDS.specialization.label.toLowerCase()
	),
	studyForm: yup.mixed().required(FIELD_IS_REQUIRED).nullable(),
	graduationYear: numberType1Schema(
		EDUCATION_FORM_FIELDS.graduationYear.label.toLowerCase()
	).required(FIELD_IS_REQUIRED),
	comment: longTextType1Schema(
		EDUCATION_FORM_FIELDS.comment.label.toLowerCase()
	),
});
