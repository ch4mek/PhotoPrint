import { FolderCheckIcon } from '@mms/mms-ui-library/icons/base/FolderCheckIcon';
import { PerformanceReviewIcon } from '@mms/mms-ui-library/icons/base/PerformanceReviewIcon';
import { ProjectsIcon } from '@mms/mms-ui-library/icons/base/ProjectsIcon';
import { SlashIcon } from '@mms/mms-ui-library/icons/base/SlashIcon';

import { UpcomingEventType } from '@/types/home';

const { Contract, PerformanceReview, DevelopmentPlan, Absence } =
	UpcomingEventType;

export const upcomingEventsOptions = {
	[Contract]: {
		title: 'Contract end',
		icon: <ProjectsIcon />,
		color: '#DC7525',
		background: '#F9E9D5',
	},
	[PerformanceReview]: {
		title: PerformanceReview,
		icon: <PerformanceReviewIcon />,
		color: '#244D91',
		background: '#D1E6FD',
	},
	[DevelopmentPlan]: {
		title: DevelopmentPlan,
		icon: <FolderCheckIcon />,
		color: '#9B40E4',
		background: '#E9D5F9',
	},
	[Absence]: {
		title: Absence,
		icon: <SlashIcon />,
		color: '#999999',
		background: '#F2F2F2',
	},
};
