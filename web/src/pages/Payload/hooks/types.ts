import { Roles } from '@/constants/roles';

export interface PayloadFiltersProps {
	role: Roles;
	departmentId: string;
	order: string;
	orderBy: string;
	specializationTitle: string;
}
