import { useMemo } from 'react';

import { EducationsGridRow } from '../types';

import { findById } from '@/helpers/findById';
import { PersistentConfigurationItem } from '@/types/configuration';
import { EducationsResponse } from '@/types/educations';

export const useEducationsGridRows = ({
	educations,
	educationLevelTypes,
	studyFormTypes,
}: {
	educations: Array<EducationsResponse>;
	educationLevelTypes: Array<PersistentConfigurationItem>;
	studyFormTypes: Array<PersistentConfigurationItem>;
}) =>
	useMemo<Array<EducationsGridRow>>(
		() =>
			educations.map(
				({
					educationLevel,
					educationalInstitution,
					faculty,
					specialization,
					studyForm,
					graduationYear,
				}) => ({
					educationLevel:
						findById(educationLevelTypes, educationLevel)?.description ??
						educationLevel,
					educationalInstitution,
					faculty,
					specialization,
					studyForm:
						findById(studyFormTypes, studyForm)?.description ?? studyForm,
					graduationYear,
				})
			),
		[educations, educationLevelTypes, studyFormTypes]
	);
