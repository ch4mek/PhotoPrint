import { useContext, useMemo, memo, MouseEvent } from 'react';

import {
	HOVER_NAME_CELL_CLASS_NAME,
	HOVER_ROW_CELL_CLASS_NAME,
	PRIMARY_ROW_USER_CELL_CLASS_NAME,
} from '../../../constants';
import { TimeSheetManagerRow } from '../../../types';

import { ExpandedCell } from '@/components/ExpandedCell';
import { cellWithColorCircleRenderer } from '@/components/Grid';
import { isPMO, isHeadOfPMO } from '@/constants/departments';
import { filterByPeriod } from '@/constants/filters';
import {
	isHeadOrGroupManager,
	isChiefRole,
	isRegularEmployee,
} from '@/constants/roles';
import { TimesheetManagerStatus } from '@/pages/TimeSheet/components/TimeSheeetManager/components/TimesheetManagerStatus';
import { timesheetModesIds } from '@/pages/TimeSheet/constants';
import { TimesheetParametersContext } from '@/pages/TimeSheet/context/timesheetParametersContext';
import { TimesheetStatusContext } from '@/pages/TimeSheet/context/timesheetStatusContext';

import {
	UserInfoContent,
	UserInfoContentWrapper,
	UserInfoNameCellWrapper,
	UserInfoWrapper,
} from './styles';

interface UserInfoCellProps {
	expanded: boolean;
	row: TimeSheetManagerRow;
	ariaColIndex: number;
	isActive: boolean;
}

function UserInfoCellInner({
	expanded,
	row,
	isActive,
	ariaColIndex,
}: UserInfoCellProps) {
	const { status, userId, role: userRole, userDepartmentId } = row;

	const {
		periodFilter,
		departmentTypeFilter,
		role,
		departmentId,
		handleUserClick,
		enableCellClick: enableClick,
	} = useContext(TimesheetStatusContext);

	const { isProjectsModeOn } = useContext(TimesheetParametersContext);

	const handleCellClick = (
		_row: unknown,
		_columnKey: string,
		e: MouseEvent<HTMLDivElement>
	) => {
		e.stopPropagation();
		handleUserClick(userId);
	};

	const shouldStatusRenderForHeadAndGroupManager = useMemo(
		() =>
			isHeadOrGroupManager(role) &&
			filterByPeriod.monthFilter === periodFilter &&
			(timesheetModesIds.departmentModeId === departmentTypeFilter?.id ||
				timesheetModesIds.groupModeId === departmentTypeFilter?.id),

		[role, periodFilter, departmentTypeFilter?.id]
	);

	const shouldStatusRenderForChiefs = useMemo(
		() => isChiefRole(role) && filterByPeriod.monthFilter === periodFilter,
		[role, periodFilter]
	);

	const shouldStatusRender =
		shouldStatusRenderForHeadAndGroupManager || shouldStatusRenderForChiefs;

	const isUserChief = useMemo(() => isChiefRole(userRole), [userRole]);

	const isHeadOfPMOHoversOverUserNotFromHisDepartment = useMemo(
		() => isHeadOfPMO(departmentId, role) && !isPMO(userDepartmentId),
		[departmentId, role, userDepartmentId]
	);

	const isEmployeeOfPMO = useMemo(
		() => isPMO(departmentId) && isRegularEmployee(role),
		[departmentId, role]
	);

	const enableCellClick =
		enableClick(userId) &&
		!isUserChief &&
		!(isHeadOfPMOHoversOverUserNotFromHisDepartment && isProjectsModeOn) &&
		!(isEmployeeOfPMO && isProjectsModeOn) &&
		isActive;

	return (
		<UserInfoWrapper
			className={`${PRIMARY_ROW_USER_CELL_CLASS_NAME} ${HOVER_ROW_CELL_CLASS_NAME} ${HOVER_NAME_CELL_CLASS_NAME}`}
			data-hovercolindex={ariaColIndex}
		>
			<UserInfoContentWrapper>
				<UserInfoNameCellWrapper
					shouldStatusRender={shouldStatusRender}
					periodFilter={periodFilter}
				>
					<UserInfoContent>
						{cellWithColorCircleRenderer(
							{
								row: {
									...row,
									disabled: !isActive,
								},
								column: { key: 'userName' },
							} as any,
							enableCellClick
								? {
										onCellClick: handleCellClick,
								  }
								: undefined
						)}
					</UserInfoContent>
					{shouldStatusRender && (
						<TimesheetManagerStatus userId={userId} status={status} />
					)}
					<ExpandedCell expanded={expanded} tabIndex={0} />
				</UserInfoNameCellWrapper>
			</UserInfoContentWrapper>
		</UserInfoWrapper>
	);
}

export const UserInfoCell = memo(UserInfoCellInner);
