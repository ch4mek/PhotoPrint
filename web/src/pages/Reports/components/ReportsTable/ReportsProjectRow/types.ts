import { ReportsQueryParams } from '@/pages/Reports/types';
import { ProjectResponse } from '@/types/project';

export interface ProjectRowProps {
	project: ProjectResponse;
	userId: number;
	queryParams: ReportsQueryParams;
	isUserRowOpen: boolean;
	isLastItem?: boolean;
}
