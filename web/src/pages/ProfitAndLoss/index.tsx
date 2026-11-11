import { useMemo } from 'react';

import { PageWithTabs } from '@/components/PageWithTabs';
import { PROFIT_AND_LOSS_TAB_INDEX_STORAGE_KEY } from '@/constants/storageKeys';
import { useAuth } from '@/context/AuthContext';

import { getProfitAndLossTabs } from './utils/getProfitAndLossTabs';

export function ProfitAndLossPage() {
	const { claims } = useAuth();

	const tabs = useMemo(() => getProfitAndLossTabs(claims), [claims]);

	return (
		<PageWithTabs
			tabIndexLocalStorageKey={PROFIT_AND_LOSS_TAB_INDEX_STORAGE_KEY}
			{...{ tabs }}
		/>
	);
}
