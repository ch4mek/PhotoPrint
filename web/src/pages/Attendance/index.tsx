import { PageWithTabs } from '@/components/PageWithTabs';

import {
	ATTENDANCE_TAB_INDEX_LOCAL_STORAGE_KEY,
	attendancePageTabs,
} from './constants';

export function AttendancePage() {
	return (
		<PageWithTabs
			tabs={attendancePageTabs}
			tabIndexLocalStorageKey={ATTENDANCE_TAB_INDEX_LOCAL_STORAGE_KEY}
		/>
	);
}
