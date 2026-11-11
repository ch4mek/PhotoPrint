import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import { formatNullableNumber } from '@/helpers/formatNullableNumber';
import { getDateFormat } from '@/helpers/getDateFormat';
import { toNullableDayjs, toNullableNumber } from '@/helpers/nullable';
import {
	InternshipResponse,
	UpdateInternshipPayload,
} from '@/types/internship';

import {
	mapFormItemsToTechnologies,
	mapTechnologiesToFormItems,
} from './components/TechnologiesFields/utils';
import { InternshipFormValues, UpdateInternship } from './types';

export const getInternshipFormValuesFromResponse = (
	data: InternshipResponse
): InternshipFormValues => ({
	id: data.id,
	internshipSpecialization: data.specialization ?? '',
	internshipType: data.type ?? null,
	internshipEngineerStage: formatNullableNumber(data.engineerStage) ?? '',
	internshipFinalResult: data.finalResult,
	internshipCompletedPoints: formatNullableNumber(data.completedPoints) ?? '',
	internshipApprovedPoints: formatNullableNumber(data.approvedPoints) ?? '',
	internshipStartDate: toNullableDayjs(data.startDate),
	internshipPlannedEndDate: toNullableDayjs(data.plannedEndDate),
	technologies: mapTechnologiesToFormItems(
		data.userInternshipSpecializations ?? []
	),
});

export const getInternshipRequestFromFormValues = ({
	id,
	internshipSpecialization,
	internshipType,
	internshipEngineerStage,
	internshipFinalResult,
	internshipCompletedPoints,
	internshipApprovedPoints,
	internshipStartDate,
	internshipPlannedEndDate,
	technologies,
}: UpdateInternship): UpdateInternshipPayload => ({
	id,
	specialization: internshipSpecialization,
	type: internshipType!,
	engineerStage: Number(internshipEngineerStage),
	finalResult: internshipFinalResult,
	completedPoints: toNullableNumber(internshipCompletedPoints),
	approvedPoints: toNullableNumber(internshipApprovedPoints),
	startDate: getDateFormat(internshipStartDate, YYYY_MM_DD_DAYJS_DATE_FORMAT),
	plannedEndDate: getDateFormat(
		internshipPlannedEndDate,
		YYYY_MM_DD_DAYJS_DATE_FORMAT
	),
	internshipSpecializations: mapFormItemsToTechnologies(technologies),
});
