import { Roles } from '@/constants/roles';
import { GetTodaysInterviewsResponse } from '@/queries/dashboard/types';

export interface InterviewsGridRow {
	id: number;
	userId: number;
	userFullName: string;
	userPosition: Roles;
	specialization: string;
	time: string;
	status: string;
}

export interface InterviewsGridComponentProps {
	data: GetTodaysInterviewsResponse;
}
