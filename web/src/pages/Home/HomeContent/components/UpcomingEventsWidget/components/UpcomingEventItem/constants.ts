import { UserProfileTabsId } from '@/components/UserProfile/types';
import { UpcomingEventType } from '@/types/home';

export const STATUS_TAG_WIDTH = 85;

export const eventTypeToTabIdMap = {
	[UpcomingEventType.PerformanceReview]: UserProfileTabsId.PERFORMANCE_REVIEW,
	[UpcomingEventType.DevelopmentPlan]: UserProfileTabsId.DEVELOPMENT_PLAN,
	[UpcomingEventType.Contract]: UserProfileTabsId.CONTRACTS,
	[UpcomingEventType.Absence]: UserProfileTabsId.ABSENCES,
} satisfies Record<UpcomingEventType, UserProfileTabsId>;
