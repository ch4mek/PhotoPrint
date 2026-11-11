import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';

import {
	TIMESHEET_FREEZE_COLUMN_BREAKPOINT,
	timesheetModesIds,
} from '../../constants';

import { ProtectedView } from '@/components/ProtectedView';
import { isPMO } from '@/constants/departments';
import { managerRolesWithGroupManager, employees } from '@/constants/roles';
import { ListScrollObserverProvider } from '@/context/ListScrollObserver';
import {
	TimesheetEngineer,
	TimeSheetManager,
} from '@/pages/TimeSheet/components';

import { EmployeeGridWrapper, ManagerGridWrapper } from './styles';
import { TimeSheetProps } from './types';

const { personalModeId } = timesheetModesIds;

export function TimeSheet(props: TimeSheetProps) {
	const {
		departmentTypeFilter,
		clientFilter,
		projectFilter,
		userFilter,
		groupFilter,
		month,
		period,
		periodFilter,
		isLoading,
		userId,
		departmentId,
		engineerStatus = null,
	} = props;

	const isTotalColumnFrozen = useHasScreenWidth(
		TIMESHEET_FREEZE_COLUMN_BREAKPOINT
	);

	return (
		<ListScrollObserverProvider>
			<ProtectedView roles={managerRolesWithGroupManager}>
				{departmentTypeFilter?.id !== personalModeId ? (
					<ManagerGridWrapper periodLength={period.length}>
						<TimeSheetManager
							{...{
								groupFilter,
								departmentTypeFilter,
								clientFilter,
								projectFilter,
								userFilter,
								month,
								periodFilter,
								period,
							}}
						/>
					</ManagerGridWrapper>
				) : (
					<EmployeeGridWrapper periodLength={period.length}>
						<TimesheetEngineer
							{...{ isLoading, userId }}
							status={engineerStatus}
						/>
					</EmployeeGridWrapper>
				)}
			</ProtectedView>
			<ProtectedView roles={employees}>
				{departmentTypeFilter?.id !== personalModeId && isPMO(departmentId) ? (
					<ManagerGridWrapper periodLength={period.length}>
						<TimeSheetManager
							{...{
								departmentTypeFilter,
								clientFilter,
								projectFilter,
								userFilter,
								month,
								periodFilter,
								period,
							}}
						/>
					</ManagerGridWrapper>
				) : (
					<EmployeeGridWrapper periodLength={period.length}>
						<TimesheetEngineer
							freezeTotalColumn={isTotalColumnFrozen}
							userId={userId}
							status={engineerStatus}
						/>
					</EmployeeGridWrapper>
				)}
			</ProtectedView>
		</ListScrollObserverProvider>
	);
}
