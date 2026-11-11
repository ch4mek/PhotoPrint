import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { breakpoints } from '@mms/mms-ui-library/theme';
import {
	useCallback,
	useContext,
	useEffect,
	useMemo,
	useRef,
	useState,
} from 'react';

import { ManagerContext } from '../..';
import { TimesheetEngineer } from '../../../TimesheetEngineer';
import { TIMESHEET_DATE_FORMAT } from '../../../TimesheetEngineer/constants';
import { TimeTrackItem } from '../../../TimesheetEngineer/types';
import {
	MANAGER_TIMESHEET_ROW_WRAPPER_CLASS_NAME,
	USER_GRID_WRAPPER_CLASS_NAME,
	USER_TOTAL_ROW_WRAPPER_CLASS_NAME,
} from '../../constants';
import { TimeSheetManagerRow } from '../../types';
import { getRowHeight } from '../../utils/getRowHeight';

import { USER_IS_INACTIVE } from '@/constants/errors';
import { groupManagerAndHead, Roles } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import dayjs from '@/lib/dayjs';
import {
	TIMESHEET_FREEZE_COLUMN_BREAKPOINT,
	USER_INFO_COLUMN_HOVER_INDEX,
} from '@/pages/TimeSheet/constants';
import { MutateTimesheetStatusContext } from '@/pages/TimeSheet/context/mutateTimesheetStatusContext';
import { TimesheetParametersContext } from '@/pages/TimeSheet/context/timesheetParametersContext';
import { TimesheetStatusContext } from '@/pages/TimeSheet/context/timesheetStatusContext';
import { TimesheetStatusType } from '@/pages/TimeSheet/types';
import { useUpdateTimesheetStatus } from '@/queries/timesheet/useUpdateUserTimesheetStatus';

import { DayTotalTimeCell } from './components/DayTotalTime';
import { PeriodTotalTimeCell } from './components/PeriodTotalTime';
import { UserInfoCell } from './components/UserInfo';
import {
	UserTimesheetGrid,
	RowWrapper,
	TimeSheetEngineerGridWrapper,
	UserTimesheetWrapper,
	overrideEngineerTimesheetStyle,
} from './styles';

interface UserTimesheetProps {
	row: TimeSheetManagerRow;
	onRowHeightChange: (height: number) => void;
	onExpandedChange: (isExpanded: boolean) => void;
	onTotalTimeMapChange: (
		newTimeMap: Map<string, undefined | TimeTrackItem>
	) => void;
	onTimesheetStatusChange: (status: TimesheetStatusType) => void;
}

export function UserTimesheet(props: UserTimesheetProps) {
	const {
		row,
		onRowHeightChange,
		onExpandedChange,
		onTotalTimeMapChange,
		onTimesheetStatusChange,
	} = props;
	const { role, departmentId } = useAuth();
	const { userId, isActiveUser, userDepartmentId, isExpanded, status } = row;
	const { period } = useContext(TimesheetParametersContext);

	const { areColumnsFrozen } = useContext(ManagerContext);

	const isTotalColumnFrozen = useHasScreenWidth(
		TIMESHEET_FREEZE_COLUMN_BREAKPOINT
	);

	const [timeMap, setTimeMap] = useState(row.timeMap);

	const gridWrapperRef = useRef<HTMLDivElement | null>(null);

	const periodLength = period.length;

	const { periodFilter, departmentTypeFilter } = useContext(
		TimesheetStatusContext
	);

	const { tablet } = breakpoints;
	const isTabletWidth = useHasScreenWidth(tablet);

	const rowHeight = getRowHeight(
		role,
		isTabletWidth,
		periodFilter,
		departmentTypeFilter
	);

	const handleRowClick = () => {
		onTotalTimeMapChange(timeMap);
		onExpandedChange(!isExpanded);
	};

	useEffect(() => {
		setTimeMap(row.timeMap);
	}, [row.timeMap]);

	const handleExpand = useCallback(
		(height: number) => {
			onRowHeightChange(rowHeight + height);
		},
		[onRowHeightChange, rowHeight]
	);

	const userDepartmentAndRoleAllowToEditTimesheet = useMemo(() => {
		if (role === Roles.SE) {
			return false;
		}

		if (groupManagerAndHead.includes(role)) {
			return departmentId === userDepartmentId;
		}

		return true;
	}, [role, departmentId, userDepartmentId]);

	const displayTooltips = userDepartmentAndRoleAllowToEditTimesheet;

	const isDisabledUserTimesheet = !isActiveUser || !displayTooltips;

	const disableMessage =
		displayTooltips && isDisabledUserTimesheet ? USER_IS_INACTIVE : undefined;

	const overrideGridStyles = useMemo(() => overrideEngineerTimesheetStyle, []);

	const { month, year } = useContext(TimesheetStatusContext);

	const { mutate } = useUpdateTimesheetStatus(userId, month, year, {
		onSuccess: (data) => {
			onTimesheetStatusChange(data.status);
		},
	});

	const MutateTimesheetStatusContextValue = useMemo(
		() => ({ mutate }),
		[mutate]
	);

	return (
		<UserTimesheetWrapper className={MANAGER_TIMESHEET_ROW_WRAPPER_CLASS_NAME}>
			<MutateTimesheetStatusContext.Provider
				value={MutateTimesheetStatusContextValue}
			>
				<UserTimesheetGrid ref={gridWrapperRef}>
					<RowWrapper
						onClick={handleRowClick}
						className={USER_TOTAL_ROW_WRAPPER_CLASS_NAME}
					>
						<UserInfoCell
							expanded={isExpanded}
							row={row}
							ariaColIndex={USER_INFO_COLUMN_HOVER_INDEX}
							isActive={isActiveUser}
						/>
						{period.map((day, index) => {
							const dateStringRepresentation = dayjs(day).format(
								TIMESHEET_DATE_FORMAT
							);

							return (
								<DayTotalTimeCell
									ariaColIndex={index + 2}
									column={index + 3}
									key={dateStringRepresentation}
									day={dateStringRepresentation}
									timeMap={timeMap}
									displayRightBorder={index === periodLength - 1}
								/>
							);
						})}
						<PeriodTotalTimeCell
							periodLength={periodLength}
							timeMap={timeMap}
						/>
					</RowWrapper>
					{isExpanded && (
						<TimeSheetEngineerGridWrapper
							className={USER_GRID_WRAPPER_CLASS_NAME}
						>
							<TimesheetEngineer
								isDisabled={isDisabledUserTimesheet}
								disableMessage={disableMessage}
								freezeTaskInfoColumn={areColumnsFrozen}
								freezeTotalColumn={isTotalColumnFrozen}
								isActiveUser={isActiveUser}
								userId={userId}
								isEngineer={false}
								displayTotalRow={false}
								overrideGridStyles={overrideGridStyles}
								onTotalTimeMapChange={setTimeMap}
								onGetTotalTimeMapOnFirstRender={onTotalTimeMapChange}
								displayTooltips={displayTooltips}
								areColumnsFrozen={areColumnsFrozen}
								status={status}
								onExpand={handleExpand}
								hideHeaderRow
							/>
						</TimeSheetEngineerGridWrapper>
					)}
				</UserTimesheetGrid>
			</MutateTimesheetStatusContext.Provider>
		</UserTimesheetWrapper>
	);
}
