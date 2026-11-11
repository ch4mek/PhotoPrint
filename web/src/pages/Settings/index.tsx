import { useMemo } from 'react';

import { PageWithTabs } from '@/components/PageWithTabs';
import { SETTINGS_TAB_INDEX_STORAGE_KEY } from '@/constants/storageKeys';
import { useAuth } from '@/context/AuthContext';
import { paddingsExcludedTabIds } from '@/pages/Settings/constants';

import { getSettingsTabs } from './utils';

export function SettingsPage() {
	const { role, claims } = useAuth();

	const tabs = useMemo(() => getSettingsTabs(role, claims), [role, claims]);

	return (
		<PageWithTabs
			tabs={tabs}
			tabIndexLocalStorageKey={SETTINGS_TAB_INDEX_STORAGE_KEY}
			{...{ paddingsExcludedTabIds }}
		/>
	);
}
