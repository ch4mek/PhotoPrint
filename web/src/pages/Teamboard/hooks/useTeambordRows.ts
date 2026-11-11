import { useMemo } from 'react';

import { HandleGridItemClick } from '@/components/Grid';
import { DEFAULT_GRID_SKELETON_ROWS_AMOUNT } from '@/components/Grid/constants';
import { Dayjs } from '@/lib/dayjs';
import { RoleType } from '@/types/configuration';
import { RolesType } from '@/types/index';
import { Teamboard, TeamboardGridRow } from '@/types/teamboard';

const mockRow: TeamboardGridRow = {
	id: 0,
	profExperienceInMonths: 0,
	fullName: '',
	primarySpecializationName: [],
	departmentFullName: '',
	resourceGroupName: '',
	currentLocation: '',
	location: '',
	registeredLocation: '',
	positionOrderWeight: '',
	showSkeleton: true,
	disabled: false,
	comments: '',
	englishLevelOrderWeight: '',
	seniorityLevelOrderWeight: '',
};

type UseTeamBoardRowsParams = {
	items: Teamboard[] | undefined;
	dataUpdatedAt: Dayjs | Date | number;
	page: number;
	rowsPerPage: number;
	showSkeleton: boolean;
	role: RolesType;
	roleTypes: RoleType[];
	handleEdit?: HandleGridItemClick;
};

export const useTeamBoardRows = ({
	items,
	dataUpdatedAt,
	page,
	rowsPerPage,
	showSkeleton,
	role,
	handleEdit,
	roleTypes,
}: UseTeamBoardRowsParams): TeamboardGridRow[] =>
	useMemo(
		() =>
			showSkeleton || !items?.length
				? new Array(items?.length ?? DEFAULT_GRID_SKELETON_ROWS_AMOUNT)
						.fill(mockRow)
						.map((row, index) => ({ ...row, id: -99999 - index }))
				: items.map(
						({
							id,
							fullName,
							location,
							registeredLocation,
							currentLocation,
							position,
							profExperienceInMonths,
							careerStartedAt,
							comments,
							englishLevelName,
							seniorityLevel,
							primarySpecializationName,
							odpNonPrimarySpecializations,
							departmentFullName,
							resourceGroupName,
						}) => {
							const additionalSpecializationNames =
								odpNonPrimarySpecializations.map(
									(specialization) =>
										`${specialization.name} (${specialization.seniorityLevel})`
								);

							const positionOrderWeight = roleTypes.find(
								({ id: roleTypeId }) => roleTypeId === position
							)?.description;

							return {
								id,
								positionOrderWeight,
								currentLocation,
								location,
								registeredLocation,
								handleEditClick: handleEdit,
								role,
								showSkeleton,
								profExperienceInMonths: careerStartedAt
									? profExperienceInMonths
									: null,
								fullName,
								primarySpecializationName: [
									primarySpecializationName,
									...additionalSpecializationNames,
								],
								comments,
								englishLevelOrderWeight: englishLevelName,
								departmentFullName,
								seniorityLevelOrderWeight: seniorityLevel,
								resourceGroupName,
							};
						}
				  ),

		[dataUpdatedAt, page, rowsPerPage, showSkeleton, items]
	);
