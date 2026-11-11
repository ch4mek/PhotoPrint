import { UserDevelopmentPlan } from '@/types/developmentPlan';

export interface DevelopmentPlanItemProps {
	item: UserDevelopmentPlan;
	handleSendToReview: (id: number) => void;
	isDesktopView?: boolean;
}
