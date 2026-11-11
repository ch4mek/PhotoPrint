import { lazy } from 'react';

import { TabWithComponent } from '@/types/common';

const DetailedTab = lazy(async () => ({
	default: (await import('./components/DetailedTab')).DetailedTab,
}));

const TotalTab = lazy(async () => ({
	default: (await import('./components/TotalTab')).TotalTab,
}));

export const attendancePageTabs: TabWithComponent[] = [
	{ id: 1, text: 'Total', Component: TotalTab },
	{ id: 2, text: 'Detailed', Component: DetailedTab },
];

export const ATTENDANCE_TAB_INDEX_LOCAL_STORAGE_KEY = 'attendance-panel-tab';
