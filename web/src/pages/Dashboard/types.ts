import { Departments } from '@/constants/departments';

export type IncludedDepartments =
	| Departments.Js
	| Departments.Net
	| Departments.Java
	| Departments.Python
	| Departments.QA
	| Departments.Mobile
	| Departments.PHP
	| Departments.DevOps
	| Departments.Ruby
	| Departments.AIML
	| Departments.SAP
	| Departments.Design
	| Departments.BA
	| Departments.PMO;

export interface DashboardFiltersContextValues {
	selectedDepartmentId: Departments | null;
	setSelectedDepartmentId: (departmentId: Departments | null) => void;
}
