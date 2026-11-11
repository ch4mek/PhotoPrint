import { FromToProps } from '@/components/Grid';
import { AttendanceTotalFilters } from '@/pages/Attendance/components/TotalTab/types';

export interface AttendanceTotalFilterStore extends AttendanceTotalFilters {
	initialFilters: AttendanceTotalFilters;

	setUserId: (id: string) => void;
	setUser: (user: string) => void;
	setDepartments: (departments: string[]) => void;
	setGroup: (group: string) => void;
	setDate: (date: FromToProps<Date>) => void;
	setTime: (time: FromToProps<string>) => void;
	setAll: (filters: AttendanceTotalFilters) => void;
	reset: () => void;

	initialize: (initialFilters: AttendanceTotalFilters) => void;
}
