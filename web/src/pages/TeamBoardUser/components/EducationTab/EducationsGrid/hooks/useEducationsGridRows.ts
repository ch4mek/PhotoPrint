import { useMemo } from 'react';

import { EducationsGridRow } from '../types';

import { findById } from '@/helpers/findById';
import { PersistentConfigurationItem } from '@/types/configuration';
import { EducationsResponse } from '@/types/educations';

export const useEducationsGridRows = ({
	educations,
	educationLevelTypes,
	studyFormTypes,
	handleItemEditClick,
}: {
	educations: Array<EducationsResponse>;
	educationLevelTypes: Array<PersistentConfigurationItem>;
	studyFormTypes: Array<PersistentConfigurationItem>;
	handleItemEditClick: (id: number) => VoidFunction;
}) =>
	useMemo<Array<EducationsGridRow>>(
		() =>
			educations.map(
				({
					id,
					educationLevel,
					educationalInstitution,
					faculty,
					specialization,
					studyForm,
					graduationYear,
					comment,
				}) => ({
					id,
					educationLevel:
						findById(educationLevelTypes, educationLevel)?.description ??
						educationLevel,
					educationalInstitution,
					faculty,
					specialization,
					studyForm:
						findById(studyFormTypes, studyForm)?.description ?? studyForm,
					graduationYear,
					comment: comment ?? '',
					handleEditClick: handleItemEditClick as () => VoidFunction,
				})
			),
		[educations, educationLevelTypes, studyFormTypes, handleItemEditClick]
	);
