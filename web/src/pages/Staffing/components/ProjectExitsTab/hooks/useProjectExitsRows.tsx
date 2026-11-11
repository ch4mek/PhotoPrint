import { useMemo } from 'react';

import { mockProjectExitsGridRow } from '../constants';
import { UseProjectExitsRowsProps } from '../types';

import { DEFAULT_GRID_SKELETON_ROWS_AMOUNT } from '@/components/Grid/constants';
import { getSkeletonRows } from '@/components/Grid/utils/getSkeletonRows';
import { personalInfoClaimAccess } from '@/constants/claims';
import { DATE_FORMAT } from '@/constants/date';
import { isChiefRole } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import dayjs from '@/lib/dayjs';
import { ProjectExitsGridRow } from '@/types/projectExits';

export const useProjectExitsRows = ({
	items,
	dataUpdatedAt,
	page,
	rowsPerPage,
	isDataLoading,
}: UseProjectExitsRowsProps): Array<ProjectExitsGridRow> => {
	const { role, id: authId, claims } = useAuth();

	const canViewPersonalInfoByClaim = useMemo(
		() => checkIsHasClaims(personalInfoClaimAccess, claims),
		[claims]
	);

	return useMemo(() => {
		if (isDataLoading) {
			return getSkeletonRows(
				DEFAULT_GRID_SKELETON_ROWS_AMOUNT,
				mockProjectExitsGridRow
			);
		}

		return (items || []).map(
			({
				isActive,
				isUserActive,
				userId,
				userPosition,
				userResourceGroupName,
				departmentName,
				isManagedByRole,
				closingDate,
				closingReasonName,
				...projectExitItem
			}): ProjectExitsGridRow => {
				const canViewPersonalInfoByRole = isChiefRole(role)
					? !isChiefRole(userPosition)
					: userId !== authId && isManagedByRole;

				const canViewPersonalInfo =
					isUserActive &&
					(canViewPersonalInfoByRole || canViewPersonalInfoByClaim);

				return {
					...projectExitItem,
					userId,
					department: departmentName,
					group: userResourceGroupName,
					isActive,
					disabled: !isActive,
					closingDate: closingDate
						? dayjs(closingDate).format(DATE_FORMAT)
						: null,
					closingReason: closingReasonName,
					canViewPersonalInfo,
				};
			}
		);
	}, [
		authId,
		role,
		dataUpdatedAt,
		page,
		rowsPerPage,
		isDataLoading,
		items,
		canViewPersonalInfoByClaim,
	]);
};
