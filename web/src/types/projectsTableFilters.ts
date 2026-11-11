import { FromToProps } from '@/components/Grid';

export interface ProjectsTableFilters {
	projectName: string;
	clientName: string;
	isActive: string[];
	billability: string[];
	startDate: FromToProps<Date>;
	endDate: FromToProps<Date>;
}

export interface ProjectTableFilterSetter {
	projectName: (name: string) => void;
	clientName: (name: string) => void;
	isActive: (isActive: string[]) => void;
	billability: (billability: string[]) => void;
	startDate: (date: FromToProps<Date>) => void;
	endDate: (date: FromToProps<Date>) => void;
	set: (allFilters: ProjectsTableFilters) => void;
}

export enum ProjectTableFiltersKeys {
	projectName = 'projectName',
	billability = 'billability',
	isActive = 'isActive',
	startDate = 'startDate',
	endDate = 'endDate',
	set = 'set',
	clientName = 'clientName',
}

export interface ProjectAction {
	type: ProjectTableFiltersKeys;
	payload?: string | string[] | ProjectsTableFilters | FromToProps<Date>;
}
