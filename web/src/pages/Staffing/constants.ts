import { lazy } from 'react';

import { staffingTabsIds } from '@/constants/staffing';
import { UserClaims } from '@/types/claims';
import { TabWithComponent } from '@/types/common';

const InterviewTab = lazy(async () => ({
	default: (await import('./components/InterviewTab')).InterviewTab,
}));

const ProjectExitsTab = lazy(async () => ({
	default: (await import('./components/ProjectExitsTab')).ProjectExitsTab,
}));

export const staffingPageTabs: TabWithComponent[] = [
	{
		id: staffingTabsIds.interviewId,
		text: 'Interview',
		Component: InterviewTab,
		claims: [UserClaims.ManageInterviews, UserClaims.ManageAllInterviews],
	},
	{
		id: staffingTabsIds.projectExitsId,
		text: 'Project exits',
		Component: ProjectExitsTab,
		claims: [UserClaims.ViewProjectExits, UserClaims.ManageAllProjectExits],
	},
];
