import { AutoselectStylesConfig } from '@mms/mms-ui-library/components/Autoselects/Autoselect/types';
import { Dispatch, SetStateAction } from 'react';

import { TimesheetModeFilterOption } from '../../types';

import { ALL_ID } from '@/constants/filters';
import { WithMobileView } from '@/types/filter';

export interface TimeSheetControlsBarProps extends WithMobileView {
	setDate: (date: (prev: Date) => Date) => void;
	weekDisplay: string;
	clientFilter: FilterOption;
	setClient: FilterSetter;
	projectFilter: FilterOption;
	setProject: FilterSetter;
	departmentTypeFilter: TimesheetModeFilterOption | null;
	setDepartmentTypeFilter: TimesheetModeFilterSetter;
	initialClientsValue: FilterOption<typeof ALL_ID>;
	initialProjectsValue: FilterOption<typeof ALL_ID>;
	initialUserFilterValue: FilterOption<typeof ALL_ID>;
	setPeriod: (period: string) => void;
	periodFilter: string;
	setUserFilter: FilterSetter;
	userFilter: FilterOption;
	groupFilter: FilterOption;
	setGroupFilter: FilterSetter;
	month: number;
	currentWeek: number;
	currentDate: Date;
	period: Date[];
	disabled?: boolean;
	onClick?: () => void;
	isEmployeeOrSysAdminRole: boolean;
}

export type TimeSheetFiltersProps = Pick<
	TimeSheetControlsBarProps,
	| 'clientFilter'
	| 'setClient'
	| 'projectFilter'
	| 'setProject'
	| 'initialClientsValue'
	| 'initialProjectsValue'
	| 'initialUserFilterValue'
	| 'setUserFilter'
	| 'userFilter'
	| 'departmentTypeFilter'
	| 'period'
> & {
	departmentId: string;
	stylesConfig: AutoselectStylesConfig;
	groupFilter?: FilterOption;
} & WithMobileView;

interface FilterOption<IdType = typeof ALL_ID | number | string> {
	id: IdType;
	name: string;
}

export type FilterSetter<T = FilterOption> = Dispatch<SetStateAction<T>>;

export type TimesheetModeFilterSetter = Dispatch<
	SetStateAction<TimesheetModeFilterOption>
>;

export type TimesheetFilterChangeHandler = (
	newFilter: FilterOption | null
) => void;
