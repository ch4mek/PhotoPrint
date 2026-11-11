import { UserDevelopmentPlan } from '@/types/developmentPlan';

export interface DevelopmentPlanListProps {
	data: UserDevelopmentPlan[];
	handleSendToReview: (id: number) => void;
}
