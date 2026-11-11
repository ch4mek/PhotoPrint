import { GetProbationResponse } from '@/queries/dashboard/types';

export interface ProbationGridRow {
	id: number;
	userId: number;
	userFullName: string;
	date: string;
}

export interface ProbationGridComponentProps {
	data: GetProbationResponse;
}
