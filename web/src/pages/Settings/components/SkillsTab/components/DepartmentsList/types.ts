import { DepartmentType } from '@/types/configuration';

export interface DepartmentsListProps {
	selectedDepartmentId: string | null;
	handleDepartmentItemClick: (department: DepartmentType) => void;
}
