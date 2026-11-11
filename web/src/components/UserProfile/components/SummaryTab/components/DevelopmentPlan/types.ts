import { ByPageResponse } from '@/queries/types';
import {
	DevelopmentPlanStatus,
	UserDevelopmentPlan,
} from '@/types/developmentPlan';

export interface DevelopmentPlanProps {
	developmentPlan: ByPageResponse<UserDevelopmentPlan> | undefined;
}

export interface DevelopmentPlanSectionRow {
	id: number;
	startDate: string;
	endDate: string | null;
	deadlineDate: string;
	summary: string;
	status: DevelopmentPlanStatus;
	notes: string;
}
