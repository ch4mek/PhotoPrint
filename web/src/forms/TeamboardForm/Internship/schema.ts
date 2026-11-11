import * as yup from 'yup';

import { FIELD_IS_REQUIRED } from '@/constants/errors';
import {
	dateType1Schema,
	numberType1Schema,
	textType4Schema,
} from '@/forms/schemas';

import { technologiesValidationSchema } from './components/TechnologiesFields/schema';
import { internshipFormFields } from './constants';

const {
	internshipSpecialization,
	internshipType,
	internshipEngineerStage,
	internshipFinalResult,
	internshipCompletedPoints,
	internshipApprovedPoints,
	internshipStartDate,
	internshipPlannedEndDate,
} = internshipFormFields;

const optionalNumberType1Schema = (label: string) =>
	numberType1Schema(label.toLowerCase())
		.transform((value) => (value === '' ? null : value))
		.nullable();

export const validationSchema = yup
	.object()
	.shape({
		[internshipSpecialization.name]: textType4Schema(
			internshipSpecialization.label.toLowerCase()
		),
		[internshipType.name]: yup.mixed().nullable().required(FIELD_IS_REQUIRED),
		[internshipEngineerStage.name]: numberType1Schema(
			internshipEngineerStage.label.toLowerCase()
		).required(FIELD_IS_REQUIRED),
		[internshipFinalResult.name]: yup.mixed().nullable().notRequired(),
		[internshipCompletedPoints.name]: optionalNumberType1Schema(
			internshipCompletedPoints.label
		),
		[internshipApprovedPoints.name]: optionalNumberType1Schema(
			internshipApprovedPoints.label
		),
		[internshipStartDate.name]: dateType1Schema(),
		[internshipPlannedEndDate.name]: dateType1Schema(),
	})
	.concat(technologiesValidationSchema);
