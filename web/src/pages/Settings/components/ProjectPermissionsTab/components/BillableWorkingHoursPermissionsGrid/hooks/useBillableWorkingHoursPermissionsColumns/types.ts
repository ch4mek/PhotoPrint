import { ProjectPermissionsSetFilters } from '../../../../types';

export interface UseBillableWorkingHoursPermissionsColumnsOptions {
	userIdFilter: string;
	userFullNameFilter: string;
	projectNameFilter: string;
	usersOptions: string[];
	setFilters: ProjectPermissionsSetFilters;
}
