import { DEFAULT_GRID_ROW_HEIGHT } from '@mms/mms-ui-library/constants/grid';
import { useMemo } from 'react';

import {
	SKELETON_SPECIALIZATION_ROW,
	SKELETON_SPECIALIZATION_ROW_AMOUNT,
} from '../constants';

import { getSkeletonRows } from '@/components/Grid/utils/getSkeletonRows';
import { getSpecializationData } from '@/pages/TeamBoardUser/components/SpecializationTable/utils/getSpecializationData';
import {
	SpecializationRow,
	SpecializationRowType,
} from '@/pages/TeamBoardUser/types';
import { UserSpecializationInfo } from '@/types/teamboard';

export const useTabSpecializationTableRows = (
	specializations: UserSpecializationInfo[] | undefined,
	showSkeleton?: boolean
): Array<SpecializationRow> =>
	useMemo(() => {
		if (showSkeleton) {
			return getSkeletonRows(
				SKELETON_SPECIALIZATION_ROW_AMOUNT,
				SKELETON_SPECIALIZATION_ROW
			);
		}

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
					specializationTemplateId,
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
	}, [specializations, showSkeleton]);
