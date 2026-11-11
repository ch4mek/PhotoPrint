import { DEFAULT_GRID_ROW_HEIGHT } from '@mms/mms-ui-library/constants/grid';
import { useMemo } from 'react';

import { SpecializationRow, SpecializationRowType } from '../../../types';
import { getSpecializationData } from '../utils/getSpecializationData';

import { UserSpecializationInfo } from '@/types/userProfile';

export const useTabSpecializationTableRows = (
	specializations: UserSpecializationInfo[] | undefined
): Array<SpecializationRow> =>
	useMemo(() => {
		if (!specializations) {
			return [];
		}

		return specializations.map(
			(
				{
					id,
					isPrimary,
					specializationTemplateName,
					specializationTemplateId,
					specializationSnapshots,
				},
				index
			) => {
				const { snapshots, hardSkills } = getSpecializationData(
					specializationSnapshots,
					specializationTemplateId
				);

				return {
					id,
					specializationId: id,
					isPrimary,
					name: specializationTemplateName,
					isExpanded: false,
					height: DEFAULT_GRID_ROW_HEIGHT,
					snapshots,
					type: SpecializationRowType.Specialization,
					hardSkills,
					isEven: index % 2 === 0,
				};
			}
		);
	}, [specializations]);
