import { useMemo } from 'react';

import { PageWithTabs } from '@/components/PageWithTabs';
import { STAFFING_TAB_INDEX_STORAGE_KEY } from '@/constants/storageKeys';
import { useAuth } from '@/context/AuthContext';

import { getStaffingTabs } from './utils/getStaffingTabs';

export function StaffingPage() {
	const { claims } = useAuth();

	const tabs = useMemo(() => getStaffingTabs(claims), [claims]);

	return (
		<PageWithTabs
			tabIndexLocalStorageKey={STAFFING_TAB_INDEX_STORAGE_KEY}
			{...{ tabs }}
		/>
	);
}
