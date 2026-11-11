import type { Dayjs } from '@/lib/dayjs';
import { Project } from '@/queries/types';
import {
	DepartmentType,
	PersistentConfigurationItem,
	LocationType,
	CalendaryCountry,
	RoleType,
} from '@/types/configuration';
import { DepartmentGroup } from '@/types/department';
import { UserNameValues } from '@/types/personalData';

export interface UserFormValues extends UserNameValues {
	color: string;
	email: string;
	department: DepartmentType | null;
	group: DepartmentGroup | null;
	position: RoleType | null;
	currentLocation: string | null;
	location: LocationType | null;
	registeredLocation: CalendaryCountry | null;
	registeredLocationId: number | null;
	projects: Project[];
	stack?: PersistentConfigurationItem | null;
	date?: Date | string | Dayjs;
	rightDepartment?: string;
	rightPosition?: string;
	isActive: boolean;
}

type PlateNodeChildren = Array<{ text: string } | PlateNode>;

export interface PlateNode {
	children: PlateNodeChildren;
	type: string;
	id?: string;
}

export type DateRangeValidator = (fromDate: Date, toDate: Date) => boolean;
