import { useMemo } from 'react';

import { PageWithTabs } from '@/components/PageWithTabs';
import { useAuth } from '@/context/AuthContext';

import { ADMIN_PANEL_TAB_INDEX_LOCAL_STORAGE_KEY } from './constants';
import { getTabs } from './utils';

export function AdminPanelPage() {
	const { role } = useAuth();
	const tabs = useMemo(() => getTabs(role), [role]);

	return (
		<PageWithTabs
			tabs={tabs}
			tabIndexLocalStorageKey={ADMIN_PANEL_TAB_INDEX_LOCAL_STORAGE_KEY}
		/>
	);
}
